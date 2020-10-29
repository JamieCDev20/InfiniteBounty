using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockPile : MonoBehaviour
{

    [SerializeField] private int i_hitsLeft;
    [SerializeField] private GameObject go_enemy;
    [SerializeField] private float f_spawnDistance;
    private GameObject go_looker;
    [SerializeField] private GameObject go_deathEffects;
    [SerializeField] private int i_secondsToSuccess;

    internal void Begin()
    {
        go_looker = new GameObject("StockPileLooker");
        InvokeRepeating("CreateWave", 0, 10);
        go_deathEffects.SetActive(false);
        Invoke("Success", i_secondsToSuccess);
    }

    private void CreateWave()
    {
        if (gameObject.activeSelf)
        {
            go_looker.transform.position = transform.position;
            for (int i = 0; i < 20 - i_hitsLeft; i++)
            {
                go_looker.transform.Rotate(0, Random.value * 360, 0);
                Instantiate(go_enemy, (transform.position + go_looker.transform.forward * f_spawnDistance) + new Vector3(0, -0.5f, 0), Quaternion.identity);
            }
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
        go_deathEffects.transform.parent = null;
        go_deathEffects.SetActive(true);
        CancelInvoke();
        Invoke("BeamBackToShip", 5);
        LocationController.x.CompletedStandoff(false);
    }

    private void Success()
    {
        LocationController.x.CompletedStandoff(true);
        BeamBackToShip();
        gameObject.SetActive(false);
        CancelInvoke();
    }

    private void BeamBackToShip()
    {
        for (int i = 0; i < LocationController.x.goA_playerObjects.Length; i++)
            LocationController.x.goA_playerObjects[i].transform.position = new Vector3(0, 0, i * 1.5f);
        LocationController.x.UnloadArea();
    }

}
