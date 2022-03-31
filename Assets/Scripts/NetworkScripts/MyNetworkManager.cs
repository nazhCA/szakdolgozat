using Mirror;
using Player;
using UnityEngine;

namespace NetworkScripts
{
    public class MyNetworkManager : NetworkManager
    {
        
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            
        }
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            PlayerSettings playerSettings = conn.identity.GetComponent<PlayerSettings>();

            playerSettings.SetDisplayName($"Player {numPlayers.ToString()}");

            
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();

            
        }

        
    }
}
