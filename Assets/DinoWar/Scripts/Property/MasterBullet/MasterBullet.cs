using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MasterBullet : MonoBehaviour{
    
    public bool isMaster = false; 

    public void Start(){
        if( isMaster){
            
            modeling(); 
        }
    }

    public void Update(){

    }

    public virtual void modeling(){

    }
}

