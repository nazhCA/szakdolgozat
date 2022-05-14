using Mirror;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkScripts
{
    public class MyNetworkManager : NetworkManager
    {

        public InputField nameTextField;
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            
            // PlayerSettings playerSettings = conn.identity.GetComponent<PlayerSettings>();
            // playerSettings.SetDisplayName(nameTextField.text);
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            
        }

        
    }
}
