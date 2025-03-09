using System;

namespace Assets.MicroWar.Scripts.Data
{
    /// <summary>
    /// Common params for AI that controls creatures
    /// </summary>
    [Serializable]
    public class AIParams
    {
        /// <summary>
        /// Time that AI do something, in seconds
        /// </summary>
        public float ActionTime;

        /// <summary>
        /// Time that AI do nothing, in seconds
        /// </summary>
        public float IdleTime;

        /// <summary>
        /// Keep distance from player, in world space
        /// </summary>
        public float KeepDistance;

        /// <summary>
        /// Attack timeout, in seconds
        /// </summary>
        public float AttackTimeout;
    }
}