using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    public enum BulletHitEffectType
    {
        None = -1,
        Default = 0,
        Fire,
        Glue,
    }

    [HideInInspector]
    public Vector3 direction;

    public Collider shellCollider; 

    public BulletHitEffectType hitEffectType = BulletHitEffectType.Default;

    [HideInInspector]
    public Rigidbody bulletRB;
    public int team;

    public int damage;
    public int speed;

    public float impactForce = 0;

    // Take negative value as infinite count
    public int hitCountLimit;
    protected int hitCountLeft;

    // Take negative value as infinite lifetime
    public float lifeTime;
    protected float bulletLifeDuration;
    

    public int buff_damage; 
    public int buff_speed; 
    public int buff_hitCountLimit;
    public float buff_lifeTime;

    private GameObject hitWallVFXPrefab;

    public delegate void OnBulletTriggerStatus(Collider collider); 
    public OnBulletTriggerStatus onBulletTriggerStatus; 


    public delegate void OnBulletDestoryStatus(); 
    public OnBulletDestoryStatus onBulletDestoryStatus; 
    protected Vector2 ownerWalkDirection;

    // Initialize is called earlier than Start, but later than Awake
    public virtual void Initialize(Vector3 dir, Creature owner) {
        this.Initialize(dir, owner.team);
    }

    public virtual void Initialize(Vector3 dir, int teamID)
    {
        team = teamID;
        direction = dir;
        
        if(bulletRB != null && !bulletRB.isKinematic && speed != 0) {
            bulletRB.velocity = new Vector3(direction.x, direction.y, direction.z) * speed;
        }

    }

    protected virtual void move() {
        if(bulletRB != null && bulletRB.isKinematic && speed != 0) {
            gameObject.transform.position += direction * speed * Time.deltaTime;
        }
    }

    // public virtual void modeling(){
    //     if(bulletRB != null && !bulletRB.isKinematic && speed != 0) {
    //         bulletRB.velocity = new Vector3(direction.x, 0, direction.z) * speed;
    //     }
    // }

    public virtual void OnEnable() {
        bulletLifeDuration = 0;
        hitCountLeft = hitCountLimit + buff_hitCountLimit;
        if(bulletRB != null) {
            bulletRB.velocity = Vector3.zero;
        }        
    }

    public void resetBuffedValue(){
        buff_damage = 0;
        buff_speed = 0;
        buff_hitCountLimit  = 0;
        buff_lifeTime = 0;

    }

    public virtual void Awake()
    {
        bulletRB = GetComponent<Rigidbody>();

        hitWallVFXPrefab = Resources.Load<GameObject>("Prefabs/VFX/SmokeHit");
    }
    // Start is called before the first frame update
    public virtual void Start()
    {        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        move();
        
        if(lifeTime >= 0) {
            if(bulletLifeDuration >= lifeTime + buff_lifeTime) {
                ActionBeforeBulletDestory();
                destoryBullet();
            }
            else {
                bulletLifeDuration += Time.deltaTime;
            }
        }
    }

    public int getBulletDamage(){
        return damage + buff_damage;
    }

    private void OnDestroy() {
    }

    public void destoryBullet(){

        if( onBulletDestoryStatus != null){
            onBulletDestoryStatus();
        }
        ObjectPoolManager.DestroyPooled(gameObject);
    }

    protected virtual void InteractToCreature(Collider other) {
        Creature c = other.gameObject.GetComponent<Creature>();
        if(c != null && c.team != this.team && c.currentHp > 0) {
            c.GetDamage( getBulletDamage(), hitEffectType);
            if(impactForce != 0) {
                c.AddImpact((c.transform.position - transform.position), impactForce);
            }             
            hitCountLeft--;

            if( onBulletTriggerStatus != null){
                onBulletTriggerStatus(other);
            }
        }
    }

    protected virtual void InteractToObstacle(Collider other) {
        // Some logic to damage obstacle perhaps
        hitCountLeft--;
        Destructable d = other.gameObject.GetComponent<Destructable>();
        if(d != null) {
            d.GetDamage(getBulletDamage());
        }

        if( onBulletTriggerStatus != null){
            onBulletTriggerStatus(other);
        }
    }

    protected virtual void InteractToEnvironment(Collider other) {
        hitCountLeft--;

        if( onBulletTriggerStatus != null){
            onBulletTriggerStatus(other);
        }
    }

    protected virtual void ActionBeforeBulletDestory() {
        GameObject hitVFX = ObjectPoolManager.CreatePooled(hitWallVFXPrefab, BattleManager.Instance.projectileContainer);
        hitVFX.transform.position = this.transform.position;
    }

    public virtual void OnTriggerEnter(Collider other ) {

        int colliderLayer = other.gameObject.layer;
        if(colliderLayer == GameConstants.LayerCreature) {
            InteractToCreature(other);
        }
        else if(colliderLayer == GameConstants.LayerEnvironment) {
            InteractToEnvironment(other);
        }
        else if(colliderLayer == GameConstants.LayerObstacle) {
            InteractToObstacle(other);
        }

        if( hitCountLeft == 0){            
            ActionBeforeBulletDestory();

            destoryBullet();
        } 
    }
}
