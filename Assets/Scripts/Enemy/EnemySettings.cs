using System;
using InputSystem;
using Mirror;
using UnityEngine;

namespace Enemy
{
    public class EnemySettings : NetworkBehaviour
    {
        public float enemyDamage = 10f;
        private GameObject _child;

        void Update()
        {
            _child.transform.rotation = Quaternion.Euler (0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
        }

        private void Start()
        {
            _child = transform.GetChild(0).gameObject;
        }
    }
}
