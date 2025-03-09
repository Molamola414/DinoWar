using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheRemains : MasterBullet
{

    public float duration; 
     public override void modeling(){

    
     
        Debug.Log("Modeling!!! update speed ");
        
    }


    private IEnumerator Start()
     {
         yield return new WaitForSeconds(duration);
         BulletShell bullet = gameObject.GetComponent<BulletShell>();
         if( bullet.onBulletDestoryStatus != null){
             bullet.onBulletDestoryStatus();             
         }
         Destroy(bullet.gameObject); 
     }



    public void OnTriggerEnter(Collider other ){
        //affrod target collided with it 
    }

}
