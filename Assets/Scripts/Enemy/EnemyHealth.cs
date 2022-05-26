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

        public void TakeDamage(int amount)
        {
            if (!isServer)
            {
                return;
            }

            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Destroy(gameObject);
            }

        }

        void OnChangeHealth(int oldHealth, int newHealt)
        {
            healthDisplay.text = newHealt.ToString();
        }
    }
}
