namespace TinkerFlow.Core.Editor.Scenes;

/// <summary>
///   <para>Used when loading a Scene in a player.</para>
/// </summary>
public enum LoadSceneMode
{
    /// <summary>
    ///   <para>Closes all current loaded Scenes
    ///           and loads a Scene.</para>
    /// </summary>
    Single,

    /// <summary>
    ///   <para>Adds the Scene to the current loaded Scenes.</para>
    /// </summary>
    Additive,
}