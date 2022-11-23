using Mirror;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkScripts
{
    public class MyNetworkManager : NetworkManager
    {
        [Header("Game Settings")]
        [Tooltip("Decides whether or not the player takes damage if shot by teammate.")]
        public bool friendlyFire = false;
        [Tooltip("Amount of damage the enemy inflicts upon one shot.")]
        public int enemyDamage = 10;
        [Tooltip("Amount of damage the player inflicts upon one shot.")]
        public int playerDamage = 10;
        
        public InputField nameTextField;
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            
            base.OnServerAddPlayer(conn);
            PlayerSettings playerSettings = conn.identity.GetComponent<PlayerSettings>();
            playerSettings.SetDisplayName($"Player {numPlayers.ToString()}");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log("Kliens lecsatlakozott");
        }
    }
}
