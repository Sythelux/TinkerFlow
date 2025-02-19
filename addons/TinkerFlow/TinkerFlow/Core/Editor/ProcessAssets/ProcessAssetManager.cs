// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using VRBuilder.Core;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.IO;
using VRBuilder.Core.Serialization;
using VRBuilder.Core.Utils;
using VRBuilder.Editor.Configuration;
using FileAccess = Godot.FileAccess;

namespace VRBuilder.Editor
{
    /// <summary>
    /// A static class that handles the process assets. It lets you to save, load, delete, and import processes and provides multiple related utility methods.
    /// </summary>
    internal static class ProcessAssetManager
    {
        private static FileSystemWatcher watcher;
        private static bool isSaving;
        private static object lockObject = new();

        /// <summary>
        /// Called when an external change to the process file is detected.
        /// </summary>
        internal static event EventHandler ExternalFileChange;

        /// <summary>
        /// Deletes the process with <paramref name="processName"/>.
        /// </summary>
        internal static void Delete(string processName)
        {
            if (ProcessAssetUtils.DoesProcessAssetExist(processName)) Directory.Delete(ProcessAssetUtils.GetProcessAssetDirectory(processName), true);
            //TODO: is godot doing this automatically AssetDatabase.Refresh();
        }

        /// <summary>
        /// Imports the given <paramref name="process"/> by saving it to the proper directory. If there is a name collision, this process will be renamed.
        /// </summary>
        internal static void Import(IProcess process)
        {
            var counter = 0;
            string oldName = process.Data.Name;
            while (ProcessAssetUtils.DoesProcessAssetExist(process.Data.Name))
            {
                if (counter > 0) process.Data.SetName(process.Data.Name.Substring(0, process.Data.Name.Length - 2));

                counter++;
                process.Data.SetName(process.Data.Name + " " + counter);
            }

            if (oldName != process.Data.Name) GD.PushWarning($"We detected a name collision while importing process \"{oldName}\". We have renamed it to \"{process.Data.Name}\" before importing.");

            Save(process);
        }

        /// <summary>
        /// Imports the process from file at given file <paramref name="path"/> if the file extensions matches the <paramref name="serializer"/>.
        /// </summary>
        internal static void Import(string path, IProcessSerializer serializer)
        {
            IProcess process;

            if (Path.GetExtension(path) != $".{serializer.FileFormat}") GD.PushError($"The file extension of {path} does not match the expected file extension of {serializer.FileFormat} of the current serializer.");

            try
            {
                byte[] file = FileAccess.GetFileAsBytes(path);
                process = serializer.ProcessFromByteArray(file);
            }
            catch (Exception e)
            {
                GD.PushError($"{e.GetType().Name} occured while trying to import file '{path}' with serializer '{serializer.GetType().Name}'\n\n{e.StackTrace}");
                return;
            }

            Import(process);
        }

        /// <summary>
        /// Save the <paramref name="process"/> to the file system.
        /// </summary>
        internal static void Save(IProcess process)
        {
            try
            {
                IDictionary<string, byte[]> assetData = EditorConfigurator.Instance.ProcessAssetStrategy.CreateSerializedProcessAssets(process, EditorConfigurator.Instance.Serializer);
                List<string> filesToDelete = new();
                string processDirectory = ProcessAssetUtils.GetProcessAssetDirectory(process.Data.Name);

                if (Directory.Exists(processDirectory)) filesToDelete.AddRange(Directory.GetFiles(processDirectory, $"*.{EditorConfigurator.Instance.Serializer.FileFormat}"));

                foreach (string fileName in assetData.Keys)
                {
                    var fullFileName = $"{fileName}.{EditorConfigurator.Instance.Serializer.FileFormat}";
                    filesToDelete.Remove(filesToDelete.FirstOrDefault(file => file.EndsWith(fullFileName)));
                    var path = $"{processDirectory}/{fullFileName}";

                    WriteFileIfChanged(assetData[fileName], path);
                }

                if (EditorConfigurator.Instance.ProcessAssetStrategy.CreateManifest)
                {
                    byte[] manifestData = CreateSerializedManifest(assetData);
                    var fullManifestName = $"{BaseRuntimeConfiguration.ManifestFileName}.{EditorConfigurator.Instance.Serializer.FileFormat}";
                    var manifestPath = $"{processDirectory}/{fullManifestName}";

                    WriteFileIfChanged(manifestData, manifestPath);

                    filesToDelete.Remove(filesToDelete.FirstOrDefault(file => file.EndsWith($"{fullManifestName}")));
                }

                DeleteFiles(filesToDelete);
            }
            catch (Exception ex)
            {
                GD.PushError(ex);
            }
        }

