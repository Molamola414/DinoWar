using UnityEngine;

namespace Assets.MicroWar.Scripts
{
    /// <summary>
    /// Prevents creatures overlap and creates opposite "force"
    /// </summary>
    public class BodyBox : MonoBehaviour
    {
        public void OnTriggerStay(Collider target)
        {
            var offset = Mathf.Sign(target.transform.position.z - transform.position.z) / 10;

            transform.parent.position -= new Vector3(0, 0, offset);
        }
    }
}