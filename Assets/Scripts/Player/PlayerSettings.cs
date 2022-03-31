using Mirror;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerSettings : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleHealthDisplayUpdate))] [SerializeField] public float health = 100f;
        [SerializeField] private TMP_Text healthDisplay = null;
        [SerializeField] private TMP_Text displayNameText = null;
        [SyncVar(hook = nameof(HandleDisplayNameUpdate))] [SerializeField] public string displayName = "Unknown";

        

        #region Client
        
        [Client]
        public override void OnStartClient()
        {
            base.OnStartClient();

            healthDisplay.text = health.ToString();
        }
        
        private void HandleHealthDisplayUpdate(float oldText, float newText)
        {
            healthDisplay.text = newText.ToString();
        }
        
        private void HandleDisplayNameUpdate(string oldName, string newName)
        {
            displayNameText.text = newName;
            Debug.Log("HandleDisplayNameUpdate");
        }
        
        // Manually damage player via context menu, client call
        [ContextMenu("DamagePlayer")]
        private void DamagePlayer()
        {

            // Call a server function on client
            CmdDamagePlayer(10f);
            
        }
        
        
        #endregion
        
        #region Server
        
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

        [Server]
        public void SetDisplayName(string newName)
        {
            displayName = newName;
        }
        
        #endregion

    }
}
