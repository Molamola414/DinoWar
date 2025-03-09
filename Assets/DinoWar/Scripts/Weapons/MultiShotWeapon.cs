using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShotWeapon : RangedWeapon
{
    public float bulletVelocityBoostRate = 1f;
    public float bulletVelocityBoostValueStep = 0f;
    public int bulletPerShot = 3;
    public float angleRange = 30f;

    public override void TriggerWeapon(Vector3 attackDirection, int eventIdx)
    {
        base.TriggerWeapon(attackDirection, eventIdx);
        
        if(bulletPerShot <= 1)
            return;
        
        float boostRate = bulletVelocityBoostRate;
        for(int i=0; i<bulletPerShot-1; i++) {
            boostRate += bulletVelocityBoostValueStep;

            Vector3 newAttDir = Quaternion.AngleAxis(Random.Range(-angleRange, angleRange), Vector3.up) * attackDirection;
            createBullet(newAttDir, boostRate);
        }
    }

    public override BulletShell createBullet(Vector3 attackDirection) {
        return this.createBullet(attackDirection, bulletVelocityBoostRate);
    }

    public BulletShell createBullet(Vector3 attackDirection, float boostRate) {
        BulletShell newBullet = base.createBullet(attackDirection);

        newBullet.bulletRB.velocity *= boostRate;
        
        return newBullet;
    }

}
