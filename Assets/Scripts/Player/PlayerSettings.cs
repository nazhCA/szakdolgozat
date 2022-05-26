using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerSettings : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleHealthDisplayUpdate))] [SerializeField] public float health = 20f;
        [SerializeField] private TMP_Text healthDisplay = null;
        [SerializeField] private TMP_Text displayNameText = null;
        [SyncVar(hook = nameof(HandleDisplayNameUpdate))] [SerializeField] public string displayName = "Unknown";

        public GameObject offlinePlayer = null;
        private GameObject _child;
        

        #region Client

        private void Update()
        {
            _child.transform.rotation = Quaternion.Euler (0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
        }

        private void Start()
        {
            _child = transform.GetChild(0).gameObject;

        }

        [Client]
        public override void OnStartClient()
        {
            base.OnStartClient();

            healthDisplay.text = health.ToString();

            if (isServer && isLocalPlayer)
            {
                SpawnOfflinePlayer();
            }

            if (!isServer && isLocalPlayer)
            {
                DespawnOfflinePlayer();
            }
            
        }

        [ContextMenu("ChangeName")]
        private void ChangeName()
        {
            displayName = "kaka";
        }

        [Client]
        public override void OnStopClient()
        {
            DespawnOfflinePlayer();
            
            base.OnStopClient();
        }

        private void HandleHealthDisplayUpdate(float oldText, float newText)
        {
            healthDisplay.text = newText.ToString();
        }
        
        private void HandleDisplayNameUpdate(string oldName, string newName)
        {
            displayNameText.text = newName;
        }
        
        // Manually damage player via context menu, client call
        [ContextMenu("DamagePlayer")]
        private void DamagePlayer()
        {

            // Call a server function on client
            CmdDamagePlayer(10f);
            
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Enemy") && isLocalPlayer)
            {
                DamagePlayer();
            }
        }

        #endregion
        
        #region Server

        [Command]
        private void SpawnOfflinePlayer()
        {
            Instantiate(offlinePlayer, transform.position - new Vector3(2f,0f,0f), Quaternion.identity);
        }

        [Command]
        private void DespawnOfflinePlayer()
        {
            GameObject offline = GameObject.FindWithTag("OfflinePlayer");
            Destroy(offline);
        }
        
        [Command]
        private void CmdDamagePlayer(float damage)
        {
            health -= damage;
            
            // Call a server function from server
            CheckIfPlayerIsDead();
        }
        
        [Server]
        private void CheckIfPlayerIsDead()
        {
            if (health <= 0)
            {
                TellServerToDestroyObject(gameObject);
            }
        }
        
        [Server]
        public void TellServerToDestroyObject(GameObject gObject)
        {
            CmdDestroyObject(gObject);
        }
        
        [Server]
        private void CmdDestroyObject(GameObject gObject)
        {
            NetworkServer.Destroy(gObject);
        }

        [Client]
        public void SetDisplayName(string newName)
        {
            displayName = newName;
        }

        #endregion

    }
}
