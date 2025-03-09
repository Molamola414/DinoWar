using System.Linq;
using Assets.MicroWar.Scripts.Data;
using Assets.MicroWar.Scripts.Enums;
using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// AI behaviuor
    /// </summary>
    [RequireComponent(typeof(Creature))]
    public class AI : MonoBehaviour
    {
        /// <summary>
        /// AI params
        /// </summary>
        public AIParams Params;
        private int KeepDistance;
        private Creature _creature;
        private Creature _target;
        private bool _jump;
        private bool _bound;
        private bool _retreat;
        private float _attackDistance;
        private float _idleTime;
        private float _attackTime;
        private float _findTargetTime;
        
        public void Awake()
        {
            _creature = GetComponent<Creature>();
            _creature.State.AI = enabled;
            _idleTime = Time.time + Params.ActionTime;
            _attackTime = Time.time;
            _attackDistance = _creature.GetComponentInChildren<WeaponBox>().Collider.size.x - 4;
        }
        
        public void Update()
        {
            _creature.StatusBars.Enable(_creature.State.Hp > 0 && _creature.State.Hp < _creature.Params.HpMax, false);
            _creature.Controls.Direction = Vector2.zero;
            _creature.Controls.AttackDirection = Vector2.zero;
            // _creature.Controls.Attack = false;

            if (_creature.State.Hp <= 0)
            {
                return;
            }

            if (_target == null || _target.State.Hp <= 0 || Time.time - _findTargetTime > 1)
            {
                FindTarget();
            }

            if (_target == null)
            {
                return;
            }

            if (Time.time > _idleTime)
            {
                if (Time.time < _idleTime + Params.IdleTime)
                {
                    return;
                }
                
                _idleTime = Time.time + Params.ActionTime;
            }

            var direction = _target.transform.position - _creature.transform.position;
            var canAttack = _creature.WeaponBox.Targets.Contains(_target) && !_retreat;

            _creature.Turn(direction.x);
            if(canAttack && (Time.time - _attackTime > Params.AttackTimeout)) 
            {
                _creature.Controls.AttackDirection = (new Vector2(direction.x, direction.z)).normalized;    
            }
            // _creature.Controls.Attack = canAttack && (Time.time - _attackTime > Params.AttackTimeout);
            
            if (_creature.Params.AttackType == AttackType.Melee)
            {
                MeleeMove(direction, canAttack);
            }
            else
            {
                RangedMove(direction, canAttack);
            }
        }


        /// <summary>
        /// Called from animation when hit
        /// </summary>
        public void OnHit()
        {
            _attackTime = Time.time;
        }

        /// <summary>
        /// Called from animation when shot
        /// </summary>
        public void OnShot()
        {
            _attackTime = Time.time;
        }

        /// <summary>
        /// Called from animation when cast
        /// </summary>
        public void OnCast()
        {
            _attackTime = Time.time;
        }

        /// <summary>
        /// Defines a behaviour when creature hits environment collider (jump place or world bounds)
        /// </summary>
        /// <param name="target">Hited collider</param>
        public void OnTriggerEnter(Collider target)
        {
            if (!enabled) return;

            if (target.tag == "Jump")
            {
                _jump = true;
            }
            else if (target.tag == "Bound")
            {
                _bound = true;
            }
        }

        /// <summary>
        /// Defines a behaviour when creature hits environment collider (jump place or world bounds)
        /// </summary>
        /// <param name="target">Hited collider</param>
        public void OnTriggerExit(Collider target)
        {
            if (!enabled) return;

            if (target.tag == "Jump")
            {
                _jump = _creature.Controls.Jump = false;
            }
            else if (target.tag == "Bound")
            {
                _bound = false;
            }
        }

        /// <summary>
        /// Defines a behaviour when creature hits other creature
        /// </summary>
        /// <param name="target">Hited creature collider</param>
        public void OnControllerColliderHit(ControllerColliderHit target)
        {
            if (!enabled) return;

            if (target.transform.tag == "Jump" && _jump)
            {
                var direction = new Vector3(_creature.Controls.Direction.x, 0, _creature.Controls.Direction.y);
                var hits = Physics.RaycastAll(_creature.transform.position, direction, 999, LayerMask.GetMask("Environment"));

                if (hits.Select(i => i.transform).Contains(target.transform))
                {
                    _creature.Controls.Jump = true;
                }
            }
        }

        private void FindTarget()
        {
            var enemies = Engine.Instance.Creatures.Where(i => i.State.Hp > 0 && i.State.Team != _creature.State.Team).ToList();

            _target = enemies.OrderBy(i => Vector2.Distance(i.transform.position, _creature.transform.position)).FirstOrDefault();
            _findTargetTime = Time.time;
        }

        private void MeleeMove(Vector3 direction, bool canAttack)
        {
            if (canAttack)
            {
                _creature.Controls.Direction = Vector2.zero;
            }
            else if (Mathf.Abs(direction.x) > 32)
            {
                _creature.Controls.Direction = new Vector2(direction.x, 0);
            }
            else
            {
                if (Mathf.Abs(direction.x) > _attackDistance)
                {
                    var offset = Mathf.Sign(direction.x) * _attackDistance;

                    _creature.Controls.Direction.x = direction.x - offset;
                }
                else
                {
                    _creature.Turn(direction.x);
                }

                _creature.Controls.Direction.y = direction.z;
            }
        }

        private void RangedMove(Vector3 direction, bool canAttack)
        {
            _retreat = direction.magnitude < KeepDistance && !_bound && (_creature.Controls.AttackDirection == Vector2.zero);
            // _retreat = Mathf.Abs(direction.x) < Params.KeepDistance && !_bound && !_creature.Controls.Attack;

            if (_retreat)
            {
                _creature.Controls.Direction = -new Vector2(direction.x, direction.z);
            }
            else if (Mathf.Abs(direction.x) > _attackDistance)
            {
                _creature.Controls.Direction = new Vector2(direction.x, 0);
            }
            else
            {
                _creature.Controls.Direction = new Vector2(0, canAttack ? 0 : direction.z);
            }
        }
    }
}