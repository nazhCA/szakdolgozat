using System.Collections;
using System.Collections.Generic;
using Enemy;
using InputSystem;
using Mirror;
using UnityEngine;


namespace Player
{
    public class SpawnTriggerOnPlayer : NetworkBehaviour
    {
        public GameObject enemyPrefab;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("SpawnTrigger"))
            {
                SpawnTrigger.SpawnTrigger spawnTrigger = other.gameObject.GetComponent<SpawnTrigger.SpawnTrigger>();
                if (!spawnTrigger.used)
                {
                    other.gameObject.GetComponent<NetworkIdentity>()
                        .AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
                    CmdSpawnEnemy();

                    spawnTrigger.used = true;
                }
            }
        }


        [Command]
        public void CmdSpawnEnemy()
        {
            Vector3 offset = new Vector3(-10, 0, 0);
            GameObject enemy = Instantiate(enemyPrefab, transform.position + offset, transform.rotation);
            enemy.GetComponent<EnemyCombat>().justSpawned = true;
            NetworkServer.Spawn(enemy);
        }
    }
}