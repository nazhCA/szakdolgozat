using Mirror;
using TMPro;
using UnityEngine;

namespace Enemy
{
    public class EnemyHealth : NetworkBehaviour
    {
        private const int MaxHealth = 100;
        [SyncVar (hook = nameof(OnChangeHealth))] public int currentHealth = MaxHealth;
        [SerializeField] private TMP_Text healthDisplay = null;

        public GameObject fireRateBooster = null;
        public GameObject jumpBooster = null;
        public GameObject runBooster = null;

        public void TakeDamage(int amount)
        {
            if (!isServer)
            {
                return;
            }

            EnemyCombat enemyCombat = GetComponent<EnemyCombat>();
            
            if (enemyCombat.shieldActive)
            {
                return;
            }

            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Destroy(gameObject);
                MayDropBooster();
            }

        }

        [Command(requiresAuthority = false)]
        private void MayDropBooster()
        {
            if (Random.Range(1,100) > 80)
            {
                GameObject booster = GiveRandomBooster();
                if (booster == null)
                {
                    return;
                }
                
                booster.transform.position += new Vector3(0, .25f, 0);
                NetworkServer.Spawn(booster);
            }
        }

        private GameObject GiveRandomBooster()
        {
            int random = Random.Range(1, 99);
            int result = random / 33;
            
            switch (result)
            {
                case 0:
                    return Instantiate(fireRateBooster, transform.position, transform.rotation);
                case 1:
                    return Instantiate(runBooster, transform.position, transform.rotation);
                case 2:
                    return Instantiate(jumpBooster, transform.position, transform.rotation);
            }

            return null;
        }

        void OnChangeHealth(int oldHealth, int newHealt)
        {
            healthDisplay.text = newHealt.ToString();
        }
    }
}
