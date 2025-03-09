using UnityEngine;
using Assets.MicroWar.Scripts;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Defines arrow behaviour
    /// </summary>
    public class Arrow : Throwing
    {
        /// <summary>
        /// Initialize direction and shooter
        /// </summary>
        public override void Initialize(Vector3 direction, Creature shooter)
        {
            base.Initialize(direction, shooter);

            transform.rotation = Quaternion.LookRotation(direction);
        }

        /// <summary>
        /// Defines hit behaviour
        /// </summary>
        /// <param name="creature">Hited creature</param>
        public override void Hit(Creature creature)
        {
            if (creature != null)
            {
                creature.GetDamage(ShooterParams.Damage);
                AudioPlayer.PlayEffect("Hit");
            }

            Destroy(gameObject);
        }

    }
}