using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBullet : BulletShell
{   
    public override void Initialize(Vector3 d, Creature owner) {
        base.Initialize(d, owner);
        
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void FixedUpdate() {
        transform.rotation = Quaternion.LookRotation(bulletRB.velocity);
    }
}
