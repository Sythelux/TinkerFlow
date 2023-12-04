using Godot;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Utils;

/// <summary>
/// Default process audio player.
/// </summary>
public class DefaultAudioPlayer : IProcessAudioPlayer
{
    private /*AudioSource*/ AudioStreamPlayer audioSource;

    public DefaultAudioPlayer()
    {
        Node user = RuntimeConfigurator.Configuration.LocalUser.Head;

        audioSource = user.GetComponent<AudioStreamPlayer>() ?? user.AddComponent<AudioStreamPlayer>();
    }

    public DefaultAudioPlayer(AudioStreamPlayer audioSource)
    {
        this.audioSource = audioSource;
    }

    /// <inheritdoc />
    public AudioStreamPlayer FallbackAudioSource => audioSource;

    /// <inheritdoc />
    public bool IsPlaying => audioSource.Playing;

    /// <inheritdoc />
    public void PlayAudio(AudioStream audioData, float volume = 1, float pitch = 1)
    {
        audioSource.Stream = audioData;
        audioSource.VolumeDb = volume;
        audioSource.PitchScale = pitch;
        audioSource.Play();
    }

    /// <inheritdoc />
    public void Reset()
    {
        audioSource.Stream = null;
    }

    /// <inheritdoc />
    public void Stop()
    {
        audioSource.Stop();
        audioSource.Stream = null;
    }
}
