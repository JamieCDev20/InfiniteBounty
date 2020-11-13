using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AugmentShop : MonoBehaviour
{
    [SerializeField] Transform[] A_spawnPoints;
    [SerializeField] List<GameObject> L_allItems;
    GameObject[] A_itemsOnDisplay;
    IPurchasable[] ip_purchasables;

    private void OnEnable()
    {
        // Items can only be on display if there are enough spawn points
        A_itemsOnDisplay = new GameObject[A_spawnPoints.Length];
        // Objects in scene 
        List<GameObject> L_objectsInScene = new List<GameObject>();
        ip_purchasables = new IPurchasable[A_spawnPoints.Length];
        foreach(GameObject go in L_allItems)
        {
            L_objectsInScene.Add(Instantiate(go));
            go.transform.position = transform.position;
            go.SetActive(false);
        }
        for (int i = 0; i < A_spawnPoints.Length; i++)
        {
            if (L_allItems.Count <= 0)
                return;
            int r = Random.Range(0, L_allItems.Count);
            A_itemsOnDisplay[i] = L_objectsInScene[r];
            A_itemsOnDisplay[i].SetActive(true);
            A_itemsOnDisplay[i].transform.parent = transform;
            ip_purchasables[i] = A_itemsOnDisplay[i].GetComponent<IPurchasable>();
            L_allItems.RemoveAt(r);
            L_objectsInScene.RemoveAt(r);
            A_itemsOnDisplay[i].transform.position = A_spawnPoints[i].position;
            A_itemsOnDisplay[i].transform.rotation = A_spawnPoints[i].rotation;
        }
    }

    public void RemoveFromDisplay(IPurchasable _i_itemRemoved)
    {
        for(int i = 0; i < A_spawnPoints.Length; i++)
        {
            if(_i_itemRemoved == ip_purchasables[i])
            {
                if(L_allItems.Count > 0)
                {
                    int r = Random.Range(0, L_allItems.Count);
                    A_itemsOnDisplay[i] = L_allItems[r];
                    ip_purchasables[i] = L_allItems[r].GetComponent<IPurchasable>();
                }
            }
        }
    }

}
