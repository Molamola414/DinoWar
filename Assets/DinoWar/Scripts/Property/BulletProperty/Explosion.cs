using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public GameObject explosionPrefabs; 

    // Start is called before the first frame update
    void Start()
    {
        BulletShell shell = gameObject.GetComponent<BulletShell>(); 

        if( shell != null){
            shell.onBulletDestoryStatus += onBulletDestoryStatus; 
        }

    }

    public void onBulletDestoryStatus(){

        //Debug.Log("On bullet destory !");
        GameObject explosion = Instantiate(explosionPrefabs, gameObject.transform.parent);
        
        explosion.transform.position  = gameObject.transform.position;    
        explosion.GetComponent<ParticleSystem>().Play();

        Destroy(explosion, 3);
    }
}
