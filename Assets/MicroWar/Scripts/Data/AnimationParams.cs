using System;

namespace Assets.MicroWar.Scripts.Data
{
    /// <summary>
    /// Common animation params
    /// </summary>
    [Serializable]
    public class AnimationParams
    {
        /// <summary>
        /// Stand clip name
        /// </summary>
        public string Stand;

        /// <summary>
        /// Move clip name
        /// </summary>
        public string Move;

        /// <summary>
        /// Jump clip name
        /// </summary>
        public string Jump;

        /// <summary>
        /// Attack clip name
        /// </summary>
        public string Attack;

        /// <summary>
        /// Attack in jump clip name
        /// </summary>
        public string AttackInJump;

        /// <summary>
        /// Death clip name
        /// </summary>
        public string Death;

        /// <summary>
        /// Upper body layer index if layers used
        /// </summary>
        public int UpperLayerIndex;
    }
}