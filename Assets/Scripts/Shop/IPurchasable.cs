using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchasable
{
    /// <summary>
    /// Obtain the item that is purchaseable
    /// </summary>
    /// <param name="_i_cost">the const of the item</param>
    /// <param name="_i_purchaseParams">All interger parameters, always startintg with player ID and any more info needed</param>
    void Purchase(int _i_cost, params int[] _i_purchaseParams);
    bool CheckShopType(ShopType _st_itemType);
}
