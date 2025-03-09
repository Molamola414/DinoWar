using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Make eyes gray when die
    /// </summary>
    public class Eye : MonoBehaviour
    {
        public void OnDie()
        {
            GetComponent<MeshRenderer>().material.color = Color.grey;
        }
    }
}