        private static void DeleteFiles(IEnumerable<string> filesToDelete)
        {
            foreach (string file in filesToDelete)
            {
                GD.Print($"File deleted: {file}");
                DirAccess.RemoveAbsolute(file);
            }
        }

        private static byte[] CreateSerializedManifest(IDictionary<string, byte[]> assetData)
        {
            IProcessAssetManifest manifest = new ProcessAssetManifest()
            {
                AssetStrategyTypeName = EditorConfigurator.Instance.ProcessAssetStrategy.GetType().FullName,
                AdditionalFileNames = assetData.Keys.Where(name => name != assetData.Keys.First()).ToList(),
                ProcessFileName = assetData.Keys.First()
            };

            byte[] manifestData = EditorConfigurator.Instance.Serializer.ManifestToByteArray(manifest);
            return manifestData;
        }

        private static void WriteFileIfChanged(byte[] data, string path)
        {
            var storedData = new byte[0];

            if (FileAccess.FileExists(path)) storedData = FileAccess.GetFileAsBytes(path);

            if (Enumerable.SequenceEqual(storedData, data) == false)
            {
                // if (AssetDatabase.MakeEditable(path))
                // {
                WriteProcessFile(path, data);
                GD.Print($"File saved: \"{path}\"");
                // }
                // else
                // {
                //     GD.PushError($"Saving of \"{path}\" failed! Could not make it editable.");
                // }
            }
        }

