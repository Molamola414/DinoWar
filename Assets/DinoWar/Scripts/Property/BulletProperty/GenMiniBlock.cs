using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenMiniBlock : MonoBehaviour
{
    public int numOfMiniBlockGen ;

    public int buff_numOfMiniBlockGen;

    public int miniBlockDamage; 

    public int buff_miniBlockDamage;
    public GameObject miniBlock;
    
    // Start is called before the first frame update
    void Start()
    {
        BulletShell shell = gameObject.GetComponent<BulletShell>(); 

        if( shell != null){
            shell.onBulletTriggerStatus += genMiniBlock; 
        }
    }

    public int getMiniBlockDamage(){
        return miniBlockDamage + buff_miniBlockDamage;
    }


    public void genMiniBlock(Collider other){
        for( int i =0; i < numOfMiniBlockGen + buff_numOfMiniBlockGen; i++){
            GameObject mb = ObjectPoolManager.CreatePooled(miniBlock.gameObject, BattleManager.Instance.projectileContainer);
            mb.GetComponent<BulletShell>().Initialize(Vector3.zero, gameObject.GetComponent<BulletShell>().team);
            mb.GetComponent<BulletShell>().damage = getMiniBlockDamage(); 
            mb.transform.position  = gameObject.transform.position;   
        }
    }

}
