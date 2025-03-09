using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BMovement), typeof(BulletShell))]
public class BReflection : MonoBehaviour
{
    BulletShell bullet;
    // Start is called before the first frame update
    void Start()
    {
        bullet = gameObject.GetComponent<BulletShell>();
       // bullet.onBulletTriggerStatus += onBulletTriggerStatus;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    // public void onBulletTriggerStatus(Collision collision){
    //     BMovement move = gameObject.GetComponent<BMovement>();

    //     Vector2 inDirection = new Vector2(move.direction.x, move.direction.z);
    //     Vector2 inNormal =  collision.contacts[0].normal;
       
    //     Vector2 newVelocity = Vector2.Reflect(inDirection, inNormal);

    //     move.direction = new Vector3(newVelocity.x, 0, newVelocity.y);
    // }
}
