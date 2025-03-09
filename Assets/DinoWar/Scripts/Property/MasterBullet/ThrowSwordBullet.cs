using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSwordBullet : BulletShell
{
    Rigidbody rb ;

    
    public override void Initialize(Vector3 d, Creature owner) {
        base.Initialize(d, owner);
        rb =  this.GetComponent<Rigidbody>();        
        
        rb.velocity = new Vector3((direction.x + ownerWalkDirection.x) * speed, Random.Range(speed - 20, speed), (direction.z + ownerWalkDirection.y) * speed) ;
        rb.AddTorque (new Vector3(0, 3000,0 ), ForceMode.Impulse);  

        transform.rotation = Quaternion.LookRotation(direction);
        
    }
 
    public override void OnTriggerEnter(Collider other ) {
        base.OnTriggerEnter(other);
   
        int colliderLayer = other.gameObject.layer;
        if(colliderLayer == GameConstants.LayerEnvironment || colliderLayer == GameConstants.LayerObstacle) {
        }
    }
}
