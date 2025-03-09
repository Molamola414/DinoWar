using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    // Consider the case an attack animation with multiple hit
    public BulletShell[] bulletShells;

    public override void Start()
    {
        base.Start();

        this.attackType = Weapon.AttackType.Melee;

        foreach(BulletShell bs in bulletShells) {
            bs.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void TriggerWeapon(Vector3 attackDirection, int eventIdx)
    {
        base.TriggerWeapon(attackDirection, eventIdx);

        // Activiate melee bullet object (collider) for certain time
        if(eventIdx < bulletShells.Length) {
            bulletShells[eventIdx].gameObject.SetActive(true);
            bulletShells[eventIdx].Initialize(attackDirection, weaponOwner);
        }
    }

    public override void TriggerWeaponEnd(int eventIdx)
    {
        // TO-DO: Need to consider the case that attack is stopped by damage. Somewhere else should turn off bulletShells
        base.TriggerWeaponEnd(eventIdx);
        if(eventIdx < bulletShells.Length) {
            bulletShells[eventIdx].gameObject.SetActive(false);
        }
    }

}
