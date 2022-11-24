using System;
using System.Timers;
using InputSystem;
using Mirror;
using Player;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

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
        private EnemyHealth _enemyHealth;
        private bool _patrolLeftRight = true;
        private float _offset = 0f;
        private bool _isPatrolling = true;
        private bool _combatState = false;
        private float _lastFired = Mathf.NegativeInfinity;
        private bool _followPlayer = false;
        private RaycastHit _hit;
        private Vector3 _vectorOffset = new Vector3(0f, 1.2f, 0f);
        public bool justSpawned = false;
        public bool isAlreadyUsedShieldOrLostTheTry = false;
        public bool shieldActive = false;
        private float shieldActivateTime = 0.0f;


        private void Start()
        {
            _tpc = GetComponent<ThirdPersonController>();
            _sai = GetComponent<StarterAssetsInputs>();
            _enemyHealth = GetComponent<EnemyHealth>();
        }

        void Update()
        {
            // Debug.Log(_followPlayer.ToString());
            RunForPlayer();
            Patrol();
            SearchForPlayer();
            Combat();
            FleeIfNeeded();
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

        [Command(requiresAuthority = false)]
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
                if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Enemy"))
                {
                    _sai.jump = true;
                }
            }
        }

        void RunForPlayer()
        {
            if (justSpawned)
            {
                _sai.sprint = true;
                _sai.move = Vector2.right;
            }
        }

        void FleeIfNeeded()
        {
            if (_enemyHealth.currentHealth < 40)
            {
                if (!isAlreadyUsedShieldOrLostTheTry)
                {
                    if (Random.Range(1,10) > 5)
                    {
                        ActivateShield();
                        isAlreadyUsedShieldOrLostTheTry = true;
                        return;
                    }
                    
                    isAlreadyUsedShieldOrLostTheTry = true;
                }

                if (shieldActive)
                {
                    DeactivateShield();
                    return;
                }
                
                _sai.sprint = true;
                if (transform.position.x - GameObject.FindWithTag("Player").transform.position.x < 0)
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

        private void ActivateShield()
        {
            Debug.Log("Shield Active");
            shieldActivateTime = Time.time;
            shieldActive = true;
            gameObject.GetComponent<Renderer>().material.color = new Color(255,255,0);
        }

        private void DeactivateShield()
        {
            if (Time.time - shieldActivateTime > 3f)
            {
                Debug.Log("Shield Inactive");
                shieldActive = false;
                gameObject.GetComponent<Renderer>().material.color = Color.red;
            }
        }

        // [Command]
        // public bool ShouldActivateShield(int minInclusive, int maxInclusive, int border)
        // {
        //     return ShouldActivateShieldOnServer(minInclusive, maxInclusive, border);
        // }
        //
        // private bool ShouldActivateShieldOnServer(int minInclusive, int maxInclusive, int border)
        // {
        //     return true;
        // }
    }
}