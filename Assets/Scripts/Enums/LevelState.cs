namespace Enums
{
    /// <summary>
    /// Level state.
    /// Enum used mostly by LevelManager and GUI scripts
    /// </summary>
    public enum LevelState
    {
        Start, // before first wave
        Wave, // mobs are on the map
        InterWave, // break time between two wave
        Finish, // last wave finished, player can leave the level
        PlayerDead // the player died, override all other state
    }
}
