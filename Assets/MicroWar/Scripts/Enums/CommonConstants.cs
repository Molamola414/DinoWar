namespace Assets.MicroWar.Scripts.Enums
{
    /// <summary>
    /// Creature attack type
    /// </summary>
    public enum AttackType
    {
        Melee,
        Ranged,
        Magic
    }

    public enum BattleStatus
    {
        Ready,
        Start,
        Pause,
        Win,
        Lose,
        Result
    }
}