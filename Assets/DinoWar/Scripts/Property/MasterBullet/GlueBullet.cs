using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueBullet : BulletShell
{

    // public override void modeling(){
    //     // Alex: Speed should be defined in prefab or the weapon shooting this bullet
    //     // Debug.Log("Modeling!!! update speed ");
    //     // BMovement movement = gameObject.GetComponent<BMovement>(); 
    //     // if (movement != null){
    //     //     movement.speed.z = movement.speed.x = 100; 

    //     //     Debug.Log("Modeling!!! update speed complete");
    //     // }
    // }

    public override void OnTriggerEnter(Collider other ) {
        base.OnTriggerEnter(other);
    }

}
