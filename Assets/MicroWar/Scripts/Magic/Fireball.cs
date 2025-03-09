using System.Linq;
using Assets.MicroWar.Scripts.Common.Tweens;
using UnityEngine;

namespace Assets.MicroWar.Scripts.Magic
{
    /// <summary>
    /// Fireball magic behaviour
    /// </summary>
    public class Fireball : MagicBase
    {
        /// <summary>
        /// Initialize fireball
        /// </summary>
        /// <param name="direction">Fireball direction</param>
        /// <param name="shooter">Fireball owner to prevent self damage</param>
        public override void Initialize(Vector3 direction, Creature shooter)
        {
            base.Initialize(direction, shooter);
            Ball.GetComponent<Rotation>().Speed *= -direction.magnitude;
        }

        public new void Update()
        {
            base.Update();

            if (Ball.transform.localScale.x < 6)
            {
                Ball.transform.localScale += 0.10f * Vector3.one;
            }
        }

        /// <summary>
        /// Defines what happens when fireball hits something
        /// </summary>
        /// <param name="creature">Hited creature or null if wall</param>
        public override void Hit(Creature creature) {
            const int radius = 16;
            var victims = Physics.OverlapSphere(creature == null ? transform.position : creature.transform.position, radius);

            foreach (var victim in victims.Where(i => !i.isTrigger))
            {
                var target = victim.GetComponent<Creature>();

                if (target != null && target.State.Team != ShooterState.Team)
                {
                    var distance = Mathf.Max(1, Vector2.Distance(transform.position, target.transform.position));
                    var damage = 5 / distance * ShooterParams.Damage;

                    target.GetDamage(damage);
                }
            }

            Explosion.transform.parent = transform.parent;
            Explosion.Play();
            Destroy(gameObject);
            Destroy(Explosion.gameObject, 1);
            AudioPlayer.PlayEffect("Fireball");
        }
    }
}