using Assets.MicroWar.Scripts.Data;
using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Defines basic behaviour for throwable objects - arrows, fireballs and other
    /// </summary>
    public abstract class Throwing : MonoBehaviour
    {
        /// <summary>
        /// Speed in global space
        /// </summary>
        public float Speed;

        protected Vector3 Direction;
        protected CreatureParams ShooterParams;
        protected CreatureState ShooterState;

        /// <summary>
        /// Default initializer
        /// </summary>
        /// <param name="direction">Object direction</param>
        /// <param name="shooter">Object owner</param>
        public virtual void Initialize(Vector3 direction, Creature shooter)
        {
            Direction = direction;
            ShooterParams = shooter.Params;
            ShooterState = shooter.State;
            Destroy(gameObject, 10);
        }

        public abstract void Hit(Creature creature);

        public void Update()
        {
            transform.position += Direction * Speed * Time.deltaTime;
        }

        /// <summary>
        /// Defines what happens when object hits something
        /// </summary>
        /// <param name="target">Hited collider</param>
        public void OnTriggerEnter(Collider target)
        {
            if (target.isTrigger) return;

            var creature = target.GetComponent<Creature>();

            if (creature == null)
            {
                if (transform.position.z > target.bounds.min.z && transform.position.z < target.bounds.max.z)
                {
                    // Hit(null);
                }
            }
            else if (creature.State.Team != ShooterState.Team && creature.State.Hp > 0)
            {
                Hit(creature);
            }

            // if (target.isTrigger) return;

            // var creature = target.GetComponent<Creature>();
            // Debug.Log("TP1");

            // if (creature == null)
            // {
            // Debug.Log("TP2");
            //     if (transform.position.z > target.bounds.min.z && transform.position.z < target.bounds.max.z)
            //     {
            // Debug.Log("TP3");
            //         Hit(null);
            //     }
            // }
            // else if (creature.State.Team != ShooterState.Team && creature.State.Hp > 0)
            // {
            // Debug.Log("TP4");
            //     Hit(creature);
            // }
        }
    }
}