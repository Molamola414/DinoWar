using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardAlignCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    void LateUpdate()
    {
        Vector3 oRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, oRotation.y, oRotation.z);
    }
}
