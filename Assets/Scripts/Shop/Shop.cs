﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShopType
{
    weapon,
    mobility,
    augment,
    skill
}

public class Shop : MonoBehaviour
{
    [SerializeField] Transform[] A_spawnPoints;
    [SerializeField] ShopType st;
    List<IPurchasable> L_allItems = new List<IPurchasable>();
    IPurchasable[] ip_itemsOnDisplay;

    private void OnEnable()
    {
        foreach (IPurchasable ip in FindObjectsOfType<MonoBehaviour>().OfType<IPurchasable>())
        {
            if (ip.CheckShopType(st) && !ip.CheckPurchaseStatus())
                L_allItems.Add(ip);
        }
        ip_itemsOnDisplay = new IPurchasable[A_spawnPoints.Length];
        for (int i = 0; i < A_spawnPoints.Length; i++)
        {
            if (L_allItems.Count <= 0)
                return;
            int r = Random.Range(0, L_allItems.Count);
            ip_itemsOnDisplay[i] = L_allItems[r];
            L_allItems.RemoveAt(r);
            ip_itemsOnDisplay[i].GetGameObject().transform.position = A_spawnPoints[i].position;
            ip_itemsOnDisplay[i].GetGameObject().transform.rotation = A_spawnPoints[i].rotation;
            ip_itemsOnDisplay[i].GetGameObject().GetComponent<ToolBase>().SetShopRef(this);
        }
    }

    public void RemoveFromDisplay(IPurchasable _i_itemRemoved)
    {
        for(int i = 0; i < A_spawnPoints.Length; i++)
        {
            if(_i_itemRemoved == ip_itemsOnDisplay[i])
            {
                if(L_allItems.Count > 0)
                    ip_itemsOnDisplay[i] = L_allItems[Random.Range(0, L_allItems.Count)];
            }
        }
    }

}
