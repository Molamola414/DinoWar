using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BomberDinoAI : FigherDinoAI
{
    public BulletShell explosionBulletPrefab;
    public override void OnAttackEnd(AnimationEvent animEvent)
    {
        base.OnAttackEnd(animEvent);

        // _creature.currentHp = 0;

        // transform.localScale = defaultScale;
        // Sequence seq = DOTween.Sequence();
        // seq.Append(transform.DOScale(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.OutElastic));
        // seq.AppendCallback(()=> {
            StartCoroutine(WaitOneFrameToRecycleSelf());
        // });

        BulletShell shot = ObjectPoolManager.CreatePooled(explosionBulletPrefab.gameObject, BattleManager.Instance.projectileContainer).GetComponent<BulletShell>();
        shot.transform.position = transform.position;
        shot.Initialize(Vector3.zero, 999);
    }

    public void OnDie()
    {
        StartCoroutine(WaitAndExplode());
    }

    IEnumerator WaitOneFrameToRecycleSelf() {
        yield return new WaitForEndOfFrame();

        ObjectPoolManager.DestroyPooled(gameObject);
    }

    IEnumerator WaitAndExplode()
    {
        yield return new WaitForSeconds(0.4f);
        
        BulletShell shot = ObjectPoolManager.CreatePooled(explosionBulletPrefab.gameObject, BattleManager.Instance.projectileContainer).GetComponent<BulletShell>();
        shot.transform.position = transform.position;
        shot.Initialize(Vector3.zero, 999);

    }
}
