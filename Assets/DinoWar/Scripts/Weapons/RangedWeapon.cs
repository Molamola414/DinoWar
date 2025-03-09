using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    // TO-DO: Change gameobject to base bullet class
    public BulletShell bulletPrefab;

    // Position of bullet being shot
    public Transform muzzleTransform;

    // Add spinning force to bullet
    public Vector3 torqueForce = Vector3.zero;

    public float walkingSpeedAffection = 0f;
    public float bulletUpwardValue = 0;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        this.attackType = Weapon.AttackType.Ranged;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }


    public virtual BulletShell createBullet(Vector3 attackDirection){
        BulletShell shot = ObjectPoolManager.CreatePooled(bulletPrefab.gameObject, BattleManager.Instance.projectileContainer).GetComponent<BulletShell>();
        shot.transform.position  = muzzleTransform.position;

        Vector3 finalDir = new Vector3(attackDirection.x, bulletUpwardValue, attackDirection.z).normalized;
        if(walkingSpeedAffection > 0) {
            finalDir += new Vector3(weaponOwner.controls.Direction.x, 0, weaponOwner.controls.Direction.y) * walkingSpeedAffection;
        }

        shot.Initialize(finalDir, weaponOwner);

        if(torqueForce != Vector3.zero && shot.bulletRB != null) {
            shot.bulletRB.AddTorque(torqueForce, ForceMode.Force);
        }

        return shot;
    }

    public override void TriggerWeapon(Vector3 attackDirection, int eventIdx)
    {
        if(bulletLeft == 0) {
            return;
        }

        base.TriggerWeapon(attackDirection);
        
        AudioPlayer.PlayEffect("Shot");

        createBullet(attackDirection);

        // shot.transform.position = weaponAttachPoint.transform.position;
        // shot.Initialize(attackDirection, this);
    }
}
