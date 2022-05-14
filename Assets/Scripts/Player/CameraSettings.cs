using Mirror;
using UnityEngine;

namespace Player
{
    public class CameraSettings : NetworkBehaviour
    {
        private Camera _mainCamera;

        private void Update()
        {
            if (!isLocalPlayer) { return; }

            _mainCamera.transform.position = transform.position + new Vector3(0, 1, -10);
        }

        public override void OnStartLocalPlayer()
        {

            base.OnStartLocalPlayer();

            _mainCamera = Camera.main;
        }
    }
    
}
