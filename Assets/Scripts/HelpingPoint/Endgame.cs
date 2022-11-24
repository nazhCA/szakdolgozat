using Mirror;
using NetworkScripts;
using OfflinePlayer;
using Player;
using UnityEngine;

namespace HelpingPoint
{
    
    public class Endgame : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {

                var remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

                if (remainingEnemies.Length > 0)
                {
                    return;
                }
                
                Debug.Log(remainingEnemies.Length.ToString());

                LoadEndScreen();
                TellServerToDisableActions();

            }
        }

        [Command(requiresAuthority = false)]
        private void TellServerToDisableActions()
        {
            DisableActionsOnServer();
        }

        [ClientRpc]
        public void DisableActionsOnServer()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            var offlineAi = GameObject.FindWithTag("OfflinePlayer");
            
            foreach (var player in players)
            {
                player.GetComponent<ThirdPersonController>().enabled = false;
                player.GetComponent<Animator>().enabled = false;
                player.GetComponent<MoveAiToCursor>().enabled = false;
            }
            
            foreach (var enemy in enemies)
            {
                enemy.GetComponent<ThirdPersonController>().enabled = false;
                enemy.GetComponent<Animator>().enabled = false;
            }

            if (offlineAi != null)
            {
                offlineAi.GetComponent<ThirdPersonController>().enabled = false;
                offlineAi.GetComponent<Animator>().enabled = false;
                offlineAi.GetComponent<MoveAiToCursor>().enabled = false;
            }
            
        }

        public void LoadEndScreen()
        {
            var menuscript = GameObject.FindWithTag("NetworkManager").GetComponent<MenuScript>();
            menuscript.endPanel.SetActive(true);

        }
    }
}