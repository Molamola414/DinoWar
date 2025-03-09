using UnityEngine;

namespace Assets.MicroWar.Scripts.Common.Tweens
{
    /// <summary>
    /// Rotate object by any axis with defined speed
    /// </summary>
    public class Rotation : TweenBase
    {
        /// <summary>
        /// Rotation axis
        /// </summary>
        public Axis Axis;

        public void Update()
        {
            var angle = Speed * Time.time + Period;
            var eulerAngles = transform.eulerAngles;

            if (Axis == Axis.X) eulerAngles.x = angle;
            if (Axis == Axis.Y) eulerAngles.y = angle;
            if (Axis == Axis.Z) eulerAngles.z = angle;

            transform.eulerAngles = eulerAngles;
        }
    }
}