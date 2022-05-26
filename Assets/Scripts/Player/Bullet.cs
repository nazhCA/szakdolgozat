using Enemy;
using Mirror;
using Telepathy;
using UnityEngine;

namespace Player
{
    public class Bullet : NetworkBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            GameObject hit = collision.gameObject;
            EnemyHealth health = hit.GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.TakeDamage(10);
            }
        
            Destroy(gameObject);
        }

        // [Command]
        // private void CmdDestroyBullet()
        // {
        //     Destroy(gameObject);
        // }
    }
}
