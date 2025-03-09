using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchDinoAI : FigherDinoAI
{
    [SerializeField]
    GameObject helperPrefab;

    public int helperSpawnNumber = 3;
    public float skillInterval = 5;
    public float skillPrepareDuration = 1;
    private float skillTimeCount;

    private bool isSpawningHelper = false;

    public override void OnHit()
    {
        base.OnHit();

        if(skillTimeCount >= skillInterval && (skillTimeCount < skillInterval + skillPrepareDuration)) {
            isSpawningHelper = false;
            _creature.SetPosture(Creature.Posture.None);
            skillTimeCount = 0;
        }
    }

    public override void OnAttackStart(AnimationEvent animEvent)
    {
        base.OnAttackStart(animEvent);
    }

    public override void OnAttackEnd(AnimationEvent animEvent)
    {
        base.OnAttackEnd(animEvent);
    }

    protected override void RangedMove(Vector3 direction, bool canAttack, bool canSeeEnemy)
    {
        skillTimeCount += Time.deltaTime;
        if(skillTimeCount >= skillInterval) {
            if(!isSpawningHelper) {
                var helpers = BattleManager.Instance.creatureList.Where(i =>i.currentHp > 0 && i.team == _creature.team && i.creatureType == helperPrefab.GetComponent<Creature>().creatureType).ToList();
                if(helpers.Count < 5) {
                    isSpawningHelper = true;
                    _creature.SetPosture(Creature.Posture.Happy);
                }
                else {
                    _creature.SetPosture(Creature.Posture.None);
                    isSpawningHelper = false;
                    skillTimeCount = 0;
                }

            }
            else {
                if(skillTimeCount >= skillInterval + skillPrepareDuration) {
                    isSpawningHelper = false;
                    this.SpawnChildren(helperSpawnNumber);
                }
            }
        }
        else {
            base.RangedMove(direction, canAttack, canSeeEnemy);
        }
    }

    public void SpawnChildren(int numOfGenin) {
        _creature.SetPosture(Creature.Posture.None);
        skillTimeCount = 0;

        for(int i=0; i<numOfGenin; i++) {
            Creature newEnemy = ObjectPoolManager.CreatePooled(helperPrefab, BattleManager.Instance.creatureContainer).GetComponent<Creature>();
            newEnemy.team = 1;

            Vector2 ranDirection = Random.insideUnitCircle;
            newEnemy.transform.position = new Vector3(ranDirection.x * 20, 0, ranDirection.y * 20) + transform.position;
            newEnemy.transform.rotation = transform.rotation;

        }
    } 
}
