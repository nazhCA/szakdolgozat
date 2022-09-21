using System;
using Enemy;
using Mirror;
using NetworkScripts;
using OfflinePlayer;
using UnityEngine;

namespace Player
{
    public class Bullet : NetworkBehaviour
    {
        public GameObject networkManager = null;
        private MyNetworkManager _mynetworkManager = null;

        private bool _friendlyFire;
        private int _enemyDamage;
        private int _playerDamage;


        public void Start()
        {
            _mynetworkManager = networkManager.GetComponent<MyNetworkManager>();
        }
        
        public void Update()
        {
            _friendlyFire = _mynetworkManager.friendlyFire;
            _enemyDamage = _mynetworkManager.enemyDamage;
            _playerDamage = _mynetworkManager.playerDamage;
        }

        private void OnCollisionEnter(Collision collision)
        {
            GameObject hit = collision.gameObject;
            if (hit.tag.Equals("Enemy"))
            {
                EnemyHealth health = hit.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.TakeDamage(_playerDamage);
                }
            }

            if (hit.tag.Equals("Player") && _friendlyFire)
            {
                PlayerSettings health = hit.GetComponent<PlayerSettings>();
                if (health != null)
                {
                    health.DamagePlayer(_enemyDamage);
                }
            }
            
            if (hit.tag.Equals("OfflinePlayer") && _friendlyFire)
            {
                PlayerSettings health = hit.GetComponent<PlayerSettings>();
                if (health != null)
                {
                    health.DamageOfflinePlayer(_enemyDamage);
                }
            }
            
            Destroy(gameObject);
            
        }

    }
}
