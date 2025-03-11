using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BulletDetector : MonoBehaviour
{
    public SphereCollider detectingCollider;
    public readonly List<BulletShell> bullets = new List<BulletShell>();
    // Start is called before the first frame update
    public virtual void Awake()
    {
        bullets.Clear();
    }

    public List<BulletShell> GetBullets() {
        for(int i = bullets.Count-1; i >= 0; i--) {
            if(bullets[i] == null || (bullets[i] != null && !bullets[i].gameObject.activeSelf)) {
                bullets.RemoveAt(i);
            }
        }
        return bullets;
    }

    public void OnTriggerEnter(Collider target)
    {
        var bullet = target.GetComponentInChildren<BulletShell>();

        if (bullet != null && !bullets.Contains(bullet))
        {
            bullets.Add(bullet);
        }
    }

    public void OnTriggerExit(Collider target)
    {
        var bullet = target.GetComponentInChildren<BulletShell>();

        if (bullet != null && bullets.Contains(bullet))
        {
            bullets.Remove(bullet);
        }
    }
}
