using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using Random = UnityEngine.Random;
public class BattleManager : MonoBehaviour
{
    public enum BattleStatus {
        BattleStatus_Ready,
        BattleStatus_Fighting,
        BattleStatus_Penality,
        BattleStatus_Reward,
        BattleStatus_Win,
        BattleStatus_Lose,
        BattleStatus_Result
    }

    public BattleStatus currentBattleStatus;

    public FixedJoystick joystickWalk;
    public FixedJoystick joystickWeapon;

    public Transform projectileContainer;
    public Transform creatureContainer;
    public Transform playerSpawnLocation;
    public Transform[] enemySpawnLocations;

    public ChasingCamera chasingCamera;

    [HideInInspector]
    public List<Creature> creatureList = new List<Creature>();
    public Creature[] playerPrefabList;
    public Creature[] allyPrefabList;
    public Creature[] enemyMPrefabList;
    public Creature[] enemySRPrefabList;
    public Creature[] enemyRPrefabList;
    public Creature[] enemySPrefabList;
    public Creature[] enemySPPrefabList;

    public Dictionary<GameConstants.EnemeyType, Creature[]> enemyPrefabDict = new Dictionary<GameConstants.EnemeyType, Creature[]>();

    private PlayerMarker markerPrefab;
    private Queue<EnemyWaveData> pendingWave = new Queue<EnemyWaveData>();
    [HideInInspector]
    public int numberOfWaveSurvived = 0;

    private float timeLimit;
    private float timeleft;

    public static BattleManager Instance;

    public Text statusLabel;
    public Button resetButton;

    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        markerPrefab = Resources.Load<PlayerMarker>("Prefabs/Stage/PlayerMarker");

        enemyPrefabDict.Add(GameConstants.EnemeyType.Melee, enemyMPrefabList);
        enemyPrefabDict.Add(GameConstants.EnemeyType.ShortRanged, enemySRPrefabList);
        enemyPrefabDict.Add(GameConstants.EnemeyType.Ranged, enemyRPrefabList);
        enemyPrefabDict.Add(GameConstants.EnemeyType.Special, enemySPrefabList);
        enemyPrefabDict.Add(GameConstants.EnemeyType.Support, enemySPPrefabList);
        
        // Need to generate
        // pendingWave.Enqueue(EnemyWaveData.GetNormalWaveData());
        // pendingWave.Enqueue(EnemyWaveData.GetNormalWaveData());
        // pendingWave.Enqueue(EnemyWaveData.GetMeleeWaveData());
        // pendingWave.Enqueue(EnemyWaveData.GetShootersWaveData());
        // pendingWave.Enqueue(EnemyWaveData.GetMeleeWaveData());
        // pendingWave.Enqueue(EnemyWaveData.GetShootersWaveData());

