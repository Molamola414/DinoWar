using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBomberDinoAI : FigherDinoAI
{
    public BulletShell explosionBulletPrefab;

    private enum BossBomberWeaponType
    {
        GroundThrower,
        HighThrower,
        LineThrower,
        Melee
    }

    int shotToSwitchWeapon = 3;
    int toBiteCount = 0;

    public override void OnAttackStart(AnimationEvent animEvent)
    {
        base.OnAttackStart(animEvent);
    }

    public override void OnAttackEnd(AnimationEvent animEvent)
    {
        base.OnAttackEnd(animEvent);

        if(_creature.activeWeaponIndex == (int)BossBomberWeaponType.HighThrower || _creature.activeWeaponIndex == (int)BossBomberWeaponType.LineThrower) {
            _creature.AddImpact((Random.Range(0, 2) == 0?transform.right:-transform.right), 300);
        }

        shotToSwitchWeapon--;

        if(shotToSwitchWeapon <= 0) {
            SwitchWeapon();
        }
    }

    public override void OnHit()
    {
        base.OnHit();

        if(Random.value > 0.7f) {
            _creature.AddImpact((Random.value > 0.5f?transform.right:-transform.right), 200);

            if(_creature.activeWeaponIndex == (int)BossBomberWeaponType.Melee) {
                shotToSwitchWeapon = Random.Range(5, 8);
                this.attackTimeout = 0.5f;
                _creature.SetActiveWeapon((int)BossBomberWeaponType.GroundThrower);
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
            _creature.SetActiveWeapon((int)BossBomberWeaponType.Melee);
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

    private void SwitchWeapon() {
        switch((BossBomberWeaponType)_creature.activeWeaponIndex) {
            case BossBomberWeaponType.GroundThrower:
            {
                if(Random.value > 0.3f) {
                    shotToSwitchWeapon = Random.Range(3, 6);
                    this.attackTimeout = 1f;
                    _creature.SetActiveWeapon((int)BossBomberWeaponType.HighThrower);
                }
                else {
                    shotToSwitchWeapon = Random.Range(2, 6);
                    this.attackTimeout = 1f;
                    _creature.SetActiveWeapon((int)BossBomberWeaponType.LineThrower);
                }
            }
            break;

            case BossBomberWeaponType.HighThrower:
            {
                if(Random.value > 0.5f) {
                    shotToSwitchWeapon = Random.Range(5, 8);
                    this.attackTimeout = 0.5f;
                    _creature.SetActiveWeapon((int)BossBomberWeaponType.GroundThrower);
                }
                else {
                    shotToSwitchWeapon = Random.Range(1, 3);
                    this.attackTimeout = 1f;
                    _creature.SetActiveWeapon((int)BossBomberWeaponType.LineThrower);
                }
            }
            break;

            case BossBomberWeaponType.LineThrower:
            {
                if(Random.value > 0.5f) {
                    shotToSwitchWeapon = Random.Range(3, 6);
                    this.attackTimeout = 1f;
                    _creature.SetActiveWeapon((int)BossBomberWeaponType.HighThrower);
                }
                else {
                    shotToSwitchWeapon = Random.Range(5, 8);
                    this.attackTimeout = 0.5f;
                    _creature.SetActiveWeapon((int)BossBomberWeaponType.GroundThrower);
                }
            }
            break;

            case BossBomberWeaponType.Melee:
            {
                shotToSwitchWeapon = Random.Range(5, 8);
                this.attackTimeout = 0.5f;
                _creature.SetActiveWeapon((int)BossBomberWeaponType.GroundThrower);
            }
            break;
        }
    }
}
