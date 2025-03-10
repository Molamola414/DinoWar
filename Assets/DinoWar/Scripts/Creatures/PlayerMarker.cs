using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    public Transform trackingTarget;
    public Color markerColor;

    private SpriteRenderer ringRenderer;
    private int layerMask;
    private static readonly float raycastDistance = 1000f;

    void Awake()
    {
        ringRenderer = GetComponentInChildren<SpriteRenderer>();
        if(ringRenderer != null)
        {
            ringRenderer.color = markerColor;   
        }
        else {
            Debug.LogWarning("PlayerMarker: No SpriteRenderer found in children!", this);
        }
    }

    void Start()
    {
        layerMask = LayerMask.GetMask("Environment");
    }

    void Update()
    {
        if(trackingTarget != null && trackingTarget.gameObject.activeSelf) {
            if(Physics.Raycast(trackingTarget.position, Vector3.down, out RaycastHit hit, raycastDistance, layerMask)) {
                transform.position = hit.point;
            }
            else {
                transform.position = trackingTarget.position;
            }
        }
        else {
            SelfDestruct();
        }
    }

    private void SelfDestruct()
    {
        Destroy(gameObject, 0.5f);
    }
}
