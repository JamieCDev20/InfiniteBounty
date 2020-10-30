using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolBase : MonoBehaviour, IPurchasable
{
    #region Private Vars
    const ShopType st = ShopType.weapon;

    #endregion

    #region Protected Vars
    protected AugmentType[] at_augments;
    #endregion

    #region Serialized Vars
    [SerializeField] protected EnergyGauge eg_gauge;
    [Header("Audio")]
    [SerializeField] protected AudioClip ac_activationSound;
    [SerializeField] protected AudioClip ac_hitSound;
    [SerializeField] protected AudioClip ac_diegeticAudio;
    [SerializeField] Transform t_raycastPoint;
    [SerializeField] protected bool b_releaseActivated;
    protected bool b_purchased = false;
    public bool Purchased { get { return b_purchased; } }
    public bool ReleaseActivated { get { return b_releaseActivated; } }
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

    public void Attach()
    {

    }

    public void Purchase(GameObject _go_owner, params int[] _i_purchaseParams)
    {
        if (_go_owner.GetComponent<PlayerController>())
        {
            _go_owner.GetComponent<ToolHandler>().SwapWeapon((ToolSlot)_i_purchaseParams[1], this);
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public Transform GetRaycastTransform()
    {
        return t_raycastPoint;
    }
}
