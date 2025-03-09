using System.Linq;
using Assets.MicroWar.Scripts.Common.Tweens;
using UnityEngine;

namespace Assets.MicroWar.Scripts.Magic
{
    public class PoisonGlob : MagicBase
    {
        private float _z;

        /// <summary>
        /// Initialize poison glob
        /// </summary>
        /// <param name="direction">Poison glob direction</param>
        /// <param name="shooter">Poison glob owner to prevent self damage</param>
        public override void Initialize(Vector3 direction, Creature shooter)
        {
            base.Initialize(direction, shooter);
            Ball.GetComponent<Rotation>().Speed *= -direction.magnitude;
            _z = transform.position.z;
        }

        public new void Update()
        {
            base.Update();

            if (Ball.transform.localScale.x < 4)
            {
                Ball.transform.localScale += 0.25f * Vector3.one;
            }
            transform.Translate(new Vector3(1 * Mathf.Sin(20 * Time.time), 0, 0));
            // transform.position = new Vector3(transform.position.x, transform.position.y, _z + 4 * Mathf.Sin(20 * Time.time));
        }

        /// <summary>
        /// Defines what happens when poison glob hits something
        /// </summary>
        /// <param name="creature">Hited creature or null if wall</param>
        public override void Hit(Creature creature)
        {
            const int radius = 8;
            var victims = Physics.OverlapSphere(creature == null ? transform.position : creature.transform.position, radius).Where(i => !i.isTrigger);

            foreach (var victim in victims)
            {
                var target = victim.GetComponent<Creature>();

                if (target != null && target.State.Hp > 0 && target.State.Team != ShooterState.Team)
                {
                    target.GetDamage(ShooterParams.Damage);
                    Poison.Set(target, 0.5f, 10);
                }
            }

            Explosion.transform.parent = transform.parent;
            Explosion.Play();
            Destroy(gameObject);
            Destroy(Explosion.gameObject, 1);
            AudioPlayer.PlayEffect("PoisonGlob");
        }
    }
}