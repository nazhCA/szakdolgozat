using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        private Camera _mainCamera;
        private CharacterController _controller;
        private Vector3 _playerVelocity;
        private bool _groundedPlayer;
        [SerializeField] private float playerSpeed = 7.0f;
        [SerializeField] private float jumpHeight = 2.0f;
        [SerializeField] private float gravityValue = -15.81f;

        #region Server

        
        #endregion


        #region Client

        void Update()
        {
            if (isLocalPlayer)
            {
                _groundedPlayer = _controller.isGrounded;

                if (_groundedPlayer && _playerVelocity.y < 0f)
                {
                    _playerVelocity.y = -2f;
                }

                if (Input.GetAxis("Horizontal") != 0)
                {
                    _controller.Move(new Vector3(Input.GetAxis("Horizontal"), 0, 0) * Time.deltaTime * playerSpeed);
                }

                if (Input.GetButtonDown("Jump") && _groundedPlayer)
                {
                    _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                }

                _playerVelocity.y += gravityValue * Time.deltaTime;
                _controller.Move(_playerVelocity * Time.deltaTime);

                _mainCamera.transform.position = transform.position + new Vector3(0, 1, -10);
            }
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            _controller = gameObject.AddComponent<CharacterController>();
            _controller.height = 1f;
            _controller.minMoveDistance = 0f;
        }
        
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();


            _mainCamera = Camera.main;
        }
        
        #endregion
    }
}