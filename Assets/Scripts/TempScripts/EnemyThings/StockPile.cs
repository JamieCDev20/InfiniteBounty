using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockPile : MonoBehaviour
{

    [SerializeField] private int i_hitsLeft;
    [SerializeField] private GameObject go_enemy;
    [SerializeField] private float f_spawnDistance;
    private GameObject go_looker;

    internal void Begin()
    {
        go_looker = new GameObject("StockPileLooker");
        InvokeRepeating("CreateWave", 0, 10);
    }

    private void CreateWave()
    {
        go_looker.transform.position = transform.position;
        for (int i = 0; i < 20 - i_hitsLeft; i++)
        {
            go_looker.transform.Rotate(0, Random.value * 360, 0);
            Instantiate(go_enemy, (transform.position + go_looker.transform.forward * f_spawnDistance) + new Vector3(0, -0.5f, 0), Quaternion.identity);
        }
    }

    internal void TakeDamage()
    {
        i_hitsLeft--;
        if (i_hitsLeft < 0) Death();
    }

    private void Death()
    {
        gameObject.SetActive(false);
    }

}
