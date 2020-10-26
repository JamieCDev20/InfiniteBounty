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
        for(int i = 0; i < 3; i++)
        {
            ip_itemsOnDisplay[i] = L_allItems[Random.Range(0, L_allItems.Count)];
        }
    }
}
