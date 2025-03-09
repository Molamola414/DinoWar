using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    public Transform trackingTarget;
    public Color markerColor;

    SpriteRenderer ringRenderer;
    int layerMask;

    void Start()
    {
        ringRenderer = this.GetComponentInChildren<SpriteRenderer>();
        ringRenderer.color = markerColor;   
        layerMask = LayerMask.GetMask("Environment");
    }

    void Update()
    {
        if(trackingTarget != null && trackingTarget.gameObject.activeSelf) {
            RaycastHit hit;
            if(Physics.Raycast(trackingTarget.position, Vector3.down, out hit, 1000, layerMask)) {
                transform.position = hit.point;
            }
            else {
                transform.position = trackingTarget.position;
            }
        }
        else {
            Destroy(gameObject);
        }
    }
}
