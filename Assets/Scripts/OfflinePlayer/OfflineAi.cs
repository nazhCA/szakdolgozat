using System.Collections.Generic;
using InputSystem;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace OfflinePlayer
{
    public class OfflineAi : MonoBehaviour
    {
        
        public enum OfflinePlayerBehaviour
        {
            Passive = 1,
            Stay = 0,
            Active = 2,
            Aggressive = 3
        }
        public float minFollowDistance = 2f;
        
        [SerializeField]
        public static OfflinePlayerBehaviour offlinePlayerBehaviour;
        
        private GameObject _player;
        private GameObject[] _helpingPoints;
        private GameObject _helpingPoint;
        private bool _shouldFollow = true;
        private StarterAssetsInputs _starterAssetsInputs = null;
        private StarterAssetsInputs _playerSai = null;
        private float _triggerDistance;
        public bool combatState;
        private float _lastFired = Mathf.NegativeInfinity;
        public GameObject bulletPrefab;
        public Transform bulletSpawn = null;
        private Vector3 _vectorOffset = new Vector3(0f, 1.2f, 0f);
        private GameObject _target;
        

        void Start()
        {
            Debug.Log(offlinePlayerBehaviour);
            _player = GameObject.FindWithTag("Player");
            _helpingPoints = GameObject.FindGameObjectsWithTag("HelpingPoint");
            _starterAssetsInputs = gameObject.GetComponent<StarterAssetsInputs>();
            _playerSai = _player.GetComponent<StarterAssetsInputs>();
        }

        void Update()
        {
            switch (offlinePlayerBehaviour)
            {
                case OfflinePlayerBehaviour.Passive:
                    FollowPlayer();
                    JumpIfNecessary();
                    break;
                case OfflinePlayerBehaviour.Active:
                    FollowPlayer();
                    FindClosestHelpingPoint();
                    GoToHelpingPoint();
                    JumpIfNecessary();
                    break;
                case OfflinePlayerBehaviour.Aggressive:
                    FollowPlayer();
                    WatchingBackwards();
                    FindClosestHelpingPoint();
                    GoToHelpingPoint();
                    JumpIfNecessary();
                    break;
                default:
                    _starterAssetsInputs.move = Vector2.zero;
                    break;
            }

        }

        private void FindClosestHelpingPoint()
        {
            GameObject closest = null;
            float distance = 1000;
            foreach (var helpingPoint in _helpingPoints)
            {
                float tmpDistance = Vector3.Distance(_player.transform.position, helpingPoint.transform.position);

                if (tmpDistance < distance)
                {
                    distance = tmpDistance;
                    closest = helpingPoint;
                }
            }
            _helpingPoint = closest;
        }

        private void FollowPlayer()
        {
            _starterAssetsInputs.sprint = true;
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance > minFollowDistance - 0.5f && distance < minFollowDistance + 0.5f)
            {
                _starterAssetsInputs.sprint = false;
            }
            _starterAssetsInputs.move = Vector2.zero;
            if (distance > minFollowDistance && _shouldFollow)
            {
                if (transform.position.x - _player.transform.position.x < 0)
                {
                    _starterAssetsInputs.move = Vector2.right;
                }
                else
                {
                    _starterAssetsInputs.move = Vector2.left;
                }
            }

            if (distance < 1f)
            {
                WalkWithPlayer();
            }
        }

        private void GoToHelpingPoint()
        {
            Debug.Log("first line");
            if (combatState)
            {
                return;
            }
            Debug.Log("second line");
            _triggerDistance = _helpingPoint.GetComponent<HelpingPoint.HelpingPoint>().triggerDistance;
            if (Vector3.Distance(_player.transform.position, _helpingPoint.transform.position) < _triggerDistance)
            {
                _shouldFollow = false;
                if (Vector3.Distance(transform.position, _helpingPoint.transform.position) > 0.5f)
                {
                    if (transform.position.x - _helpingPoint.transform.position.x < 0)
                    {
                        _starterAssetsInputs.move = Vector2.right;
                    }
                    else
                    {
                        _starterAssetsInputs.move = Vector2.left;
                    }
                }
                else
                {
                    _starterAssetsInputs.move = Vector2.zero;
                }
                
                return;
            }
            _shouldFollow = true;
        }

        private void JumpIfNecessary()
        {
            Vector3 offset = new Vector3(0f, 0.8f, 0f);
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, transform.forward, out hit, 3f))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    Jump();
                }

                if (hit.collider.CompareTag("Player") && !_shouldFollow)
                {
                    Jump();
                }
            }
            Debug.DrawLine(transform.position + offset, hit.point);
        }

        private void Jump()
        {
            _starterAssetsInputs.jump = true;
        }

        private void WalkWithPlayer()
        {
            if (transform.position.x - _player.transform.position.x > 0)
            {
                _starterAssetsInputs.move = Vector2.right;
            }
            else
            {
                _starterAssetsInputs.move = Vector2.left;
            }
        }

        void WatchingBackwards()
        {
            
            RaycastHit hit;
           if (!combatState && Physics.Raycast(transform.position + _vectorOffset, -transform.forward, out hit, 8f))
           {
               if (hit.collider.CompareTag("Enemy"))
               {
                   _shouldFollow = false;
                   _target = hit.collider.gameObject;
                   transform.rotation = Quaternion.Inverse(transform.rotation);
                   combatState = true;
               }
               
            }

            Combat();
        }

        void Combat()
        {
            if (combatState)
            {
                if (Time.timeSinceLevelLoad - _lastFired > 0.3f && combatState)
                {
                    _starterAssetsInputs.move = Vector2.zero;
                    Fire();
                    _lastFired = Time.timeSinceLevelLoad;
                }

                if (!_target)
                {
                    _shouldFollow = true;
                    combatState = false;
                }
            }
        }
        
        void Fire()
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().velocity = transform.forward * 8f;
            NetworkServer.Spawn(bullet);
            Destroy(bullet, 2);
        }

        public void SetBehaviourPassive()
        {
            offlinePlayerBehaviour = OfflinePlayerBehaviour.Passive;
            Debug.Log("clicked " + offlinePlayerBehaviour);
        }
        
        public void SetBehaviourStay()
        {
            offlinePlayerBehaviour = OfflinePlayerBehaviour.Stay;
            Debug.Log("clicked " + offlinePlayerBehaviour);
        }
        
        public void SetBehaviourAggressive()
        {
            offlinePlayerBehaviour = OfflinePlayerBehaviour.Aggressive;
            Debug.Log("clicked " + offlinePlayerBehaviour);
        }
    }
}