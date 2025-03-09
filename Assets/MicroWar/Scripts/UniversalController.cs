using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Universal controller used to control creatures with keyboard and CrossPlatformInput
    /// </summary>
    [RequireComponent(typeof(Creature))]
    public class UniversalController : MonoBehaviour
    {
        public KeyCode Left = KeyCode.LeftArrow;
        public KeyCode Right = KeyCode.RightArrow;
        public KeyCode Down = KeyCode.DownArrow;
        public KeyCode Up = KeyCode.UpArrow;
        public KeyCode Jump = KeyCode.J;
        public KeyCode Attack = KeyCode.Space;

        public void Update()
        {
            var creature = GetComponent<Creature>();
            var direction = Vector2.zero;
            var atkDirection = Vector2.zero;

            if (Engine.Instance.joystickWalk != null) 
            {
                direction = Engine.Instance.joystickWalk.Direction;
            }
            if(Engine.Instance.joystickWeapon != null)
            {
                atkDirection = Engine.Instance.joystickWeapon.Direction;
            }

            /*
            if (Input.GetKey(Left))
            {
                direction.x -= 1;
            }
            else if (Input.GetKey(Right))
            {
                direction.x += 1;
            }
            else
            {
                var dx = CrossPlatformInputManager.GetAxis("Horizontal");

                if (Mathf.Abs(dx) > 0.25) direction.x = Mathf.Sign(dx);
            }

            if (Input.GetKey(Down))
            {
                direction.y -= 1;
            }
            else if (Input.GetKey(Up))
            {
                direction.y += 1;
            }
            else
            {
                var dy = CrossPlatformInputManager.GetAxis("Vertical");

                if (Mathf.Abs(dy) > 0.25) direction.y = Mathf.Sign(dy);
            }
             */

            creature.Controls.Direction = direction;
            creature.Controls.Jump = Input.GetKey(Jump);
            creature.Controls.AttackDirection = atkDirection.normalized;
            // creature.Controls.Attack = Input.GetKey(Attack);

            // #if MOBILE_INPUT

            // creature.Controls.Jump |= CrossPlatformInputManager.GetButton("Jump");
            // creature.Controls.Attack |= CrossPlatformInputManager.GetButton("Attack");

            // #endif
        }
    }
}