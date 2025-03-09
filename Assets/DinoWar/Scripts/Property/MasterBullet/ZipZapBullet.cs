using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipZapBullet : BulletShell
{
    
    // Update is called once per frame
    
    public int variance;

    Rigidbody rb ;

    float angle; 

    
    public override void Initialize(Vector3 d, Creature owner) {
        base.Initialize(d, owner);
        rb =  this.GetComponent<Rigidbody>();                
        
        rb.velocity = new Vector3((direction.x + ownerWalkDirection.x) * speed, 
                            0, (direction.z + ownerWalkDirection.y) * speed) ;
       // rb.AddTorque (new Vector3(0, 3000,0 ), ForceMode.Impulse);  

        transform.rotation = Quaternion.LookRotation(direction);
        
    }
 
    public override void OnTriggerEnter(Collider other ) {
        base.OnTriggerEnter(other);
   
        int colliderLayer = other.gameObject.layer;
        if(colliderLayer == GameConstants.LayerEnvironment || colliderLayer == GameConstants.LayerObstacle) {
        }
    }


    void Update(){

        base.Update(); 
        angle += 0.01f;//Time.deltaTime * 180; 
        angle %= 360;
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + 1 * Mathf.Sin(angle), 
        gameObject.transform.localPosition.y, 
        gameObject.transform.localPosition.z);
        
    }

    protected override void move()
    {
         
        gameObject.GetComponent<Rigidbody>().AddForce( new Vector3(speed * Mathf.Sin(bulletLifeDuration * 90.0f) * variance, 0, speed * direction.z)); //Moving projectile
        //gameObject.GetComponent<Rigidbody>().AddForce( new Mathf.Sin(bulletLifeDuration * 90.0f) * variance;); //        
        //Apply random left / right focus   
    }
}
