using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBomberDinoAI : FigherDinoAI
{
    public BulletShell explosionBulletPrefab;

    const int WeaponGroundThrower = 0;
    const int WeaponHighThrower = 1;
    const int WeaponLineThrower = 2;
    const int WeaponMelee = 3;

    int shotToSwitchWeapon = 3;
    int toBiteCount = 0;

    public override void OnAttackStart(AnimationEvent animEvent)
    {
        base.OnAttackStart(animEvent);
    }

    public override void OnAttackEnd(AnimationEvent animEvent)
    {
        base.OnAttackEnd(animEvent);

        if(_creature.activeWeaponIndex == WeaponHighThrower || _creature.activeWeaponIndex == WeaponLineThrower) {
            _creature.AddImpact((Random.Range(0, 2) == 0?transform.right:-transform.right), 300);
        }

        shotToSwitchWeapon--;

        if(shotToSwitchWeapon <= 0) {
            switch(_creature.activeWeaponIndex) {
                case WeaponGroundThrower:
                {
                    if(Random.value > 0.3f) {
                        shotToSwitchWeapon = Random.Range(3, 6);
                        this.attackTimeout = 1f;
                        _creature.SetActiveWeapon(WeaponHighThrower);
                    }
                    else {
                        shotToSwitchWeapon = Random.Range(2, 6);
                        this.attackTimeout = 1f;
                        _creature.SetActiveWeapon(WeaponLineThrower);
                    }
                }
                break;

                case WeaponHighThrower:
                {
                    if(Random.value > 0.5f) {
                        shotToSwitchWeapon = Random.Range(5, 8);
                        this.attackTimeout = 0.5f;
                        _creature.SetActiveWeapon(WeaponGroundThrower);
                    }
                    else {
                        shotToSwitchWeapon = Random.Range(1, 3);
                        this.attackTimeout = 1f;
                        _creature.SetActiveWeapon(WeaponLineThrower);
                    }
                }
                break;

                case WeaponLineThrower:
                {
                    if(Random.value > 0.5f) {
                        shotToSwitchWeapon = Random.Range(3, 6);
                        this.attackTimeout = 1f;
                        _creature.SetActiveWeapon(WeaponHighThrower);
                    }
                    else {
                        shotToSwitchWeapon = Random.Range(5, 8);
                        this.attackTimeout = 0.5f;
                        _creature.SetActiveWeapon(WeaponGroundThrower);
                    }
                }
                break;

                case WeaponMelee:
                {
                    shotToSwitchWeapon = Random.Range(5, 8);
                    this.attackTimeout = 0.5f;
                    _creature.SetActiveWeapon(WeaponGroundThrower);
                }
                break;
            }
        }
    }

    public override void OnHit()
    {
        base.OnHit();

        if(Random.value > 0.7f) {
            _creature.AddImpact((Random.value > 0.5f?transform.right:-transform.right), 200);

            if(_creature.activeWeaponIndex == WeaponMelee) {
                shotToSwitchWeapon = Random.Range(5, 8);
                this.attackTimeout = 0.5f;
                _creature.SetActiveWeapon(WeaponGroundThrower);
            }
        }
    }

    public void OnDie()
    {
        StartCoroutine(WaitAndExplode());
    }

    protected override void RunAwayFromEnemy(Vector3 runDirection) {
        base.RunAwayFromEnemy(runDirection);

        toBiteCount++;
        if(toBiteCount > 80) {
            toBiteCount = 0;
            _creature.SetActiveWeapon(WeaponMelee);
            shotToSwitchWeapon = Random.Range(1, 3);
            this.attackTimeout = 0.5f;
        }
    }


    IEnumerator WaitAndExplode()
    {
        yield return new WaitForSeconds(0.4f);
        
        BulletShell shot = ObjectPoolManager.CreatePooled(explosionBulletPrefab.gameObject, BattleManager.Instance.projectileContainer).GetComponent<BulletShell>();
        shot.transform.position = transform.position;
        shot.Initialize(Vector3.zero, 999);
        shot.damage = 50;

    }
}
