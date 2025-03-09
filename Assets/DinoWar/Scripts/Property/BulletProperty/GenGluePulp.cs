using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenGluePulp : MonoBehaviour{

    public GluePulp gluePulpBullet;

    void Start()
    {
        BulletShell shell = gameObject.GetComponent<BulletShell>(); 

        if( shell != null){
            shell.onBulletTriggerStatus += genGluePulp; 
        }
    }


    public void genGluePulp(Collider other){        
        Debug.Log("Gen Glue  Pulp!!!!");
        GameObject mb = ObjectPoolManager.CreatePooled(gluePulpBullet.gameObject, BattleManager.Instance.projectileContainer);
        mb.GetComponent<BulletShell>().Initialize(Vector3.zero, gameObject.GetComponent<BulletShell>().team);
        mb.transform.position  = new Vector3 (gameObject.transform.position.x,
        0, 
            gameObject.transform.position.z);
        
    }

}
