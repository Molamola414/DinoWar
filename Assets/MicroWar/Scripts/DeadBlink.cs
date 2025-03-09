using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Blink creature when die and before dispose
    /// </summary>
    public class DeadBlink : MonoBehaviour
    {
        private float _blinkTime;
        private GameObject _body;

        public void Start()
        {
            _blinkTime = Time.time + 1;
            _body = GetComponent<Creature>().Body.gameObject;
        }

        public void Update()
        {
            var passed = Time.time - _blinkTime;

            if (passed < 0) return;

            var tick = (int) (passed / 0.25f);

            _body.SetActive(tick % 2 == 1);

            if (passed > 2)
            {
                if (Random.Range(0, 4) == 0)
                {
                    var coin = Instantiate(Engine.Instance.CoinPrefab, transform.parent);
                    
                    coin.transform.position = transform.position;
                    coin.Team = GetComponent<Creature>().State.Team;
                }

                Destroy(gameObject);
            }
        }
    }
}