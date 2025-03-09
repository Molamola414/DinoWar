using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMovement : MonoBehaviour
{
    // Alex: Speed should be using float value?
    public /*Vector3*/ float speed; 
    public  Vector3 direction;


    // Alex: Suggest to remove this delegate since it is called every update cycle.
    // If someone wants to access it, just get the bullet direction, transform is enough
    public delegate void OnBulletMove();
    public OnBulletMove onMove;

    public void Update()
    {    
        gameObject.transform.position += direction * speed * Time.deltaTime;

        if( onMove != null){
            onMove(); 
        }
    }

}
