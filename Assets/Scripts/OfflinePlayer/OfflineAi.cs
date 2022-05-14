using System.Collections;
using System.Collections.Generic;
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
        private bool _canSwitchTrack = true;
        private StarterAssetsInputs _starterAssetsInputs = null;

        // Start is called before the first frame update
        void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _helpingPoints = GameObject.FindGameObjectsWithTag("HelpingPoint");
            _starterAssetsInputs = gameObject.GetComponent<StarterAssetsInputs>();

        }

        // Update is called once per frame
        void Update()
        {
            FindClosestHelpingPoint();
            FollowPlayer();
            GoToHelpingPoint();

            // if (Mathf.Abs(transform.position.x - player.transform.position.x) < 1.5f)
            // {
            //     transform.position = player.transform.position - new Vector3(1.5f, 0, 0);
            // }

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
            _starterAssetsInputs.move = Vector2.zero;
            if (Vector3.Distance(transform.position, _player.transform.position) > minFollowDistance && _shouldFollow)
            {
                if (transform.position.x - _player.transform.position.x < 0)
                {
                    _starterAssetsInputs.move = Vector2.right;
                }
                else
                {
                    _starterAssetsInputs.move = Vector2.left;
                }
                _canSwitchTrack = true;
            }

            // if (Vector3.Distance(transform.position, _player.transform.position) < 1.5f && _shouldFollow && _canSwitchTrack)
            // {
            //     SwitchTrack();
            // }
        }

        private void SwitchTrack()
        {
            transform.position -= new Vector3(0, 0, -1f);
            _canSwitchTrack = false;
        }

        private void GoToHelpingPoint()
        {
            var triggerDistance = _helpingPoint.GetComponent<HelpingPoint.HelpingPoint>().triggerDistance;
            if (Vector3.Distance(_player.transform.position, _helpingPoint.transform.position) < triggerDistance)
            {
                _shouldFollow = false;
                if (Vector3.Distance(transform.position, _helpingPoint.transform.position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(this.transform.position, _helpingPoint.transform.position, 0.05f);
                }
                return;
            }

            _shouldFollow = true;

        }
    }
}