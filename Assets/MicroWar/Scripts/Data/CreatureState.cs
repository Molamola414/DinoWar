using System;

namespace Assets.MicroWar.Scripts.Data
{
    /// <summary>
    /// Current creature state
    /// </summary>
    [Serializable]
    public class CreatureState
    {
        /// <summary>
        /// Creature team
        /// </summary>
        public int Team;

        /// <summary>
        /// True if this character is controlled by AI
        /// </summary>
        public bool AI;

        /// <summary>
        /// Current health value, from 0 to CreatureParams.HpMax
        /// </summary>
        public float Hp;

        /// <summary>
        /// Current mana value, from 0 to CreatureParams.MpMax
        /// </summary>
        public float Mp;
    }
}