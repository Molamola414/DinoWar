using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Poison effect, created by poison glob magic
    /// </summary>
    public class Poison : MonoBehaviour
    {
        private float _damage;
        private float _duration;
        private float _time;
        
        /// <summary>
        /// Initialize posion effect
        /// </summary>
        /// <param name="creature">Target</param>
        /// <param name="damage">Damage caused every update frame</param>
        /// <param name="duration">Duration</param>
        public static void Set(Creature creature, float damage, float duration)
        {
            var poison = creature.GetComponent<Poison>() ?? creature.gameObject.AddComponent<Poison>();

            poison.Set(damage, duration);
            creature.GetComponentInChildren<StatusBars>().Poisoned = true;
        }

        private void Set(float damage, float duration)
        {
            _damage = damage;
            _duration = duration;
            _time = Time.time;
        }

        public void Update()
        {
            var creature = GetComponent<Creature>();

            if (Time.time < _time + _duration && creature.State.Hp > 0)
            {
                creature.GetDamage(_damage, spring: false);
            }
            else
            {
                creature.GetComponentInChildren<StatusBars>().Poisoned = true;
                Destroy(this);
            }
        }
    }
}