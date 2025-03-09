using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosingBullet : BulletShell
{
    public GameObject explosingVFXPrefab;

    public override void Initialize(Vector3 dir, int teamID)
    {
        base.Initialize(dir, teamID);

        GameObject hitVFX = ObjectPoolManager.CreatePooled(explosingVFXPrefab, BattleManager.Instance.projectileContainer);
        hitVFX.transform.position = this.transform.position;
    }

    protected override void ActionBeforeBulletDestory() {
    }

}
