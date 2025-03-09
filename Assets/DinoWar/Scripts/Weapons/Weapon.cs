using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Weapon : MonoBehaviour
{
    public enum AttackType {
        None = 0,
        Melee = 1,
        Ranged,
        Trap
    }
    [HideInInspector] public AttackType attackType;
    protected Creature weaponOwner;
    public float walkSpeedRatioInAttack;
    public bool canTrigger = true;
    public float recoilForce = 0;
    // Used by AI
    public SphereCollider detectingCollider;
    [HideInInspector]
    public List<Creature> targets = new List<Creature>();
    public int bulletLeft = -1;

    public virtual void Start()
    {
        weaponOwner = this.GetComponentInParent<Creature>();
    }

    public virtual void Awake()
    {
        // targets.Clear();
    }

    public virtual void OnEnable()
    {
        targets.Clear();
    }

    public virtual void Update()
    {

    }

    public virtual void ResetWeapon()
    {
        targets.Clear();
    }

    public virtual void TriggerWeapon(Vector3 attackDirection, int eventIdx = 0)
    {
        bulletLeft--;

        if(recoilForce != 0) {
            Vector3 recoilDir = weaponOwner.transform.forward * -1;
            weaponOwner.AddImpact(recoilDir, recoilForce);
        }
    }

    public virtual void TriggerWeaponEnd(int eventIdx = 0)
    {
        // Override your code here
    }


    // Used by AI
    /// <summary>
    /// Target creature enters weapon collider
    /// </summary>
    /// <param name="target">Target</param>
    public void OnTriggerEnter(Collider target)
    {
        var creature = target.GetComponentInChildren<Creature>();

        if (creature != null && !targets.Contains(creature))
        {
            targets.Add(creature);
        }
    }

    /// <summary>
    /// Target creature exits weapon collider
    /// </summary>
    /// <param name="target">Target</param>
    public void OnTriggerExit(Collider target)
    {
        var creature = target.GetComponentInChildren<Creature>();

        if (creature != null && targets.Contains(creature))
        {
            targets.Remove(creature);
        }
    }
}
