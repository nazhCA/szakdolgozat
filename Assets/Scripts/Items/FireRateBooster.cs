using Mirror;
using Player;
using UnityEngine;

namespace Items
{
    public class FireRateBooster : NetworkBehaviour
    {
        public float triggeredFireRate = 0.1f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Acquired Fire Rate Booster");
                other.gameObject.GetComponent<ThirdPersonController>().TriggerFireRateBoost(triggeredFireRate);
                NetworkServer.Destroy(gameObject);
            }
        }
    }

}