        this.ResetGame();
        // this.SetBattleStatus(BattleStatus.BattleStatus_Ready);
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentBattleStatus) {
            case BattleStatus.BattleStatus_Ready:
            {
                timeleft -= Time.deltaTime;
                if( statusLabel != null){
                    statusLabel.text = "READY: " + (int)timeleft;
                }


                // Tell UI how many time left
                if(timeleft <= 0) {
                    Debug.Log("Battle Start");
                    this.SetBattleStatus(BattleStatus.BattleStatus_Fighting);
                }
            }
            break;

            case BattleStatus.BattleStatus_Fighting:
            {
                timeleft -= Time.deltaTime;
                statusLabel.text = "Wave: " + numberOfWaveSurvived + ", " + (int)timeleft;

                if(timeleft <= 0) {
                    Debug.Log("Wave time's up");

                    // Count how many creatures left
                    List<Creature> enemyLeft = new List<Creature>();
                    for(int i=0; i<creatureList.Count; i++) {
                        if(creatureList[i].currentHp > 0 && creatureList[i].team == 1) {
                            enemyLeft.Add(creatureList[i]);
                        }
                    }

                    if(enemyLeft.Count > 0) {
                        this.SetBattleStatus(BattleStatus.BattleStatus_Penality);
                    }
                    else {
                        this.SetBattleStatus(BattleStatus.BattleStatus_Reward);
                    }

                }
            }
            break;

            case BattleStatus.BattleStatus_Reward:
            {
                timeleft -= Time.deltaTime;
                statusLabel.text = "Wave: " + numberOfWaveSurvived + ", REWARD" + (int)timeleft;

                if(timeleft <= 0) {
                    this.SetBattleStatus(BattleStatus.BattleStatus_Fighting);
                }
            }
            break;

            case BattleStatus.BattleStatus_Penality:
            {
                timeleft -= Time.deltaTime;
                statusLabel.text = "Wave: " + numberOfWaveSurvived + ", PENALITY" + (int)timeleft;

                if(timeleft <= 0) {
                    this.SetBattleStatus(BattleStatus.BattleStatus_Fighting);
                }

            }
            break;

            case BattleStatus.BattleStatus_Win:
            break;

            case BattleStatus.BattleStatus_Lose:
            {
                timeleft -= Time.deltaTime;
                if(timeleft <= 0) {
                    this.SetBattleStatus(BattleStatus.BattleStatus_Result);
                }
            }
            break;

            case BattleStatus.BattleStatus_Result:
            break;
        }
    }

    public void SetBattleStatus(BattleStatus newStatus) {
        currentBattleStatus = newStatus;

        switch(currentBattleStatus) {
            case BattleStatus.BattleStatus_Ready:
            {
                timeLimit = timeleft = 5;

                resetButton.gameObject.SetActive(false);
            }
            break;

            case BattleStatus.BattleStatus_Fighting:
            {
                numberOfWaveSurvived++;

                int randomWave = Random.Range((int)EnemyWaveData.WaveType.WaveType_Normal, (int)EnemyWaveData.WaveType.WaveType_Speedy);
                pendingWave.Enqueue(EnemyWaveData.GetWaveByType((EnemyWaveData.WaveType)randomWave));

                EnemyWaveData currentWave = pendingWave.Peek();
                timeLimit = timeleft = currentWave.waveDuration;

                StartCoroutine(OnSpawnEnemies());
            }
            break;

            case BattleStatus.BattleStatus_Reward:
            {
                pendingWave.Dequeue();

                timeLimit = timeleft = 10;
                // Put logic of reward, buff here
                // Give player 3 allies for now
                for(int i=0; i<3; i++) {
                    OnSpawnAllyPressed();
                }
            }
            break;

            case BattleStatus.BattleStatus_Penality:
            {
                pendingWave.Dequeue();

                timeLimit = timeleft = 3;
                // Put logic of penality, debuff here
            }
            break;

            case BattleStatus.BattleStatus_Win:
            // Actually do we have this status?
            break;

            case BattleStatus.BattleStatus_Lose:
            {
                timeLimit = timeleft = 5;
                Debug.Log("YOU LOSE");
                // Display lose UI
                statusLabel.text = "LOSE";

                resetButton.gameObject.SetActive(true);
            }
            break;

            case BattleStatus.BattleStatus_Result:
            {
                Debug.Log("RESULTS: Survived " + (numberOfWaveSurvived-1) + " waves");
                // Display results page
            }
            break;
        }
    }

    public void ResetGame() {
        Creature[] allCreatures = creatureContainer.GetComponentsInChildren<Creature>();
        for(int i=0; i<allCreatures.Length; i++) {
            ObjectPoolManager.DestroyPooled(allCreatures[i].gameObject);
        }

        BulletShell[] allBullets = projectileContainer.GetComponentsInChildren<BulletShell>();
        for(int i=0; i<allBullets.Length; i++) {
            ObjectPoolManager.DestroyPooled(allBullets[i].gameObject);
        }

        // TO-DO, spawning creature should pass to another manager which is managing battle ground. It is supposed to hold the spawning information
        Creature newPlayer = ObjectPoolManager.CreatePooled(playerPrefabList[Random.Range(0, playerPrefabList.Length)].gameObject, creatureContainer).GetComponent<Creature>();
        newPlayer.team = 0;
        newPlayer.transform.position = playerSpawnLocation.transform.position + new Vector3(Random.value * 20, Random.value * 50, Random.value * 20);

        PlayerMarker marker = GameObject.Instantiate(markerPrefab, this.creatureContainer);
        marker.trackingTarget = newPlayer.transform;

        chasingCamera.SetFollowingTarget(newPlayer.transform);

        this.SetBattleStatus(BattleStatus.BattleStatus_Ready);
    }

    public void OnSpawnAllyPressed() {
        // TO-DO, spawning creature should pass to another manager which is managing battle ground. It is supposed to hold the spawning information
        Creature newPlayer = ObjectPoolManager.CreatePooled(allyPrefabList[Random.Range(0, allyPrefabList.Length)].gameObject, creatureContainer).GetComponent<Creature>();
        newPlayer.team = 0;
        newPlayer.transform.position = playerSpawnLocation.transform.position + new Vector3(Random.value * 20, Random.value * 50, Random.value * 20);
    }

    IEnumerator OnSpawnEnemies() {
        EnemyWaveData currentWave = pendingWave.Peek();

        if(currentWave != null) {
            float power = currentWave.spawningPower;            
            float[] cost = {currentWave.meleeTypeCost, currentWave.shortRangeTypeCost, currentWave.longRangeTypeCost, currentWave.specialTypeCost, currentWave.supportTypeCost};
            int[] weights = {currentWave.genMeleeTypeRate, currentWave.genShortRangeTypeRate, currentWave.genLongRangeTypeRate, currentWave.genSpecialTypeRate, currentWave.genSupportTypeRate};
            int weightSum = 0;
            for(int i=0; i<weights.Length; i++) {
                weightSum += weights[i];
            }

            while(power > 0) {
                yield return new WaitForEndOfFrame();

                int selectedIdx = -1;

                float r = Random.value;
                float s = 0f;
                for(int i=0; i<weights.Length; i++) {
                    if(weights[i] <= 0f) continue;

                    s += (float)weights[i] / weightSum;
                    if(s >= r) {
                        selectedIdx = i;
                        break;
                    }
                }

                if(selectedIdx != -1) {
                    power -= cost[selectedIdx];

                    Creature[] creaturePrefabs = enemyPrefabDict[(GameConstants.EnemeyType)selectedIdx];
                    this.SpawnSingleEnemy(creaturePrefabs[Random.Range(0, creaturePrefabs.Length)]);                     
                }
            }
        }


    }

    private void SpawnSingleEnemy(Creature prefab) {
        // TO-DO, spawning creature should pass to another manager which is managing battle ground. It is supposed to hold the spawning information
        Creature newEnemy = ObjectPoolManager.CreatePooled(prefab.gameObject, creatureContainer).GetComponent<Creature>();
        newEnemy.team = 1;
        newEnemy.transform.position =  enemySpawnLocations[Random.Range(0, enemySpawnLocations.Length)].transform.position + Random.insideUnitSphere * 20f;
    }

    public void RegisterActiveCreature(Creature c) {
        creatureList.Add(c);
    }

    public void UnregisterActiveCreature(Creature c) {
        creatureList.Remove(c);

        switch(currentBattleStatus) {
            case BattleStatus.BattleStatus_Fighting:
            {
                // Check if all creature cleared in this wave
                List<Creature> enemyLeft = new List<Creature>();
                for(int i=0; i<creatureList.Count; i++) {
                    if(creatureList[i].currentHp > 0 && creatureList[i].team == 1) {
                        enemyLeft.Add(creatureList[i]);
                    }
                }
                if(enemyLeft.Count == 0) {
                    this.SetBattleStatus(BattleStatus.BattleStatus_Reward);
                }

            }
            break;
        }
    }

    public void PlayerCreatureDie(Creature c) {
        switch(currentBattleStatus) {
            case BattleStatus.BattleStatus_Fighting:
            case BattleStatus.BattleStatus_Penality:
            case BattleStatus.BattleStatus_Reward:
            case BattleStatus.BattleStatus_Ready:
            {
                this.SetBattleStatus(BattleStatus.BattleStatus_Lose);
            }
            break;
        }
    }
}