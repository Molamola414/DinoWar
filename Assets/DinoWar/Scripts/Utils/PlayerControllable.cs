using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Universal controller used to control creatures with keyboard and CrossPlatformInput
/// </summary>
[RequireComponent(typeof(Creature))]
public class PlayerControllable : MonoBehaviour
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

        if (Input.GetKey(Left))
        {
            direction.x -= 1f;
        }
        else if (Input.GetKey(Right))
        {
            direction.x += 1f;
        }
        else
        {
            if (BattleManager.Instance.joystickWalk != null) 
                direction.x = BattleManager.Instance.joystickWalk.Direction.x;

        }

        if (Input.GetKey(Down))
        {
            direction.y -= 1f;
        }
        else if (Input.GetKey(Up))
        {
            direction.y += 1f;
        }
        else
        {
            if (BattleManager.Instance.joystickWalk != null) 
                direction.y = BattleManager.Instance.joystickWalk.Direction.y;
        }

        if(BattleManager.Instance.joystickWeapon != null)
        {
            atkDirection = BattleManager.Instance.joystickWeapon.Direction;
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

        creature.controls.Direction = direction;
        creature.controls.Jump = Input.GetKey(Jump);
        creature.controls.AttackDirection = atkDirection.normalized;
        // creature.Controls.Attack = Input.GetKey(Attack);

        // #if MOBILE_INPUT

        // creature.Controls.Jump |= CrossPlatformInputManager.GetButton("Jump");
        // creature.Controls.Attack |= CrossPlatformInputManager.GetButton("Attack");

        // #endif
    }
}