using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroBullet : BulletShell
{
    public float percentageToRegenANewOneAfterKill = 0; 

    public override void OnTriggerEnter(Collider other ){
        
        if( onBulletTriggerStatus != null){
            onBulletTriggerStatus(other);
        }

        Creature c = other.gameObject.GetComponent<Creature>();
        if(c != null && c.team != this.team) {
            
            for( int i =0 ; i < hitCountLimit; i++){
                c.GetDamage(this.damage);
            }
            hitCountLimit = 0;         
            destoryBullet();
        }        
    }

}
