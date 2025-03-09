using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bomb is a bullet which can be placed on floor. It can either wait certain time or touch to explode.
// The explosing wave can trigger other bomb to explode.
public class BombBullet : BulletShell
{
    public GameObject explosionBulletPrefab;
    public bool isInstantExploding;
    [SerializeField]
    MeshRenderer bombBallMesh;
    [SerializeField]
    Collider nonTriggerCollider;
    Collider ignoredCollider = null;

    Color startColor = new Color(50/255f, 50/255f, 50/255f);
    Color endColor = new Color(160/255f, 0, 0);

    public override void Initialize(Vector3 d, Creature owner) {
        if(nonTriggerCollider != null) {
            // Prevent bullet collide with creature itself
            ignoredCollider = owner.GetComponent<Collider>();
            Physics.IgnoreCollision(nonTriggerCollider, ignoredCollider);
        }

        base.Initialize(d, owner);        
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if(isInstantExploding) {
            bombBallMesh.material.color = endColor;
        }else {
            bombBallMesh.material.color = startColor;
        }
    }

    public override void Update()
    {
        if(!isInstantExploding) {
            bombBallMesh.material.color = Color.Lerp(startColor, endColor, bulletLifeDuration / (lifeTime + buff_lifeTime));
        }

        base.Update();
    }

    public void SetInstantExploding(bool isInstant) {
        isInstantExploding = isInstant;
        if(isInstantExploding) {
            bombBallMesh.material.color = endColor;
        }else {
            bombBallMesh.material.color = startColor;
        }
    }

    protected override void InteractToCreature(Collider other) {
        Creature c = other.gameObject.GetComponent<Creature>();
        if(c != null && c.team != this.team && c.currentHp > 0) {
            hitCountLeft--;

            if( onBulletTriggerStatus != null){
                onBulletTriggerStatus(other);
            }
        }
    }

    protected override void InteractToObstacle(Collider other) {
        if(isInstantExploding) {
            hitCountLeft--;
        }
    }

    protected override void InteractToEnvironment(Collider other) {
        if(isInstantExploding) {
            hitCountLeft--;
        }
    }

    protected override void ActionBeforeBulletDestory() {
        BulletShell shot = ObjectPoolManager.CreatePooled(explosionBulletPrefab.gameObject, BattleManager.Instance.projectileContainer).GetComponent<BulletShell>();
        shot.transform.position = transform.position;
        shot.Initialize(Vector3.zero, team);
        shot.damage = this.getBulletDamage();
        shot.impactForce = this.impactForce;

        // Cancel ignored collider before recycling
        if(ignoredCollider != null) {
            Physics.IgnoreCollision(nonTriggerCollider, ignoredCollider, false);
        }
    }

    public override void OnTriggerEnter(Collider other ) {
        int colliderLayer = other.gameObject.layer;
        if(colliderLayer == GameConstants.LayerImpactField) {
            hitCountLeft--;

            if( onBulletTriggerStatus != null){
                onBulletTriggerStatus(other);
            }
        }

        base.OnTriggerEnter(other);
    }

}