        private static void WriteProcessFile(string path, byte[] processData)
        {
            lock (lockObject)
            {
                isSaving = true;
            }

            FileAccess stream = null;
            try
            {
                // if (FileAccess.FileExists(path))
                //     FileAccess.Set(path, FileAttributes.Normal);

                DirAccess? dir = DirAccess.Open(Path.GetDirectoryName(path));
                dir?.MakeDirRecursive(Path.GetDirectoryName(path));
                stream = FileAccess.Open(path, FileAccess.ModeFlags.ReadWrite);
                stream.StoreBuffer(processData);
                // stream.Write(processData, 0, processData.Length);
                stream.Close();
            }
            catch (Exception ex)
            {
                GD.PushError(ex);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// Loads the process with the given <paramref name="processName"/> from the file system and converts it into the <seealso cref="IProcess"/> instance.
        /// </summary>
        internal static IProcess Load(string processName)
        {
            if (ProcessAssetUtils.DoesProcessAssetExist(processName))
            {
                var manifestPath = $"{ProcessAssetUtils.GetProcessAssetDirectory(processName)}/{BaseRuntimeConfiguration.ManifestFileName}.{EditorConfigurator.Instance.Serializer.FileFormat}";

                IProcessAssetManifest manifest = CreateProcessManifest(processName, manifestPath);
                var assetStrategy = ReflectionUtils.CreateInstanceOfType(ReflectionUtils.GetConcreteImplementationsOf<IProcessAssetStrategy>().FirstOrDefault(type => type.FullName == manifest.AssetStrategyTypeName)) as IProcessAssetStrategy;
                List<byte[]> additionalData = LoadAdditionalDataFromManifest(processName, manifest);

                string processAssetPath = ProcessAssetUtils.GetProcessAssetPath(processName);
                byte[] processData = FileAccess.GetFileAsBytes(processAssetPath);

                SetupWatcher(processName);

                try
                {
                    return assetStrategy.GetProcessFromSerializedData(processData, additionalData, EditorConfigurator.Instance.Serializer);
                }
                catch (Exception ex)
                {
                    GD.PushError($"Failed to load the process '{processName}' from '{processAssetPath}' because of: \n{ex.Message}");
                    GD.PushError(ex);
                }
            }

            return null;
        }

        /// <summary>
        /// Renames the <paramref name="process"/> to the <paramref name="newName"/> and moves it to the appropriate directory. Check if you can rename before with the <seealso cref="CanRename"/> method.
        /// </summary>
        internal static void RenameProcess(IProcess process, string newName)
        {
            if (ProcessAssetUtils.CanRename(process, newName, out string errorMessage) == false)
            {
                GD.PushError($"Process {process.Data.Name} was not renamed because:\n\n{errorMessage}");
                return;
            }

            string oldDirectory = ProcessAssetUtils.GetProcessAssetDirectory(process.Data.Name);
            string newDirectory = ProcessAssetUtils.GetProcessAssetDirectory(newName);

            Directory.Move(oldDirectory, newDirectory);
            File.Move($"{oldDirectory}.meta", $"{newDirectory}.meta");

            string newAsset = ProcessAssetUtils.GetProcessAssetPath(newName);
            var oldAsset = $"{ProcessAssetUtils.GetProcessAssetDirectory(newName)}/{process.Data.Name}.{EditorConfigurator.Instance.Serializer.FileFormat}";
            File.Move(oldAsset, newAsset);
            File.Move($"{oldAsset}.meta", $"{newAsset}.meta");
            process.Data.SetName(newName);

            Save(process);

            RuntimeConfigurator.Instance.SetSelectedProcess(newAsset);
        }

        /// <summary>
        /// Creates a new <seealso cref="IProcessAssetManifest"/> for the given <paramref name="processName"/> and <paramref name="manifestPath"/>.
        /// If a <paramref name="manifestPath"/> file does not exist the process was saved with the <seealso cref="SingleFileProcessAssetStrategy"/>.
        /// This strategy does not include a manifest file.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <param name="manifestPath">The path including filename to a manifest file.</param>
        /// <returns></returns>
        private static IProcessAssetManifest CreateProcessManifest(string processName, string manifestPath)
        {
            IProcessAssetManifest manifest;
            if (FileAccess.FileExists(manifestPath))
            {
                byte[] manifestData = FileAccess.GetFileAsBytes(manifestPath);
                manifest = EditorConfigurator.Instance.Serializer.ManifestFromByteArray(manifestData);
            }
            else
            {
                manifest = new ProcessAssetManifest()
                {
                    AssetStrategyTypeName = typeof(SingleFileProcessAssetStrategy).FullName,
                    AdditionalFileNames = new List<string>(),
                    ProcessFileName = processName
                };
            }

            return manifest;
        }

        /// <summary>
        /// Loads all data from the files inside <see cref="IProcessAssetManifest.AdditionalFileNames"/> and returns it.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <param name="manifest">The manifest object.</param>
        /// <returns>A list of byte arrays representing additional data.</returns>
        private static List<byte[]> LoadAdditionalDataFromManifest(string processName, IProcessAssetManifest manifest)
        {
            List<byte[]> additionalData = new();
            foreach (string fileName in manifest.AdditionalFileNames)
            {
                var path = $"{ProcessAssetUtils.GetProcessAssetDirectory(processName)}/{fileName}.{EditorConfigurator.Instance.Serializer.FileFormat}";

                if (FileAccess.FileExists(path))
                    additionalData.Add(FileAccess.GetFileAsBytes(path));
                else
                    GD.Print($"Error loading process. File not found: {path}");
            }

            return additionalData;
        }

        private static void SetupWatcher(string processName)
        {
            if (watcher == null)
            {
                watcher = new FileSystemWatcher();
                watcher.Changed += OnFileChanged;
            }

            watcher.Path = ProjectSettings.GlobalizePath(ProcessAssetUtils.GetProcessAssetDirectory(processName));
            watcher.Filter = $"*.{EditorConfigurator.Instance.Serializer.FileFormat}";

            watcher.EnableRaisingEvents = true;
        }

        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (isSaving)
            {
                lock (lockObject)
                {
                    isSaving = false;
                }

                return;
            }

            ExternalFileChange?.Invoke(null, EventArgs.Empty);
        }
    }
}