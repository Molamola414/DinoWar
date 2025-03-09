using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic VFX behavior
// Destory itself when effect duration reached
public class BasicVFX : MonoBehaviour
{
    private float effectDuration = 5;
    private float durationCount;

    protected virtual void Awake()
    {
        ParticleSystem ps = this.GetComponent<ParticleSystem>();
        if(ps != null) {
            effectDuration = ps.main.duration;
        }
    }

    protected virtual void OnEnable() {
        durationCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        durationCount += Time.deltaTime;
        if(durationCount >= effectDuration) {
            ObjectPoolManager.DestroyPooled(gameObject);
        }
    }
}
