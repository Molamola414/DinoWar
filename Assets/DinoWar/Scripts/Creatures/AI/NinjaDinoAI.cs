using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaDinoAI : FigherDinoAI
{
    bool isDisguiseMode = false;
    [SerializeField]
    GameObject geninPrefab;
    [SerializeField]
    GameObject smokeVFXPrefab;

    [SerializeField]
    Material normalModeMaterial;
    [SerializeField]
    Material ninjaModeMaterial;
    
    [SerializeField]
    SkinnedMeshRenderer[] dinoMeshes;
    [SerializeField]
    GameObject[] accessoryList;

    private float originalSpeed;
    public float disguiseSpeed = 30;
    public int geninSpawnNumber = 3;
    public float skillInterval = 5;
    public float skillPrepareDuration = 1;
    private float skillTimeCount;

    private bool isSpawningHelper = false;

    private int hitAccum = 5;

    public override void Awake() {
        base.Awake();

        originalSpeed = _creature.speed;
        this.SwitchWeapon();
    }

    public override void OnHit()
    {
        base.OnHit();

        if(_creature.GetActiveWeapon().attackType == Weapon.AttackType.Melee) {
            if(isDisguiseMode) {
                this.SetDisguiseMode(false);
                _creature.AddImpact(-transform.forward, 200);
            }
            else {
                if(skillTimeCount >= skillInterval && (skillTimeCount < skillInterval + skillPrepareDuration)) {
                    _creature.SetPosture(Creature.Posture.None);
                    skillTimeCount = 0;
                }
                else {
                    if(Random.value > 0.6f)
                        _creature.AddImpact((Random.value > 0.5f?transform.right:-transform.right), 300);
                }
            }
        }
        else {
            if(skillTimeCount >= skillInterval && (skillTimeCount < skillInterval + skillPrepareDuration)) {
                isSpawningHelper = false;
                _creature.SetPosture(Creature.Posture.None);
                skillTimeCount = 0;
            }
        }

        hitAccum--;
        if(hitAccum <= 0) {
            this.SwitchWeapon();
            hitAccum = Random.Range(5, 15);
        }
    }

    public override void OnAttackStart(AnimationEvent animEvent)
    {
        base.OnAttackStart(animEvent);

        if(_creature.GetActiveWeapon().attackType == Weapon.AttackType.Ranged) {
            // _creature.AddImpact((Random.Range(0, 2) == 0?transform.right:-transform.right), 400);
        }
        else {
            this.SetDisguiseMode(false);
        }
    }

    public override void OnAttackEnd(AnimationEvent animEvent)
    {
        base.OnAttackEnd(animEvent);

        if(_creature.GetActiveWeapon().attackType == Weapon.AttackType.Ranged) {
            _creature.AddImpact((Random.Range(0, 2) == 0?transform.right:-transform.right), 400);
        }

    }

    private void SwitchWeapon() {
        _creature.SwitchActiveWeapon();
        this.SetDisguiseMode(false);

        // if(_creature.GetActiveWeapon().attackType == Weapon.AttackType.Ranged) {

    }

    protected override void MeleeMove(Vector3 direction, bool canAttack)
    {
        skillTimeCount += Time.deltaTime;
        if(isDisguiseMode == false && skillTimeCount >= skillInterval) {
            _creature.SetPosture(Creature.Posture.Happy);

            if(skillTimeCount >= skillInterval + skillPrepareDuration) {
                this.SetDisguiseMode(true);
            }
        }
        else {
            base.MeleeMove(direction, canAttack);
        }
    }

    protected override void RangedMove(Vector3 direction, bool canAttack, bool canSeeEnemy)
    {
        skillTimeCount += Time.deltaTime;
        if(skillTimeCount >= skillInterval) {
            if(!isSpawningHelper) {
                var genins = BattleManager.Instance.creatureList.Where(i =>i.currentHp > 0 && i.team == _creature.team && i.creatureType == geninPrefab.GetComponent<Creature>().creatureType).ToList();
                if(genins.Count < 5) {
                    isSpawningHelper = true;
                    _creature.SetPosture(Creature.Posture.Happy);
                }
                else {
                    this.SwitchWeapon();
                    isSpawningHelper = false;
                    hitAccum = Random.Range(5, 15);
                    skillTimeCount = 0;
                }
            }
            else {
                if(skillTimeCount >= skillInterval + skillPrepareDuration) {
                    isSpawningHelper = false;
                    this.SpawnGenins(geninSpawnNumber);
                }
            }

        }
        else {
            base.RangedMove(direction, canAttack, canSeeEnemy);
        }
    }

    public void SpawnGenins(int numOfGenin) {
        _creature.SetPosture(Creature.Posture.None);
        skillTimeCount = 0;

        for(int i=0; i<numOfGenin; i++) {
            Creature newEnemy = ObjectPoolManager.CreatePooled(geninPrefab, BattleManager.Instance.creatureContainer).GetComponent<Creature>();
            newEnemy.team = 1;

            Vector2 ranDirection = Random.insideUnitCircle;
            newEnemy.transform.position = new Vector3(ranDirection.x * 20, 0, ranDirection.y * 20) + transform.position;
            newEnemy.transform.rotation = transform.rotation;

            GameObject smokeVFX = ObjectPoolManager.CreatePooled(smokeVFXPrefab, BattleManager.Instance.creatureContainer);
            smokeVFX.transform.position = newEnemy.transform.position + new Vector3(0, 1, 0);

        }
    } 

    public void SetDisguiseMode(bool isDisguise)
    {
        _creature.SetPosture(Creature.Posture.None);
        skillTimeCount = 0;

        if(isDisguiseMode == isDisguise) return;
        
        isDisguiseMode = isDisguise;

        if(isDisguiseMode) {
            _creature.speed = disguiseSpeed;

            for(int i=0; i<accessoryList.Length; i++) {
                accessoryList[i].SetActive(false);
            }
            for(int i=0; i<dinoMeshes.Length; i++) {
                dinoMeshes[i].material = ninjaModeMaterial;
            }
        }
        else {
            _creature.speed = originalSpeed;
            
            for(int i=0; i<accessoryList.Length; i++) {
                accessoryList[i].SetActive(true);
            }
            for(int i=0; i<dinoMeshes.Length; i++) {
                dinoMeshes[i].material = normalModeMaterial;
            }
        }
    }

}
