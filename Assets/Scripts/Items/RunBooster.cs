using Mirror;
using Player;
using UnityEngine;

namespace Items
{
    public class RunBooster : MonoBehaviour
    {
        public float triggeredRunSpped = 10f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<ThirdPersonController>().TriggerRunBoost(triggeredRunSpped);
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
