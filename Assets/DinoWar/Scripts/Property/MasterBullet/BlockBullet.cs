using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBullet : BulletShell
{
    public Collider blockNonTriggerCollider;

    public float randomDirectionThreshold;
    public float flowHeightThreshold;

    public bool isGenMiniBlockWhenKill; 
    public bool isExplosionWhenKill;
    
    public override void Initialize(Vector3 d, Creature owner) {
        // Prevent bullet collide with creature itself
        Physics.IgnoreCollision(blockNonTriggerCollider, owner.GetComponent<Collider>());

        d.y += Random.Range(1.0f, 1 + flowHeightThreshold);

        base.Initialize(d, owner);
        
    }

    // public override void modeling() {
    //     hitCountLimit = 5;
    //     rb =  this.GetComponent<Rigidbody>(); 

    //     // Debug.Log("Direction : " + direction);
    //    // rb.AddTorque (transform.right * 100);
    //     rb.velocity = new Vector3((direction.x + ownerWalkDirection.x) * speed, Random.Range(40, 70), (direction.z + ownerWalkDirection.y) * speed) ;
        
    // }

      public override void OnTriggerEnter(Collider other ) {
        base.OnTriggerEnter(other);

        ////////////
        // FIX:
        // 1. Cannot do this without checking what it is actually colliding with.
        // This function is triggered at the moment it spawns at creature mouth (throwing triggers to creature's self collider)
        // 2. new direction should be normalized. Right now it is just bouncing around with abnormal direction and speed changes.
        ////////////
        // Apply Random direction 
        int colliderLayer = other.gameObject.layer;
        if(colliderLayer == GameConstants.LayerEnvironment || colliderLayer == GameConstants.LayerObstacle) {
            direction  = new Vector3(   direction.x + Random.Range(-1 * randomDirectionThreshold, randomDirectionThreshold), 
                                        0, 
                                        direction.z + Random.Range( -1 * randomDirectionThreshold, randomDirectionThreshold)).normalized; 
            
            this.GetComponent<Rigidbody>().velocity = new Vector3(direction.x * speed, 
                                                                bulletRB.velocity.y /* * 0.8f // decrease elastic from material instead of changing speed here */,
                                                                direction.z * speed) ;
        }
    }

}
