using System.Collections.Generic;
using InputSystem;
using JetBrains.Annotations;
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
            Aggressive = 3,
            None = 4
        }
        public float minFollowDistance = 2f;
        
        [SerializeField]
        public static OfflinePlayerBehaviour offlinePlayerBehaviour;

        private GameObject _player;
        private GameObject[] _helpingPoints;
        private GameObject _helpingPoint;
        public bool shouldFollow = true;
        private StarterAssetsInputs _starterAssetsInputs = null;
        private StarterAssetsInputs _playerSai = null;
        private float _triggerDistance;
        public bool combatState;
        private float _lastFired = Mathf.NegativeInfinity;
        public GameObject bulletPrefab;
        public Transform bulletSpawn = null;
        private Vector3 _vectorOffset = new Vector3(0f, 1.2f, 0f);
        private GameObject _target = null;
        private MoveAiToCursor _moveAiToCursor;

        public GameObject Target
        {
            get => _target;
            set => _target = value;
        }


        void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _helpingPoints = GameObject.FindGameObjectsWithTag("HelpingPoint");
            _starterAssetsInputs = gameObject.GetComponent<StarterAssetsInputs>();
            _playerSai = _player.GetComponent<StarterAssetsInputs>();
            _moveAiToCursor = GetComponent<MoveAiToCursor>();
        }

        void Update()
        {
            switch (offlinePlayerBehaviour)
            {
                case OfflinePlayerBehaviour.Stay:
                    _starterAssetsInputs.move = Vector2.zero;
                    break;
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
                    // FollowPlayer();
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
            if (distance > minFollowDistance && shouldFollow)
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
            Debug.Log(combatState.ToString());
            if (combatState)
            {
                return;
            }
            _triggerDistance = _helpingPoint.GetComponent<HelpingPoint.HelpingPoint>().triggerDistance;
            if (Vector3.Distance(_player.transform.position, _helpingPoint.transform.position) < _triggerDistance)
            {
                shouldFollow = false;
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
            shouldFollow = true;
        }

        public void JumpIfNecessary()
        {
            Vector3 offset = new Vector3(0f, 0.8f, 0f);
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, transform.forward, out hit, 3f))
            {
                if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Player") && !shouldFollow)
                {
                    Jump();
                }
            }
            Debug.DrawLine(transform.position + offset, hit.point, Color.green);
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
            if ((_target == null) && !combatState && Physics.Raycast(transform.position + _vectorOffset, -transform.forward, out hit, 8f))
            {
               Debug.DrawLine(transform.position, hit.collider.gameObject.transform.position, Color.magenta);
               if (hit.collider.CompareTag("Enemy"))
               {
                   shouldFollow = false;
                   _target = hit.collider.gameObject;
                   transform.rotation = Quaternion.Inverse(transform.rotation);
                   combatState = true;
               }
               
            }

            Combat();
        }

        public void Combat()
        {
            if (_target == null)
            {
                shouldFollow = true;
                combatState = false;
                return;
            }
            
            if (combatState)
            {

                if ( Vector3.Distance(_target.transform.position, transform.position) > 6f)
                {
                    if (_target.transform.position.x > transform.position.x )
                    {
                        _starterAssetsInputs.move = Vector2.right;
                    }
                    else
                    {
                        _starterAssetsInputs.move = Vector2.left;
                    }

                    return;
                }
                
                

                if (Time.timeSinceLevelLoad - _lastFired > 0.3f && combatState)
                {
                    _starterAssetsInputs.move = Vector2.zero;
                    Fire();
                    _lastFired = Time.timeSinceLevelLoad;
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
            GameObject aggressiveButton = GameObject.Find("AggressiveButton");
            GameObject passiveButton = GameObject.Find("PassiveButton");
            GameObject stayButton = GameObject.Find("StayButton");
            GameObject activeButton = GameObject.Find("ActiveButton");
            passiveButton.GetComponent<Image>().color = Color.yellow;
            stayButton.GetComponent<Image>().color = Color.white;
            aggressiveButton.GetComponent<Image>().color = Color.white;
            activeButton.GetComponent<Image>().color = Color.white;
            _moveAiToCursor.MoveToPosition = Vector3.zero;
        }
        
        public void SetBehaviourActive()
        {
            offlinePlayerBehaviour = OfflinePlayerBehaviour.Active;
            GameObject aggressiveButton = GameObject.Find("AggressiveButton");
            GameObject passiveButton = GameObject.Find("PassiveButton");
            GameObject stayButton = GameObject.Find("StayButton");
            GameObject activeButton = GameObject.Find("ActiveButton");
            passiveButton.GetComponent<Image>().color = Color.white;
            stayButton.GetComponent<Image>().color = Color.white;
            aggressiveButton.GetComponent<Image>().color = Color.white;
            activeButton.GetComponent<Image>().color = Color.green;
            _moveAiToCursor.MoveToPosition = Vector3.zero;
        }
        
        public void SetBehaviourStay()
        {
            offlinePlayerBehaviour = OfflinePlayerBehaviour.Stay;
            GameObject aggressiveButton = GameObject.Find("AggressiveButton");
            GameObject passiveButton = GameObject.Find("PassiveButton");
            GameObject stayButton = GameObject.Find("StayButton");
            GameObject activeButton = GameObject.Find("ActiveButton");
            passiveButton.GetComponent<Image>().color = Color.white;
            stayButton.GetComponent<Image>().color = Color.grey;
            aggressiveButton.GetComponent<Image>().color = Color.white;
            activeButton.GetComponent<Image>().color = Color.white;
        }
        
        public void SetBehaviourAggressive()
        {
            offlinePlayerBehaviour = OfflinePlayerBehaviour.Aggressive;
            GameObject aggressiveButton = GameObject.Find("AggressiveButton");
            GameObject passiveButton = GameObject.Find("PassiveButton");
            GameObject stayButton = GameObject.Find("StayButton");
            GameObject activeButton = GameObject.Find("ActiveButton");
            passiveButton.GetComponent<Image>().color = Color.white;
            stayButton.GetComponent<Image>().color = Color.white;
            aggressiveButton.GetComponent<Image>().color = Color.red;
            activeButton.GetComponent<Image>().color = Color.white;
            _moveAiToCursor.MoveToPosition = Vector3.zero;
        }

        public void SetBehaviorNone()
        {
            offlinePlayerBehaviour = OfflinePlayerBehaviour.None;
            GameObject aggressiveButton = GameObject.Find("AggressiveButton");
            GameObject passiveButton = GameObject.Find("PassiveButton");
            GameObject stayButton = GameObject.Find("StayButton");
            GameObject activeButton = GameObject.Find("ActiveButton");
            passiveButton.GetComponent<Image>().color = Color.white;
            stayButton.GetComponent<Image>().color = Color.white;
            aggressiveButton.GetComponent<Image>().color = Color.white;
            activeButton.GetComponent<Image>().color = Color.white;
        }
    }
}