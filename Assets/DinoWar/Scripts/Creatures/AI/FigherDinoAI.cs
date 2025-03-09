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

    private int _shootingTestLayerMask;
    private int _dodgeImpactForce = 150;

    private float _attackDistance;
    // Creature out of this range doesn't use path finding to save resources
    protected float _startPathFindingDistance = 100f;
    
    public override void Awake()
    {
        base.Awake();

        _idleTime = Time.time + actionTime;
        _attackTime = Time.time;

        // SphereCollider sc = _creature.GetActiveWeapon().detectingCollider;
        // _attackDistance = sc.transform.lossyScale.x * sc.radius;

        _shootingTestLayerMask = LayerMask.GetMask("Environment", "Obstacle");
    }
    
    public void Update()
    {
        _creature.controls.Direction = Vector2.zero;
        _creature.controls.AttackDirection = Vector2.zero;

        if (_creature.currentHp <= 0)
        {
            return;
        }

        if (_target == null || _target.currentHp < 0 || Time.time - _findTargetTime > 1)
        {
            FindTarget();
        }

        if (_target == null)
        {
            return;
        }

        if (Time.time > _idleTime)
        {
            if (Time.time < _idleTime + idleTime)
            {
                return;
            }
            
            _idleTime = Time.time + actionTime;
        }

        var direction = _target.transform.position - _creature.transform.position;

        if(direction.magnitude < _startPathFindingDistance) {
            GeneratePath(_target.transform.position);
        }else {
            path = null;
        }

        Weapon w = _creature.GetActiveWeapon();
        if(w == null) {
            return;
        }

        
        if(w.attackType == Weapon.AttackType.Melee) {
            var canAttack = w.targets.Contains(_target) && (Time.time - _attackTime > attackTimeout);

            MeleeMove(direction, canAttack);
        }
        else if(w.attackType == Weapon.AttackType.Ranged) {
            var canAttack = w.targets.Contains(_target) && (Time.time - _attackTime > attackTimeout);

            bool canSeeEnemy = true;
            // Test shooting by a raycast, if this ray hit environment / obstacle before creature. We can say the target is behind these object.
            RaycastHit hit;
            if(Physics.SphereCast(((RangedWeapon)w).muzzleTransform.position, 1, direction, out hit, 1000, _shootingTestLayerMask)) {
                if(direction.magnitude > hit.distance) {
                    canSeeEnemy = false;
                }
            }
            RangedMove(direction, (canAttack && canSeeEnemy), canSeeEnemy);
        }
    }


    /// <summary>
    /// Called from animation when hit
    /// </summary>
    public virtual void OnHit()
    {
        /* Not triggered often, to investigate */
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

    private void FindTarget()
    {
        // Grab closest creature
        var enemies = BattleManager.Instance.creatureList.Where(i => i.currentHp > 0 && i.team != _creature.team).ToList();

        _target = enemies.OrderBy(i => Vector2.Distance(i.transform.position, _creature.transform.position)).FirstOrDefault();
        _findTargetTime = Time.time;
    }

    private void FindNearbyTankFellow()
    {
        var tanks = BattleManager.Instance.creatureList.Where(i =>i.currentHp > 0 && i.team == _creature.team && i.creatureType == Creature.CreatureType.MeleeTank).ToList();

        _tankFellow = tanks.OrderBy(i => Vector2.Distance(i.transform.position, _creature.transform.position)).FirstOrDefault();
        _findTankFellowTime = Time.time;
    }

    private void FindThreatBullet()
    {
        // Grab closest bullet
        var bullets = bulletDetector.GetBullets().Where(i => i.team != _creature.team).ToList();

        BulletShell closestBullet = bullets.OrderBy(i => Vector2.Distance(i.transform.position, _creature.transform.position)).FirstOrDefault();
        _avoidBulletTime = Time.time;

        if(closestBullet != null) {
            _avoidingBullet = closestBullet;
        }
        else {
            _avoidingBullet = null;
        }
    }

    protected virtual void MeleeMove(Vector3 direction, bool canAttack)
    {
        if (canAttack)
        {
            _creature.controls.AttackDirection = (new Vector2(direction.x, direction.z)).normalized;    

            if(canMoveInAttack) {
                _creature.controls.Direction = new Vector2(direction.x, direction.z).normalized * walkSpeedRatio;
            }
            else {
                _creature.controls.Direction = Vector2.zero;
            }
        }
        else
        {
            DodgeAndMove(direction, canAttack);
        }
    }

    protected virtual void RangedMove(Vector3 direction, bool canAttack, bool canSeeEnemy)
    {
        if (canAttack)
        {
            bool avoidBulletNow = false;

            if(canDodgeInAttack) {
                if(bulletDetector != null && (Time.time - _avoidBulletTime > 0.5f)) {
                    FindThreatBullet();
                }
                if(_avoidingBullet != null) {
                    avoidBulletNow = true;
                    // Bullet must be moving
                    Vector3 bulletDir = _avoidingBullet.direction;
                    bulletDir.y = 0;
                    if(bulletDir != Vector3.zero) {
                        // Create a ray to estimate if creature is going to get hit
                        RaycastHit hit;
                        if(Physics.SphereCast(_avoidingBullet.transform.position, 5, bulletDir, out hit, 50, LayerMask.GetMask("Creatures"))) {
                            Creature hitTarget = hit.collider.GetComponent<Creature>();
                            if(hitTarget != null && hitTarget == _creature) {
                                // Dodge in direction perpendicular to bullet direction
                                Vector3 rotatedVec = Quaternion.AngleAxis(Random.Range(0, 2)==0?90:-90, Vector3.up) * bulletDir;
                                // Dodge by adding impact looks cooler than run sideway; impact force to be adjusted, better to be dynamic
                                _creature.AddImpact(rotatedVec, _dodgeImpactForce);
                            }
                        }
                    }

                    _avoidingBullet = null;
                }
            }

            if(!avoidBulletNow) {
                SphereCollider sc = _creature.GetActiveWeapon().detectingCollider;
                _attackDistance = sc.transform.lossyScale.x * sc.radius;

                _creature.controls.AttackDirection = (new Vector2(direction.x, direction.z)).normalized;

                if(canMoveInAttack && direction.magnitude < (_attackDistance * 0.5f)) {
                    _creature.controls.Direction = new Vector2(-direction.x, -direction.z).normalized * walkSpeedRatio;
                }else {
                    _creature.controls.Direction = Vector2.zero;
                }
            }
        }
        else
        {
            // Look for nearby tank
            if(Time.time - _findTankFellowTime > 5) {
                FindNearbyTankFellow();
            }

            if(_tankFellow != null) {
                // Walk behind tank to take cover
                var tankDir = (_tankFellow.transform.position - (_tankFellow.transform.forward * 20)) - _creature.transform.position;
                float tankDistance = tankDir.magnitude;

                // Follow tank when target is further than tank
                if( tankDistance < direction.magnitude) {
                    if(tankDistance > 12) {
                        // Go to tank's back
                        _creature.controls.Direction = new Vector2(tankDir.x, tankDir.z).normalized * walkSpeedRatio;
                    }
                    else {
                        // Walk slowly to follow tank
                        _creature.controls.Direction = _tankFellow.controls.Direction * 0.3f; //_tankFellow.GetComponent<TankDinoAi>().GetTankWalkingSpeed()/(_creature.speed * walkSpeedRatio);
                    }
                }
                else {
                    // Forget about tank's cover if close enough to enemy
                    _tankFellow = null;
                    DodgeAndMove(direction, canAttack);
                }
            }
            else {
                if(!canSeeEnemy) {
                    // If creature cannot see enemy, keep findng
                    DodgeAndMove(direction, canAttack);
                }
                else {
                    SphereCollider sc = _creature.GetActiveWeapon().detectingCollider;
                    _attackDistance = sc.transform.lossyScale.x * sc.radius;

                    // Keep enemy inside attack distance, otherwise creature would keep shaking back and forth with enemy on it attack distance edge
                    float enemyDist = direction.magnitude;
                    if(enemyDist > _attackDistance * 0.8f) {
                        DodgeAndMove(direction, canAttack);
                    }
                    else if(enemyDist < (_attackDistance * 0.5f)) {
                        // Dont get too close to enemy
                        RunAwayFromEnemy(-direction);
                    } 
                }
            }
        }
    }

    private void DodgeAndMove(Vector3 direction, bool canAttack) {
        if(movingDodgeType != DinoAI.BulletDodgeType.None && bulletDetector != null && (Time.time - _avoidBulletTime > 0.5f)) {
            FindThreatBullet();
        }
        else {
            _avoidingBullet = null;
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
                    // Bullet must be moving
                    Vector3 bulletDir = _avoidingBullet.direction;
                    bulletDir.y = 0;
                    if(bulletDir != Vector3.zero) {
                        // Create a ray to estimate if creature is going to get hit
                        RaycastHit hit;
                        if(Physics.SphereCast(_avoidingBullet.transform.position, 5, bulletDir, out hit, 50, LayerMask.GetMask("Creatures"))) {
                            Creature hitTarget = hit.collider.GetComponent<Creature>();
                            if(hitTarget != null && hitTarget == _creature) {
                                // Dodge in direction perpendicular to bullet direction
                                Vector3 rotatedVec = Quaternion.AngleAxis(Random.Range(0, 2)==0?90:-90, Vector3.up) * bulletDir;
                                // Dodge by adding impact looks cooler than run sideway; impact force to be adjusted, better to be dynamic
                                _creature.AddImpact(rotatedVec, _dodgeImpactForce);
                            }
                        }
                    }
                    // Not sure if we need this line... not to dodge the same bullet again
                    // bulletDetector.bullets.Remove(_avoidingBullet);
                    _avoidingBullet = null; // Dodge once only

                    _creature.controls.Direction = new Vector2(direction.x, direction.z).normalized * walkSpeedRatio;
                }
                break;
            }
        }
        else {
            if(path != null) {
                PathFindingMove();
            }else {
                _creature.controls.Direction = new Vector2(direction.x, direction.z).normalized * walkSpeedRatio;
            }
        }
    }

    protected virtual void RunAwayFromEnemy(Vector3 runDirection) {
        _creature.controls.Direction = new Vector2(runDirection.x, runDirection.z).normalized * walkSpeedRatio;
    }
}
