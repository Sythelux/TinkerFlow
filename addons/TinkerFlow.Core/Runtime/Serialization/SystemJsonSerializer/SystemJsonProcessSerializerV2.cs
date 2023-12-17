using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using Godot;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.EntityOwners;
using VRBuilder.Core.IO;

namespace VRBuilder.Core.Serialization.JSON;

public class SystemJsonProcessSerializerV2 : SystemJsonProcessSerializer
{
    public override byte[] ProcessToByteArray(IProcess target)
    {
        throw new NotImplementedException();
    }

    public override IProcess ProcessFromByteArray(byte[] data)
    {
        string stringData = new UTF8Encoding().GetString(data);

        IProcess process;

        if (Json.ParseString(stringData).Obj is global::Godot.Collections.Dictionary dictionary)
        {
            int version = dictionary["$serializerVersion"].AsInt32();
            switch (version)
            {
                case 1:
                    process = new SystemJsonProcessSerializerV1().ProcessFromByteArray(data);
                    break;
                case 2:
                    var wrapper = JsonSerializer.Deserialize<ProcessWrapper>(stringData, Options);
                    process = wrapper != null ? wrapper.GetProcess() : Process.DEFAULT;
                    break;
                default:
                    process = Process.DEFAULT;
                    break;
            }
        }
        else
        {
            process = Process.DEFAULT;
        }


        return process;
    }

    public override byte[] ChapterToByteArray(IChapter chapter)
    {
        throw new NotImplementedException();
    }

    public override IChapter ChapterFromByteArray(byte[] data)
    {
        throw new NotImplementedException();
    }

    public override byte[] StepToByteArray(IStep step)
    {
        throw new NotImplementedException();
    }

    public override IStep StepFromByteArray(byte[] data)
    {
        throw new NotImplementedException();
    }

    public override byte[] ManifestToByteArray(IProcessAssetManifest manifest)
    {
        throw new NotImplementedException();
    }

    public override IProcessAssetManifest ManifestFromByteArray(byte[] data)
    {
        throw new NotImplementedException();
    }

    [Serializable]
    private class ProcessWrapper : Wrapper
    {
        [DataMember]
        public List<IChapter> SubChapters = new();

        [DataMember]
        public List<IStep> Steps = new();

        [DataMember]
        public IProcess Process;

        public ProcessWrapper()
        {
        }

        public ProcessWrapper(IProcess process)
        {
            foreach (IChapter chapter in process.Data.Chapters)
            {
                // Set LastSelectedStep to null, to prevent it needlessly serializing a full step tree.
                chapter.ChapterMetadata.LastSelectedStep = null;

                Steps.AddRange(GetSteps(chapter));
                SubChapters.AddRange(GetSubChapters(chapter));
            }

            foreach (IStep step in Steps)
            foreach (ITransition transition in step.Data.Transitions.Data.Transitions)
                if (transition.Data.TargetStep != null)
                    transition.Data.TargetStep = new StepRef() { StepMetadata = new StepMetadata() { Guid = transition.Data.TargetStep.StepMetadata.Guid } };

            foreach (IChapter subChapter in SubChapters)
            {
                // Set LastSelectedStep to null, to prevent it needlessly serializing a full step tree.
                subChapter.ChapterMetadata.LastSelectedStep = null;

                List<IStep> stepRefs = new();
                foreach (IStep step in subChapter.Data.Steps)
                {
                    IStep stepRef = new StepRef() { StepMetadata = new StepMetadata() { Guid = step.StepMetadata.Guid } };
                    stepRefs.Add(stepRef);

                    if (subChapter.Data.FirstStep != null && subChapter.Data.FirstStep.StepMetadata.Guid == stepRef.StepMetadata.Guid) subChapter.Data.FirstStep = stepRef;
                }

                subChapter.Data.Steps = stepRefs;
            }

            Process = process;
        }

        public IProcess GetProcess()
        {
            foreach (IStep step in Steps)
            foreach (ITransition transition in step.Data.Transitions.Data.Transitions)
            {
                if (transition.Data.TargetStep == null) continue;

                var stepRef = (StepRef)transition.Data.TargetStep;
                transition.Data.TargetStep = Steps.FirstOrDefault(step => step.StepMetadata.Guid == stepRef.StepMetadata.Guid);
            }

            foreach (IChapter subChapter in SubChapters)
            {
                List<IStep> steps = new();

                foreach (IStep stepRef in subChapter.Data.Steps)
                {
                    IStep step = Steps.FirstOrDefault(step => step.StepMetadata.Guid == stepRef.StepMetadata.Guid);
                    steps.Add(step);

                    if (subChapter.Data.FirstStep != null && subChapter.Data.FirstStep.StepMetadata.Guid == stepRef.StepMetadata.Guid) subChapter.Data.FirstStep = step;
                }

                subChapter.Data.Steps = steps;
            }

            return Process;
        }
    }

    private class Wrapper
    {
        protected IEnumerable<IStep> GetSteps(IChapter chapter)
        {
            List<IStep> steps = new();

            steps.AddRange(chapter.Data.Steps);

            IEnumerable<IChapter> subChapters = chapter.Data.Steps.SelectMany(step => step.Data.Behaviors.Data.Behaviors.Where(behavior => behavior.Data is IEntityCollectionData<IChapter>))
                .Select(behavior => behavior.Data).Cast<IEntityCollectionData<IChapter>>()
                .SelectMany(behavior => behavior.GetChildren());

            foreach (IChapter subChapter in subChapters) steps.AddRange(GetSteps(subChapter));

            return steps;
        }

        protected IEnumerable<IChapter> GetSubChapters(IChapter chapter)
        {
            List<IChapter> subChapters = new();

            foreach (IStep step in chapter.Data.Steps)
            foreach (IBehavior behavior in step.Data.Behaviors.Data.Behaviors)
                if (behavior.Data is IEntityCollectionData<IChapter> data)
                {
                    subChapters.InsertRange(0, data.GetChildren());

                    foreach (IChapter subChapter in data.GetChildren()) subChapters.InsertRange(0, GetSubChapters(subChapter));
                }

            return subChapters;
        }

        [Serializable]
        public class StepRef : IStep
        {
            IData IDataOwner.Data { get; } = null;

            IStepData IDataOwner<IStepData>.Data { get; } = null;

            public ILifeCycle LifeCycle { get; } = null;

            public IStageProcess GetActivatingProcess()
            {
                throw new NotImplementedException();
            }

            public IStageProcess GetActiveProcess()
            {
                throw new NotImplementedException();
            }

            public IStageProcess GetDeactivatingProcess()
            {
                throw new NotImplementedException();
            }

            public void Configure(IMode mode)
            {
                throw new NotImplementedException();
            }

            public void Update()
            {
                throw new NotImplementedException();
            }

            public IStep Clone()
            {
                throw new NotImplementedException();
            }

            public StepMetadata StepMetadata { get; set; }
            public IEntity Parent { get; set; }
        }
    }
}