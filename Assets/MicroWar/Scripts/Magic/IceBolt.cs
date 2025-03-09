using UnityEngine;

namespace Assets.MicroWar.Scripts.Magic
{
    /// <summary>
    /// Ice bolt magic behaviour
    /// </summary>
    public class IceBolt : MagicBase
    {
        /// <summary>
        /// Particles played on impact (hit)
        /// </summary>
        public ParticleSystem Impact;

        public new void Update()
        {
            base.Update();

            if (Ball.transform.localScale.x < 4)
            {
                Ball.transform.localScale += 0.10f * new Vector3(1, 2, 1);
            }
        }

        /// <summary>
        /// Defines what happens when ice bolt hits something
        /// </summary>
        /// <param name="creature">Hited creature or null if wall</param>
        public override void Hit(Creature creature) {
            if (creature != null)
            {
                creature.GetDamage(ShooterParams.Damage);

                var impact = Instantiate(Impact);

                impact.transform.parent = transform.parent;
                impact.transform.position = transform.position;
                impact.Play();
                Destroy(impact.gameObject, impact.main.duration);
            }
            else
            {
                Explosion.transform.parent = transform.parent;
                Explosion.Play();
                Destroy(gameObject);
                Destroy(Explosion.gameObject, 1);
            }

            AudioPlayer.PlayEffect("IceBolt");
        }
    }
}