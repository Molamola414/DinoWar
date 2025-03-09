using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRotation : MonoBehaviour
{

    public Vector3 angleSpeed;



    public void Update(){
        gameObject.transform.rotation = 
        new Quaternion(gameObject.transform.rotation.x + angleSpeed.x * Time.deltaTime, 
        gameObject.transform.rotation.y + angleSpeed.y * Time.deltaTime, 
        gameObject.transform.rotation.z + angleSpeed.z * Time.deltaTime,
        gameObject.transform.rotation.w); 

    }
}
