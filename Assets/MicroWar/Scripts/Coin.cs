using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Coin behaviuor
    /// </summary>
    public class Coin : MonoBehaviour
    {
        /// <summary>
        /// Creature team, prevents collecting coins from teammates
        /// </summary>
        public int Team;

        private bool _dropped;
        private bool _collected;
        private float _time;
        private Vector3 _position;

        public void Start()
        {
            _dropped = true;
            _time = Time.time;
            _position = transform.position;
            AudioPlayer.PlayEffect("DropCoin");
        }

        public void OnTriggerEnter(Collider target)
        {
            var creature = target.GetComponent<Creature>();

            if (creature == null || creature.State.Team == Team) return;

            _dropped = false;
            _collected = true;
            _time = Time.time;

            foreach (var c in GetComponents<Collider>())
            {
                Destroy(c);
            }

            Destroy(GetComponent<Rigidbody>());
            AudioPlayer.PlayEffect("CollectCoin");
        }

        public void Update()
        {
            if (_dropped)
            {
                var delta = Time.time - _time;
                var dy = delta < 0.5 ? 10 * Mathf.Sin(2 * delta * Mathf.PI) : 0;

                transform.position = new Vector3(_position.x, _position.y + dy, _position.z);
            }
            else if (_collected)
            {
                var delta = Time.time - _time;

                transform.eulerAngles = new Vector3(0, 90 + 360 * delta, 90);
                transform.position += new Vector3(0, 25 * Time.deltaTime);

                if (delta < 1)
                {
                    transform.localScale = Vector3.one * Mathf.Pow(1 - delta, 0.75f);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}