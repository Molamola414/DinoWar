using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Common enemy dino AI
public class FigherDinoAI : DinoAI
{
    // How long AI performing action
    public float actionTime = int.MaxValue;
    // How long AI taking rest when action time reached
    public float idleTime = 0;
    // How long for next attack
    public float attackTimeout = 0;
    // Keep moving when attack
    public bool canMoveInAttack;
    // Can dodge during attack. Useless for melee type
    public bool canDodgeInAttack;
    // How to dodge bullet when approaching enemy
    public BulletDodgeType movingDodgeType;

    private float _idleTime;
    private float _attackTime;
    private float _findTargetTime;
    private float _findTankFellowTime;
    private Creature _tankFellow;
    private BulletShell _avoidingBullet;
    // private Vector3 _avoidPos;
    private float _avoidBulletTime;

    private int _shootingLayerMask;
    private int _dodgeImpactForce = 150;

    private float _attackDistance;
    // Creature out of this range doesn't use path finding to save resources
    protected float _pathfindingThreshold = 100f;
    
    public override void Awake()
    {
        base.Awake();

        _idleTime = Time.time + actionTime;
        _attackTime = Time.time;

        // SphereCollider sc = _creature.GetActiveWeapon().detectingCollider;
        // _attackDistance = sc.transform.lossyScale.x * sc.radius;
        _shootingLayerMask = LayerMask.GetMask("Environment", "Obstacle");
    }
    
    public void Update()
    {
        if (_creature.currentHp <= 0) {
            return;
        }

        _creature.controls.Direction = Vector2.zero;
        _creature.controls.AttackDirection = Vector2.zero;

        if (_target == null || _target.currentHp < 0 || Time.time - _findTargetTime > 1) {            
            _target = FindTarget();
            _findTargetTime = Time.time;
        }

        if (_target == null) {
            return;
        }

        // Handle idle time before resuming actions
        if (Time.time > _idleTime) {
            if (Time.time < _idleTime + idleTime) {
                return;
            }            
            _idleTime = Time.time + actionTime;
        }

        var direction = _target.transform.position - _creature.transform.position;

        // Pathfinding only within threshold distance
        if(direction.magnitude < _pathfindingThreshold) {
            GeneratePath(_target.transform.position);
        } else {
            path = null;
        }

        Weapon w = _creature.GetActiveWeapon();
        if(w == null) {
            return;
        }

        bool canAttack = w.targets.Contains(_target) && (Time.time - _attackTime > attackTimeout);
        
        switch (w.attackType) {
            case Weapon.AttackType.Melee:
                HandleMeleeCombat(direction, canAttack);
            break;

            case Weapon.AttackType.Ranged:
                HandleRangedCombat(direction, canAttack);
            break;
        }
    }


    /// <summary>
    /// Called from animation when hit
    /// </summary>
    public virtual void OnHit()
    {
        /* Not often triggered, to be investigated */
        // Debug.Log("AI Dino OnHit: " + gameObject.name);
        _attackTime = Time.time;
    }

    public virtual void OnAttackStart(AnimationEvent animEvent)
    {
        _attackTime = Time.time;
        // Debug.Log("AI OnAttackStart: " + gameObject.name);
    }

    public virtual void OnAttackEnd(AnimationEvent animEvent)
    {
    }

    private Creature FindTarget()
    {
        // Grab closest creature
        return BattleManager.Instance.creatureList
                            .Where(i => i.currentHp > 0 && i.team != _creature.team)
                            .OrderBy(i => Vector2.Distance(i.transform.position, _creature.transform.position))
                            .FirstOrDefault();
    }

    private Creature FindTankFellow()
    {
        return BattleManager.Instance.creatureList
                            .Where(i =>i.currentHp > 0 && i.team == _creature.team && i.creatureType == Creature.CreatureType.MeleeTank)
                            .OrderBy(i => Vector2.Distance(i.transform.position, _creature.transform.position))
                            .FirstOrDefault();
    }

    private BulletShell FindThreatBullet()
    {
        return bulletDetector?.GetBullets()
            .Where(i => i.team != _creature.team)
            .OrderBy(i => Vector2.Distance(i.transform.position, _creature.transform.position))
            .FirstOrDefault();
    }

    protected virtual void HandleMeleeCombat(Vector3 direction, bool canAttack)
    {
        if (canAttack) {
            _creature.controls.AttackDirection = (new Vector2(direction.x, direction.z)).normalized;    
            _creature.controls.Direction = canMoveInAttack ? new Vector2(direction.x, direction.z).normalized * walkSpeedRatio : Vector2.zero;
        } else {
            DodgeAndMove(direction);
        }
    }

