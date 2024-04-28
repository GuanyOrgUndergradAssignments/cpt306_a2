/// <summary>
/// The audio manager is a static collection of audio variables.
/// 
/// Volume:
///     There is a master volume.
///     Then for each category of music, there is a that volume.
///     The category's final volume is that volume * the master volume,
///     which is obtained through catnameStrength().
/// </summary>
public class AudioManager
{
    public static float masterVolume = 1.0f;
    public static float musicVolume = 1.0f;
    public static float effectsVolume = 1.0f;

    /// <returns>master * music</returns>
    public static float musicStrength()
    {
        return masterVolume * musicVolume;
    }

    /// <returns>master * effects</returns>
    public static float effectsStrength()
    {
        return masterVolume * effectsVolume;
    }
}
