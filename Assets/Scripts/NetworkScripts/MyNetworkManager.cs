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
            
            Debug.Log($"OnServerConnect  {conn.identity}");
        }
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            PlayerSettings playerSettings = conn.identity.GetComponent<PlayerSettings>();

            playerSettings.SetDisplayName($"Player {numPlayers.ToString()}");

            
            Debug.Log($"OnServerAddPlayer  {conn.identity}");
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();

            
            Debug.Log("OnClientConnect");
        }

        
    }
}
