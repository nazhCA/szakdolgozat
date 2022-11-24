using Mirror;
using Player;
using UnityEngine;

namespace Items
{
    public class JumpBooster : MonoBehaviour
    {
        public float triggeredJumpHeight = 5f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Acquired Jump Booster");
                other.gameObject.GetComponent<ThirdPersonController>().TriggerJumpBoost(triggeredJumpHeight);
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
