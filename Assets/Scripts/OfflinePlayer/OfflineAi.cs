using InputSystem;
using UnityEngine;

namespace OfflinePlayer
{
    public class OfflineAi : MonoBehaviour
    {
        private GameObject _player;
        private GameObject[] _helpingPoints;
        private GameObject _helpingPoint;
        public float minFollowDistance = 2f;
        private bool _shouldFollow = true;
        private StarterAssetsInputs _starterAssetsInputs = null;
        private float _triggerDistance;

        void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _helpingPoints = GameObject.FindGameObjectsWithTag("HelpingPoint");
            _starterAssetsInputs = gameObject.GetComponent<StarterAssetsInputs>();
        }

        void Update()
        {
            FindClosestHelpingPoint();
            FollowPlayer();
            GoToHelpingPoint();
            JumpIfNecessary();
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
        }

        private void GoToHelpingPoint()
        {
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
            Vector3 offset = new Vector3(0f, 1f, 0f);
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
    }
}