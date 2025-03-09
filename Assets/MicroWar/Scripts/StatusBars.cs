using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Status bars, health and mana
    /// </summary>
    public class StatusBars : MonoBehaviour
    {
        [Header("Bars")]
        public Transform Hp;
        public Transform HpEmpty;
        public Transform Mp;
        public Transform MpEmpty;

        [Header("Materials")]
        public Material HpMat;
        public Material HpEmptyMat;
        public Material HpPoisonedMat;
        public Material HpEmptyPoisonedMat;

        public void Start()
        {
            Hp.localScale = new Vector3(1, 1, 1);
            HpEmpty.localScale = new Vector3(1, 0, 1);

            if (Mp != null)
            {
                Mp.localScale = new Vector3(1, 1, 1);
                MpEmpty.localScale = new Vector3(1, 0, 1);
            }
        }

        /// <summary>
        /// Set health value
        /// </summary>
        public void SetHp(float hp, float max)
        {
            var value = hp / max;

            if (value < 0) value = 0;
            else if (value > max) value = max;

            Hp.localScale = new Vector3(1, value, 1);
            HpEmpty.localScale = new Vector3(1, 1 - value, 1);
        }

        /// <summary>
        /// Set mana value
        /// </summary>
        public void SetMp(float mp, float max)
        {
            var value = mp / max;

            if (value < 0) value = 0;
            else if (value > max) value = max;

            Mp.localScale = new Vector3(1, value, 1);
            MpEmpty.localScale = new Vector3(1, 1 - value, 1);
        }

        /// <summary>
        /// Enable bars for AI controlled creatures, when first hited
        /// </summary>
        public void Enable(bool hp, bool mp)
        {
            Hp.gameObject.SetActive(hp);
            HpEmpty.gameObject.SetActive(hp);

            if (Mp != null)
            {
                Mp.gameObject.SetActive(mp);
                MpEmpty.gameObject.SetActive(mp);
            }
        }

        /// <summary>
        /// True if current creature is poisoned
        /// </summary>
        public bool Poisoned
        {
            set
            {
                Hp.GetComponent<MeshRenderer>().sharedMaterial = value ? HpPoisonedMat : HpMat;
                HpEmpty.GetComponent<MeshRenderer>().sharedMaterial = value ? HpEmptyPoisonedMat : HpEmptyMat;
            }
        }
    }
}