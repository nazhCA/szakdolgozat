using System;
using Mirror;
using UnityEngine;

namespace Enemy
{
    public class EnemySettings : NetworkBehaviour
    {
        public float enemyDamage = 10f;

        void Update()
        {
            if (GetComponent<Renderer>().isVisible)
            {
            }
        }
    }
}
