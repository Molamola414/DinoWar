using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base creature behaviour, controlled by AI or by player
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class Creature : MonoBehaviour
{
    public enum CreatureType
    {
        Generic,

        MeleeFighter = 100,
        MeleeBerserker,
        MeleeTank,
        MeleeBomber,

        ArcherShortRange = 200,
        ArcherLongRange,
        ArcherSniper,

        SpecialBomber = 300,
        SpecialNinjaJonin,
        SpecialNinjaGenin,

        SupportWitch = 400 

    }

    public enum Posture
    {
        None = 0,
        Yes = 1,
        No,
        Sick,
        Happy,
        Hello,
        Stun,
        Eat,
        Cute
    }

    public CreatureType creatureType = CreatureType.Generic;

    public int team = 0;
    public float hpMax;
    public float speed;
    public float jump;   
    public float currentHp;

    public Transform body;
    public Transform weaponAttachPoint;

    public List<Weapon> weaponList = new List<Weapon>();
    public int activeWeaponIndex = 0;

    public Controls controls = new Controls();

    public CharacterController charController;

    [HideInInspector]
    public Animator animator;
    private GameObject hitVFXPrefab;

    private Vector3 _direction = Vector3.zero;
    private const int Gravity = 120;//320;

    private float impactMass = 3;
    private Vector3 impactForce = Vector3.zero;


    public virtual void Awake() {
        animator = GetComponent<Animator>();
        hitVFXPrefab = Resources.Load<GameObject>("Prefabs/VFX/Hit");
    }

    public virtual void Start()
    {
    }

    public virtual void OnDestroy()
    {
        BattleManager.Instance.UnregisterActiveCreature(this);        
    }

    public virtual void OnEnable()
    {
        this.charController.enabled = true;
        ResetCreature();
        BattleManager.Instance.RegisterActiveCreature(this);
        
    }

    public virtual void OnDisable() {
        BasicVFX[] vfx = this.GetComponentsInChildren<BasicVFX>();
        for(int i=0; i<vfx.Length; i++) {
            Destroy(vfx[i].gameObject);
        }
        BattleManager.Instance.UnregisterActiveCreature(this);        
    }

    /// <summary>
    /// Move, jump, attack and other actions
    /// </summary>
    public void Update()
    {
        if (currentHp > 0)
        {
            // Controls.Normalize();
            Attack();

            if(impactForce != Vector3.zero) {
                if(impactForce.magnitude > 0.2f) {
                    // this.controls.Direction = Vector2.zero;
                    Vector3 dir = impactForce * Time.deltaTime;
                    dir.y -= Gravity * Time.deltaTime;
                    charController.Move(dir);
                }
                if(impactForce.magnitude < 1f) {
                    Move();
                }
                impactForce = Vector3.Lerp(impactForce, Vector3.zero, 5*Time.deltaTime);
            }
            else {
                Move();
            }
        }
        else
        {
            Fall();
        }
    }

    public void ResetCreature() {
        currentHp = hpMax;

        impactForce = Vector3.zero;

        controls.Direction = Vector2.zero;
        controls.AttackDirection = Vector2.zero;

        animator.SetInteger("WalkSpeed", 0);
        animator.SetInteger("Posture", 0);
        animator.SetInteger("AttackStatus", (int)Weapon.AttackType.None);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsDead", false);

        animator.Play("Idle");

        for(int i=0; i<weaponList.Count; i++) {
            weaponList[i].ResetWeapon();
        }
    }

    public Weapon SwitchActiveWeapon() {
        int nextIdx = (activeWeaponIndex+1)%weaponList.Count;
        return SetActiveWeapon(nextIdx);
    }

    public Weapon SetActiveWeapon(int idx) {
        weaponList[activeWeaponIndex].gameObject.SetActive(false);

        activeWeaponIndex = idx;
        weaponList[activeWeaponIndex].gameObject.SetActive(true);

        return weaponList[activeWeaponIndex];
    }

    public Weapon GetActiveWeapon() {
        if(activeWeaponIndex < weaponList.Count) {
            return weaponList[activeWeaponIndex];
        }
        return null;
    }

    /// <summary>
    /// Receive damage from other creature
    /// </summary>
    /// <param name="damage">Damage value</param>
    public virtual void GetDamage(float damage, BulletShell.BulletHitEffectType hitType = BulletShell.BulletHitEffectType.Default)
    {
        // TO-FIX: Multiple damage may cause damage & die messing together
        currentHp -= damage;

        switch(hitType) {
            case BulletShell.BulletHitEffectType.Default:
            {
                GameObject hitVFX = ObjectPoolManager.CreatePooled(hitVFXPrefab, this.transform);
                Vector3 randomVec = Random.insideUnitSphere;
                hitVFX.transform.position = new Vector3(this.transform.position.x + randomVec.x * 3f, this.transform.position.y + 5f + randomVec.y * 2f, this.transform.position.z + randomVec.z * 3f);
            }
            break;
        }

        if (currentHp <= 0)
        {
            animator.SetBool("IsDead", true);
            animator.SetTrigger("DieOnBack");
            AudioPlayer.PlayEffect("Death");
        }
        else
        {
            animator.SetTrigger("Damage");
        }
    }

    /// <summary>
    /// Turn creature to desired direction
    /// </summary>
    public virtual void Turn(Vector2 direction)
    {
        Vector3 relativePos = this.transform.position + new Vector3(direction.x, 0, direction.y);
        this.transform.LookAt(relativePos);
    }

    /// <summary>
    /// Called from animation when hit
    /// </summary>
    public virtual void OnHit()
    {
        /* Not triggered often, to investigate */
        Debug.Log("Creature OnHit: " + gameObject.name);

        AudioPlayer.PlayEffect("Hit");
    }

    /// <summary>
    /// Called from animation when shot
    /// </summary>
    public virtual void OnAttackStart(AnimationEvent animEvent)
    {
        if (controls.AttackDirection == Vector2.zero) return;

        if(activeWeaponIndex < weaponList.Count) {
            weaponList[activeWeaponIndex].TriggerWeapon(new Vector3(controls.AttackDirection.x, 0, controls.AttackDirection.y), animEvent.intParameter);
        }
    }

    public virtual void OnAttackEnd(AnimationEvent animEvent)
    {
        if(activeWeaponIndex < weaponList.Count) {
            weaponList[activeWeaponIndex].TriggerWeaponEnd(animEvent.intParameter);
        }
    }

    /// <summary>
    /// Called from animation when die
    /// </summary>
    public virtual void OnDie()
    {
        StartCoroutine(WaitAndRecycle());
        // Destroy(gameObject);
        // gameObject.AddComponent<DeadBlink>();

        // This is the creature controlled by player
        PlayerControllable pc = gameObject.GetComponent<PlayerControllable>();
        if(pc != null && pc.isActiveAndEnabled) {
            BattleManager.Instance.PlayerCreatureDie(this);
        }
    }

    public IEnumerator WaitAndRecycle() {
        yield return new WaitForSeconds(0.5f);
        ObjectPoolManager.DestroyPooled(gameObject);
    }

    protected virtual void Attack()
    {

        if (controls.AttackDirection == Vector2.zero)
        {
            animator.SetInteger("AttackStatus", (int)Weapon.AttackType.None);
            return;
        }
        
        Turn(controls.AttackDirection);

        if (charController.isGrounded)
        {
            if(activeWeaponIndex < weaponList.Count && weaponList[activeWeaponIndex].canTrigger) {
                animator.SetInteger("AttackStatus", (int)weaponList[activeWeaponIndex].attackType);
            }else {
                animator.SetInteger("AttackStatus", (int)Weapon.AttackType.None);
            }
        }
    }

    protected virtual void Move()
    {
        if (charController.isGrounded)
        {
            _direction = speed * (new Vector3(controls.Direction.x, 0, controls.Direction.y));
            // Attack affects walking speeed
            if(animator.GetBool("IsAttacking") && activeWeaponIndex < weaponList.Count) {
                _direction *= weaponList[activeWeaponIndex].walkSpeedRatioInAttack;
            }

            if (controls.Jump)
            {
                _direction.y = jump;
            }

            if (controls.Direction.magnitude > 0)
            {
                if (controls.AttackDirection == Vector2.zero)
                {
                    Turn(controls.Direction);
                }

                animator.SetInteger("WalkSpeed", (int)(controls.Direction.magnitude * 10));
            }
            else
            {
                animator.SetInteger("WalkSpeed", 0);
            }
        }
        else if (jump > 0 && charController.velocity.magnitude > 0.01)
        {
            // No jumping for now
            // animator.Play(AnimationParams.Jump);
        }

        _direction.y -= Gravity * Time.deltaTime;
        charController.Move(_direction * Time.deltaTime);
    }

    private void Fall()
    {
        _direction.x = 0;
        _direction.y -= Gravity * Time.deltaTime;
        _direction.z = 0;

        charController.Move(_direction * Time.deltaTime);
    }

    public void SetCrouching(bool isCrouching) {
        animator.SetBool("IsCrouch", true);
    }

    public void SetPosture(Creature.Posture posture) {
        animator.SetInteger("Posture", (int)posture);
    }

    public Creature.Posture GetPosture() {
        int pInt = animator.GetInteger("Posture");
        return (Creature.Posture)pInt;
    }

    private static bool IsPlaying(Animator animator, string clip)
    {
        return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(clip);
    }

    public virtual void AddImpact(Vector3 direction, float force) {
        direction.Normalize();
        if(direction.y < 0)
            direction.y = -direction.y;

        impactForce += direction.normalized * force / impactMass;
    }
}
