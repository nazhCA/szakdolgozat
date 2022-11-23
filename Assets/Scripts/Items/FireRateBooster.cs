using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Player;
using UnityEngine;

namespace Items
{
    public class FireRateBooster : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("hit");
                other.gameObject.GetComponent<ThirdPersonController>().SetFireRateAndTriggerBoost(0.1f);
                NetworkServer.Destroy(gameObject);
            }
        }
    }

}
