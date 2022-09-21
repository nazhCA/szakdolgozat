using System;
using System.Timers;
using InputSystem;
using Mirror;
using Player;
using UnityEditor;
using UnityEngine;

namespace Enemy
{
    public class EnemyCombat : NetworkBehaviour
    {
        public bool enablePatrolling = false;
        public float patrolChangeDirection = 2f;
        public GameObject bulletPrefab;
        public Transform bulletSpawn = null;
        
        private ThirdPersonController _tpc;
        private StarterAssetsInputs _sai;
        private bool _patrolLeftRight = true;
        private float _offset = 0f;
        private bool _isPatrolling = true;
        private bool _combatState = false;
        private float _lastFired = Mathf.NegativeInfinity;
        private bool _followPlayer = false;
        private RaycastHit _hit;
        private Vector3 _vectorOffset = new Vector3(0f, 1.2f, 0f);
        public bool justSpawned = false;
        


        private void Start()
        {
            _tpc = GetComponent<ThirdPersonController>();
            _sai = GetComponent<StarterAssetsInputs>();

        }

        void Update()
        {
            // Debug.Log(_followPlayer.ToString());
            RunForPlayer();
            Patrol();
            SearchForPlayer();
            Combat();
        }

        public void Patrol()
        {
            // if (isLocalPlayer)
            // {
            //     return;
            // }

            if (_isPatrolling && enablePatrolling)
            {
                if (Time.timeSinceLevelLoad - _offset > patrolChangeDirection)
                {
                    _patrolLeftRight = !_patrolLeftRight;
                    _offset = Time.timeSinceLevelLoad;
                }

                if (_patrolLeftRight)
                {
                    _sai.move = Vector2.right;
                }
                else
                {
                    _sai.move = Vector2.left;
                }

            }
            else
            {
                if (!justSpawned)
                {
                    _sai.move = Vector2.zero;
                }
            }
            
        }

        void Combat()
        {
            if (_combatState)
            {
                if (Time.timeSinceLevelLoad - _lastFired > 1.5f && _combatState)
                {
                    enablePatrolling = false;
                    _sai.move = Vector2.zero;
                    CmdFire();   
                    _lastFired = Time.timeSinceLevelLoad;
                }
            }
        }
        
        void CmdFire()
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().velocity = transform.forward * 8f;
            NetworkServer.Spawn(bullet);
            Destroy(bullet, 2);
        }
        
        public void SearchForPlayer()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + _vectorOffset, transform.forward, out hit, 6f) &&
                (hit.collider.CompareTag("Player") || hit.collider.CompareTag("OfflinePlayer")))
            {
                _hit = hit;
                _combatState = true;
                _followPlayer = true;
                justSpawned = false;
            } 
            else if (_followPlayer)
            {
                if (_hit.transform && Vector3.Distance(transform.position, _hit.transform.position) > 2f)
                {
                    FollowPlayer();
                }
                else
                {
                    _combatState = false;
                    _followPlayer = false;
                    _isPatrolling = true;
                    enablePatrolling = true;
                }
            }
        }

        void FollowPlayer()
        {
            if (_followPlayer)
            {
                _sai.sprint = true;
                if (transform.position.x - _hit.collider.transform.position.x > 0)
                {
                    _sai.move = Vector2.left;
                }
                else
                {
                    _sai.move = Vector2.right;
                }
                
                JumpIfNeccessary();
            }
        }

        void JumpIfNeccessary()
        {
            Vector3 offset = new Vector3(0f, 1f, 0f);
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, transform.forward, out hit, 3f))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    _sai.jump = true;
                }
            }
        }

        void RunForPlayer()
        {
            if (justSpawned)
            {
                Debug.Log("moving right");
                _sai.sprint = true;
                _sai.move = Vector2.right;
            }
            
        }
        
    }
    
}
