using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolBase : MonoBehaviour, IPurchasable
{
    #region Private Vars
    const ShopType st = ShopType.weapon;

    #endregion

    #region Protected Vars

    #endregion

    #region Serialized Vars
    [SerializeField] protected EnergyGauge eg_gauge;
    [Header("Audio")]
    [SerializeField] protected AudioClip ac_activationSound;
    [SerializeField] protected AudioClip ac_hitSound;
    [SerializeField] protected AudioClip ac_diegeticAudio;


    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Use()
    {

    }

    public virtual bool CheckShopType(ShopType _st_itemType)
    {
        if (st == _st_itemType)
            return true;
        return false;
    }
    /// <summary>
    /// Purchase Weapons
    /// </summary>
    /// <param name="_i_cost">Cost of weapon</param>
    /// <param name="_i_purchaseParams">player id, tool slot id</param>
    public virtual void Purchase(int _i_cost, params int[] _i_purchaseParams)
    {
        throw new System.NotImplementedException();
    }
}