    protected virtual void HandleRangedCombat(Vector3 direction, bool canAttack)
    {
        // Test shooting by a raycast, if this ray hit environment / obstacle before creature. We can say the target is behind these object.
        bool canSeeEnemy = !Physics.SphereCast(((RangedWeapon)_creature.GetActiveWeapon()).muzzleTransform.position, 1, direction, out var hit, 1000, _shootingLayerMask) 
                            || direction.magnitude <= hit.distance;

        if (canAttack && canSeeEnemy) {
            if(canDodgeInAttack && (Time.time - _avoidBulletTime > 0.5f)) {
                _avoidingBullet = FindThreatBullet();
                _avoidBulletTime = Time.time;
            }

            if(_avoidingBullet != null)
                DodgeBullet(_avoidingBullet);

            _attackDistance = _creature.GetActiveWeapon().detectingCollider.transform.lossyScale.x *
                              _creature.GetActiveWeapon().detectingCollider.radius;

            _creature.controls.AttackDirection = new Vector2(direction.x, direction.z).normalized;

            _creature.controls.Direction = canMoveInAttack && direction.magnitude < (_attackDistance * 0.5f)
                ? -_creature.controls.AttackDirection * walkSpeedRatio
                : Vector2.zero;
        } else {
            // Look for nearby tank
            if(Time.time - _findTankFellowTime > 5) {
                _tankFellow = FindTankFellow();
                _findTankFellowTime = Time.time;
            }

            if(_tankFellow != null && ShouldFollowTank(direction)) {
                MoveBehindTank();
            } else {
                // _tankFellow = null;
                if (!canSeeEnemy)
                    DodgeAndMove(direction);
                else if (direction.magnitude > _attackDistance * 0.8f)
                    DodgeAndMove(direction);
                else if (direction.magnitude < (_attackDistance * 0.5f))
                    RunAwayFromEnemy(-direction);
            }                
        }
    }

    private bool ShouldFollowTank(Vector3 enemyDirection)
    {
        var tankDirection = (_tankFellow.transform.position - (_tankFellow.transform.forward * 20)) - _creature.transform.position;
        return tankDirection.magnitude < enemyDirection.magnitude;
    }

    private void MoveBehindTank()
    {
        var tankDirection = (_tankFellow.transform.position - (_tankFellow.transform.forward * 20)) - _creature.transform.position;

        _creature.controls.Direction = tankDirection.magnitude > 12
            ? new Vector2(tankDirection.x, tankDirection.z).normalized * walkSpeedRatio
            : _tankFellow.controls.Direction * 0.3f;
    }

    private void DodgeAndMove(Vector3 direction) {
        if(movingDodgeType != DinoAI.BulletDodgeType.None && (Time.time - _avoidBulletTime > 0.5f)) {
            _avoidingBullet = FindThreatBullet();
            _avoidBulletTime = Time.time;
        }

        if(_avoidingBullet != null) {
            switch(movingDodgeType) {
                case DinoAI.BulletDodgeType.RunAway:
                {
                    // Run away in opposite direction to the bullet
                    Vector3 avoidDir = _creature.transform.position - _avoidingBullet.transform.position;
                    _creature.controls.Direction = new Vector2(avoidDir.x, avoidDir.z).normalized * walkSpeedRatio;
                }
                break;

                case DinoAI.BulletDodgeType.Dodge:
                {
                    DodgeBullet(_avoidingBullet);
                }
                break;
            }
        } else if(path != null) {
            PathFindingMove();
        } else {
            _creature.controls.Direction = new Vector2(direction.x, direction.z).normalized * walkSpeedRatio;
        }
    }

    private void DodgeBullet(BulletShell bullet)
    {
        var bulletDir = bullet.direction;
        bulletDir.y = 0;

        if (bulletDir != Vector3.zero) {
            if (Physics.SphereCast(bullet.transform.position, 5, bulletDir, out var hit, 50, LayerMask.GetMask("Creatures"))) {
                if (hit.collider.GetComponent<Creature>() == _creature) {
                    Vector3 dodgeDir = Quaternion.AngleAxis(Random.Range(0, 2) == 0 ? 90 : -90, Vector3.up) * bulletDir;
                    _creature.AddImpact(dodgeDir, _dodgeImpactForce);
                }
            }
        }

        _avoidingBullet = null;
    }

    protected virtual void RunAwayFromEnemy(Vector3 runDirection) {
        _creature.controls.Direction = new Vector2(runDirection.x, runDirection.z).normalized * walkSpeedRatio;
    }
}
