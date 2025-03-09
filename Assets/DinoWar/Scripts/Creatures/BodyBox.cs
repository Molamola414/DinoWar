using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBox : MonoBehaviour
{
    public void OnTriggerStay(Collider target)
    {
        var offset = (target.transform.position - transform.position)/(GetComponent<Rigidbody>().mass * 10.0f);
        offset.y = 0;
        
        transform.parent.position -= offset;
    }
}
