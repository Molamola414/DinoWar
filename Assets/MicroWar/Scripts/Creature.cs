using System.Linq;
using Assets.MicroWar.Scripts.Common.Tweens;
using Assets.MicroWar.Scripts.Data;
using Assets.MicroWar.Scripts.Enums;
using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Base creature behaviour, controlled by AI or by player
    /// </summary>
    [RequireComponent(typeof(CharacterController), typeof(Animator))]
    public class Creature : MonoBehaviour
    {
        /// <summary>
        /// Creature params
        /// </summary>
        public CreatureParams Params;

        /// <summary>
        /// Animation params
        /// </summary>
        public AnimationParams AnimationParams;

        /// <summary>
        /// Current creature state
        /// </summary>
        public CreatureState State;

        /// <summary>
        /// Status bars, MP and HP
        /// </summary>
        public StatusBars StatusBars;

        /// <summary>
        /// Creature body
        /// </summary>
        public Transform Body;

        /// <summary>
        /// Creature weapon
        /// </summary>
        public WeaponBox WeaponBox;

        /// <summary>
        /// Creature controls
        /// </summary>
        public Controls Controls = new Controls();

        /// <summary>
        /// Character controller
        /// </summary>
        public CharacterController Controller;

        private Vector3 _direction = Vector3.zero;
        private const int Gravity = 320;

        private float Hp
        {
            get { return State.Hp; }
            set { State.Hp = value; StatusBars.SetHp(State.Hp, Params.HpMax); }
        }

        private float Mp
        {
            get { return State.Mp; }
            set { State.Mp = value; StatusBars.SetMp(State.Mp, Params.MpMax); }
        }
       
        public void Start()
        {
            State.Hp = Params.HpMax;
            State.Mp = Params.MpMax;
            Engine.Instance.Creatures.Add(this);
        }

        public void OnDestroy()
        {
            Engine.Instance.Creatures.Remove(this);
        }

        /// <summary>
        /// Move, jump, attack and other actions
        /// </summary>
        public void Update()
        {
            if (State.Hp > 0)
            {
                // Controls.Normalize();
                Attack();
                Move();
                RestoreMp();
            }
            else
            {
                Fall();
            }
        }

        /// <summary>
        /// Receive damage from other creature
        /// </summary>
        /// <param name="damage">Damage value</param>
        /// <param name="spring">Spring scale if true</param>
        public void GetDamage(float damage, bool spring = true)
        {
            Hp -= damage;
            Body.GetComponent<ScaleSpring>().enabled = spring;

            if (Hp <= 0)
            {
                GetComponent<Animator>().Play(AnimationParams.Death);
                AudioPlayer.PlayEffect("Death");
            }
        }

        /// <summary>
        /// Turn creature to desired direction
        /// </summary>
        public void Turn(float direction)
        {
            if (direction * Body.localScale.x >= 0) return;

            direction = Mathf.Sign(direction);
            Body.localScale = new Vector3(direction, 1, 1);
            WeaponBox.Collider.center = new Vector3(direction * Mathf.Abs(WeaponBox.Collider.center.x), WeaponBox.Collider.center.y, WeaponBox.Collider.center.z);
        }

        #region Events

        /// <summary>
        /// Called from animation when hit
        /// </summary>
        public void OnHit()
        {
            foreach (var target in WeaponBox.Targets.Where(i => i.State.Team != State.Team && i.State.Hp > 0))
            {
                target.GetDamage(Params.Damage);
            }

            WeaponBox.Targets.RemoveAll(i => i.State.Hp <= 0);
            AudioPlayer.PlayEffect("Hit");
        }

        /// <summary>
        /// Called from animation when shot
        /// </summary>
        public void OnShot()
        {
            if (Controls.AttackDirection == Vector2.zero) return;

            var direction = Mathf.Sign(Controls.AttackDirection.x);
            // var direction = Mathf.Sign(Body.localScale.x);
            var shot = Instantiate(Params.ShotPrefab, transform.parent);

            shot.transform.position = transform.position + new Vector3(Params.ShotSpot.x * direction, Params.ShotSpot.y);
            shot.Initialize(new Vector3(Controls.AttackDirection.x, 0, Controls.AttackDirection.y), this);
            AudioPlayer.PlayEffect("Shot");
        }

        /// <summary>
        /// Called from animation when cast
        /// </summary>
        public void OnCast()
        {
            if (Controls.AttackDirection == Vector2.zero) return;

            if (Mp < Params.MpConsumption) return;

            Mp -= Params.MpConsumption;
            
            var direction = Mathf.Sign(Controls.AttackDirection.x);
            // var direction = Mathf.Sign(Body.localScale.x);
            var shot = Instantiate(Params.ShotPrefab, transform.parent);

            shot.transform.position = transform.position + new Vector3(Params.ShotSpot.x * direction, Params.ShotSpot.y);
            shot.Initialize(new Vector3(Controls.AttackDirection.x, 0, Controls.AttackDirection.y), this);
            AudioPlayer.PlayEffect("Cast");
        }

        /// <summary>
        /// Called from animation when die
        /// </summary>
        public void OnDie()
        {
            foreach (var eye in GetComponentsInChildren<Eye>())
            {
                eye.OnDie();
            }

            StatusBars.Enable(false, false);
            gameObject.AddComponent<DeadBlink>();
        }

        #endregion

        private void Attack()
        {
            if (Controls.AttackDirection == Vector2.zero) return;
            // if (!Controls.Attack) return;

            var animator = GetComponent<Animator>();

            Turn(Controls.AttackDirection.x);
            
            if (AnimationParams.UpperLayerIndex > 0)
            {
                animator.Play(Controller.isGrounded ? AnimationParams.Attack : AnimationParams.AttackInJump, AnimationParams.UpperLayerIndex);
            }
            else
            {
                animator.Play(AnimationParams.Attack);
            }
        }

        private void Move()
        {
            var animator = GetComponent<Animator>();

            if (AnimationParams.UpperLayerIndex > 0 || !IsPlaying(animator, AnimationParams.Attack))
            {
                if (Controller.isGrounded)
                {
                    _direction = Params.Speed * (new Vector3(Controls.Direction.x, 0, Controls.Direction.y)).normalized;

                    if (Controls.Jump)
                    {
                        _direction.y = Params.Jump;
                    }
                    
                    if (Controls.Direction.magnitude > 0)
                    {
                        if(Controls.AttackDirection.x == 0)
                        {
                            Turn(Controls.Direction.x);
                        }
                        animator.Play(AnimationParams.Move);
                    }
                    else
                    {
                        animator.Play(AnimationParams.Stand);
                    }
                }
                else if (Params.Jump > 0 && Controller.velocity.magnitude > 0.01)
                {
                    animator.Play(AnimationParams.Jump);
                }
            }
            else
            {
                _direction = Vector3.zero;
            }

            _direction.y -= Gravity * Time.deltaTime;
            Controller.Move(_direction * Time.deltaTime);
        }

        private void RestoreMp()
        {
            if (Params.AttackType != AttackType.Magic) return;

            var restore = Params.MpRestore * Time.deltaTime;

            if (State.Mp + restore <= Params.MpMax)
            {
                Mp += restore;
            }
            else
            {
                Mp = Params.MpMax;
            }
        }

        private void Fall()
        {
            _direction.x = 0;
            _direction.y -= Gravity * Time.deltaTime;
            _direction.z = 0;

            Controller.Move(_direction * Time.deltaTime);
        }

        private static bool IsPlaying(Animator animator, string clip)
        {
            return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(clip);
        }
    }
}