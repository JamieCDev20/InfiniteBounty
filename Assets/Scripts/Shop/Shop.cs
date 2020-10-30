using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShopType
{
    weapon,
    augment,
    skill
}

public class Shop : MonoBehaviour
{
    [SerializeField] Transform[] A_spawnPoints;
    [SerializeField] ShopType st;
    List<IPurchasable> L_allItems = new List<IPurchasable>();
    IPurchasable[] ip_itemsOnDisplay = new IPurchasable[4];
    private void Start()
    {
        foreach(IPurchasable ip in FindObjectsOfType<MonoBehaviour>().OfType<IPurchasable>())
        {
            if(ip.CheckShopType(st))
                L_allItems.Add(ip);
        }
    }

    private void OnEnable()
    {
        for(int i = 0; i < A_spawnPoints.Length; i++)
        {
            ip_itemsOnDisplay[i] = L_allItems[Random.Range(0, L_allItems.Count)];
            ip_itemsOnDisplay[i].GetGameObject().transform.position = A_spawnPoints[i].position;
            ip_itemsOnDisplay[i].GetGameObject().transform.rotation = A_spawnPoints[i].rotation;
        }
    }

    public void RemoveFromDisplay(IPurchasable _i_itemRemoved)
    {
        for(int i = 0; i < A_spawnPoints.Length; i++)
        {
            if(_i_itemRemoved == ip_itemsOnDisplay[i])
            {
                ip_itemsOnDisplay[i] = L_allItems[Random.Range(0, L_allItems.Count)];
            }
        }
    }

}
