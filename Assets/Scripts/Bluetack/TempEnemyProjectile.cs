using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyProjectile : MonoBehaviour
{

    [SerializeField] private int i_damage = 15;
    [SerializeField] private GameObject go_deathParticle;

    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            IHitable ih = collision.gameObject.GetComponent<IHitable>();
            if (ih != null)
                ih.TakeDamage(Mathf.RoundToInt(i_damage * DifficultyManager.x.ReturnCurrentDifficulty().f_damageMult), false);
        }


        go_deathParticle.SetActive(false);
        go_deathParticle.transform.parent = null;
        go_deathParticle.transform.position = transform.position;
        go_deathParticle.transform.forward = collision.GetContact(0).normal;
        go_deathParticle.SetActive(true);

        PoolManager.x?.ReturnObjectToPool(gameObject);
    }
}