using System.Collections.Generic;
using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Weapon collider which stores targets, used to define targets when creature attacks
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class WeaponBox : MonoBehaviour
    {
        /// <summary>
        /// Collider
        /// </summary>
        public BoxCollider Collider;

        /// <summary>
        /// List of targets available to hit
        /// </summary>
        public List<Creature> Targets;

        /// <summary>
        /// Target creature enters weapon collider
        /// </summary>
        /// <param name="target">Target</param>
        public void OnTriggerEnter(Collider target)
        {
            var creature = target.GetComponentInChildren<Creature>();

            if (creature != null && !Targets.Contains(creature))
            {
                Targets.Add(creature);
            }
        }

        /// <summary>
        /// Target creature exits weapon collider
        /// </summary>
        /// <param name="target">Target</param>
        public void OnTriggerExit(Collider target)
        {
            var creature = target.GetComponentInChildren<Creature>();

            if (creature != null && Targets.Contains(creature))
            {
                Targets.Remove(creature);
            }
        }
    }
}