using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Entry point, defines game mode and creates creatures
    /// </summary>
    public class Engine : MonoBehaviour
    {
        public bool WallToWall;
        public float Respawn;
        public Transform Transform;
        public Transform PlayerSpawnTransform;
        public List<Creature> EnemyPrefabs;
        public List<Creature> ShamanPrefabs;
        public Coin CoinPrefab;
        public FixedJoystick joystickWalk;
        public FixedJoystick joystickWeapon;

        [HideInInspector] public List<Creature> Creatures = new List<Creature>();

        public static Engine Instance;

        private int _creature;

        
        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            if (WallToWall)
            {
                CreateEnemyWallToWall();
            }
            else
            {
                CreateEnemy();
            }

            InvokeRepeating("CreateEnemyCoroutine", 0, Respawn);
        }

        public void Update()
        {
            if (WallToWall) return;

            if (Input.GetKeyDown(KeyCode.C))
            {
                SpawnTeammate();
            }
        }

        public void SpawnTeammate()
        {
            foreach (var player in Creatures.Where(i => i.State.Hp > 0 && i.GetComponent<UniversalController>()))
            {
                Destroy(player.GetComponent<UniversalController>());
                player.GetComponent<AI>().enabled = true;
            }

            var creature = Instantiate(ShamanPrefabs[Random.Range(0, ShamanPrefabs.Count)], Transform);

            creature.transform.position = PlayerSpawnTransform.position;
            creature.State.Team = 1;
            creature.State.AI = false;
            creature.gameObject.AddComponent<UniversalController>();
            Creatures.Add(creature);

            Camera.main.GetComponent<ChasingCamera>().SetFollowingTarget(creature.transform);
        }

        private void CreateEnemyCoroutine()
        {
            if (WallToWall)
            {
                CreateEnemyWallToWall();
            }
            else
            {
                CreateEnemy();
            }
        }

        private void CreateEnemy()
        {
            if (Creatures.All(i => i.State.AI)) return;

            var creature = Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)], Transform);

            creature.GetComponent<AI>().enabled = true;
            creature.transform.position = new Vector3(Random.Range(0, 2) == 0 ? -100 : 100, 16, Random.Range(-32, 48));
            creature.State.Team = 0;
        }

        private void CreateEnemyWallToWall()
        {
            var team = _creature % 2;
            var creature = Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)], Transform);
            // var pool = new[] { "Goblin", "GoblinShooter", "GoblinShaman" };
            // var random = pool[Random.Range(0, pool.Length)];
            // var color = team == 0 ? "Green" : "Red";
            // var creature = Instantiate(EnemyPrefabs.Single(i => i.name == color + random), Transform);

            creature.GetComponent<AI>().enabled = true;
            creature.transform.position = new Vector3(team == 0 ? -100 : 100, 16, Random.Range(-32, 48));
            creature.State.Team = team;
            _creature++;
        }
    }
}