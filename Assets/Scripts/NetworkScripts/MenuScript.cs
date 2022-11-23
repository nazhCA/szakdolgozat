using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Player;

namespace NetworkScripts
{
    public class MenuScript : MonoBehaviour
    {
        public NetworkManager networkManager;
        public GameObject menuPanel;
        public GameObject gamePanel;
        public GameObject aiPanel;
        public GameObject endPanel;
        private bool paused = false;
        public GameObject player;

        public void Host()
        {
            networkManager.StartHost();
            menuPanel.SetActive(false);
            gamePanel.SetActive(true);
            aiPanel.SetActive(true);
            paused = false;
        }
        
        public void QuitApplication()
        {
            Application.Quit();
        }

        public void SetIp(string ip)
        {
            networkManager.networkAddress = ip;
        }

        public void Join()
        {
            networkManager.StartClient();
            menuPanel.SetActive(false);
            gamePanel.SetActive(true);
            aiPanel.SetActive(false);
            paused = false;
        }

        public void Stop()
        {
            if (networkManager.mode == NetworkManagerMode.Host)
            {
                networkManager.StopHost();
            }

            if (networkManager.mode == NetworkManagerMode.ClientOnly)
            {
                networkManager.StopClient();
            }
            
            paused = false;
        }
        
        void Start()
        {
            menuPanel.SetActive(true);
            gamePanel.SetActive(false);
            aiPanel.SetActive(false);
            paused = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                paused = !paused;
                // player.GetComponent<PlayerMovement>().enabled = paused;
            }

            if (NetworkServer.active || NetworkClient.active)
            {
                gamePanel.SetActive(paused);
                
            }
        }
    }

}
