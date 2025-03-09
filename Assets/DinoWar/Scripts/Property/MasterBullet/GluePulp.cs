using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluePulp : BulletShell
{


    public float radiusOfPulp;
    public float buff_radiusOfPulp;


     public override void OnTriggerEnter(Collider other ) {
        base.OnTriggerEnter(other);
      //   int colliderLayer = other.gameObject.layer;
      //   if(colliderLayer == GameConstants.LayerEnvironment || colliderLayer == GameConstants.LayerObstacle) {           
      //   }
     }

}
