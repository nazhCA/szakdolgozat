using System;
using System.Collections;
using System.Collections.Generic;
using InputSystem;
using Mirror;
using NSubstitute;
using UnityEngine;
using UnityEngine.AI;

namespace OfflinePlayer
{
    public class MoveAiToCursor : NetworkBehaviour
    {
        private StarterAssetsInputs _starterAssetsInputs = null;
        private StarterAssetsInputs _offlinePlayerSai = null;
        private GameObject _offlinePlayer;
        private GameObject _player;
        private Vector3 _moveToPosition;
        private OfflineAi _offlineAi;
        private GameObject _target;

        public Vector3 MoveToPosition
        {
            get => _moveToPosition;
            set => _moveToPosition = value;
        }
        
        void Start()
        {

            try
            {
                _offlinePlayer = GameObject.FindWithTag("OfflinePlayer");
                _offlinePlayerSai = _offlinePlayer.GetComponent<StarterAssetsInputs>();
                _offlinePlayerSai = _offlinePlayer.GetComponent<StarterAssetsInputs>();
                _offlineAi = _offlinePlayer.GetComponent<OfflineAi>();
            }
            catch (Exception e)
            {
                // TellServerToDisableMoveAiToCursor();
                Debug.Log("There is no offline player");
            }
            _starterAssetsInputs = gameObject.GetComponent<StarterAssetsInputs>();
            _player = GameObject.FindWithTag("Player");

        }

        // [Client]
        // public void TellServerToDisableMoveAiToCursor()
        // {
        //     DisableMoveAiToCursor();
        // }
        //
        // [ClientRpc]
        // public void DisableMoveAiToCursor()
        // {
        //     GetComponent<MoveAiToCursor>().gameObject.SetActive(false);
        // }
        
        void Update()
        {
            if (_offlineAi == null)
            {
                return;
            }
            MoveAi();
            KillEnemy();
        }

        private void MoveAi()
        {
            if (_starterAssetsInputs.moveAi)
            {
                _offlineAi.shouldFollow = false;
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    if (hit.collider.gameObject.CompareTag("MoveAiPlane"))
                    {
                        _moveToPosition = hit.point;
                    }

                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        _target = hit.collider.gameObject;
                    }
                }
                Debug.DrawLine(Camera.main.transform.position, hit.point);
            }
            if (_moveToPosition != Vector3.zero)
            {
                _offlinePlayerSai.sprint = true;
                _offlineAi.Target = null;
                MakeAiRunToDesignatedPoint(_moveToPosition);
            }
        }

        private void KillEnemy()
        {
            if(!_target) { return; }
            
            _offlineAi.Target = _target;
            _offlineAi.combatState = true;
            _offlinePlayerSai.sprint = true;
            if (DefineDirectionAndRunToEnemy(_target))
            {
                _offlineAi.Combat();
                _offlineAi.JumpIfNecessary();
            }

        }

        private bool DefineDirectionAndRunToEnemy(GameObject enemy)
        {
            if (Vector3.Distance(_player.transform.position, enemy.transform.position) > 15f)
            {
                _offlineAi.Target = null;
                _target = null;
                _offlineAi.shouldFollow = true;
                _offlineAi.combatState = false;
            }
            
            if (Vector3.Distance(transform.position, enemy.transform.position) < 6f)
            {
                _offlinePlayerSai.move = Vector2.zero;
                return true;
            }
            
            // if (enemy && transform.position.x < enemy.transform.position.x)
            // {
            //     _offlinePlayerSai.move = Vector2.right;
            // }
            //
            // if (enemy && transform.position.x > enemy.transform.position.x)
            // {
            //     _offlinePlayerSai.move = Vector2.left;
            // }
            
            return false;
        }

        private void MakeAiRunToDesignatedPoint(Vector3 designatedPoint)
        {
            _offlineAi.combatState = false;
            if (_offlinePlayer.transform.position.x < designatedPoint.x - 1f)
            {
                _offlineAi.shouldFollow = false;
                _offlinePlayerSai.sprint = true;
                _offlinePlayerSai.move = Vector2.right;
                _offlineAi.JumpIfNecessary();
                return;
            }
            
            if (_offlinePlayer.transform.position.x > designatedPoint.x + 1f)
            {
                _offlineAi.shouldFollow = false;
                _offlinePlayerSai.sprint = true;
                _offlinePlayerSai.move = Vector2.left;
                _offlineAi.JumpIfNecessary();
                return;
            }
            _moveToPosition = Vector3.zero;
            _offlineAi.SetBehaviourStay();
            _offlineAi.shouldFollow = true;
        }
    }
}
