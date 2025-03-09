using UnityEngine;

namespace Assets.MicroWar.Scripts.Magic
{
    /// <summary>
    /// Base abstract class for throwable magic
    /// </summary>
    public abstract class MagicBase : Throwing
    {
        /// <summary>
        /// Throwable item
        /// </summary>
        public GameObject Ball;

        /// <summary>
        /// Flow object
        /// </summary>
        public GameObject Flow;

        /// <summary>
        /// Explosion effect (particles)
        /// </summary>
        public ParticleSystem Explosion;

        /// <summary>
        /// Default initializer
        /// </summary>
        /// <param name="direction">Magic direction</param>
        /// <param name="shooter">Magic owner to prevent self damage</param>
        public override void Initialize(Vector3 direction, Creature shooter)
        {
            base.Initialize(direction, shooter);
            transform.rotation = Quaternion.LookRotation(direction);
            // Flow.transform.Rotate(0, direction.x < 0 ? 180 : 0, 0);
        }

    }
}