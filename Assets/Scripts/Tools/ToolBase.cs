using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolBase : MonoBehaviour, IPurchasable
{
    #region Private Vars

    #endregion

    #region Protected Vars
    protected AugmentType[] at_augments = new AugmentType[0];
    [SerializeField] protected bool b_purchased;
    protected bool b_usable = true;
    protected Shop s_shopRef;
    protected Transform t_cam;
    #endregion

    #region Serialized Vars
    [SerializeField] protected EnergyGauge eg_gauge;
    [SerializeField] protected float f_timeBetweenUsage;
    [Header("Audio")]
    [SerializeField] protected AudioClip ac_activationSound;
    [SerializeField] protected AudioClip ac_hitSound;
    [SerializeField] protected AudioClip ac_diegeticAudio;
    [SerializeField] protected ShopType st = ShopType.weapon;
    [SerializeField] Transform t_raycastPoint;
    [SerializeField] protected bool b_releaseActivated;
    #endregion

    #region get/set
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

    public void Purchase(GameObject _go_owner, Transform _t_camera, params int[] _i_purchaseParams)
    {
        // Get the tool handler and swap the tool
        ToolHandler th = _go_owner.GetComponent<ToolHandler>();
        if (th)
        {
            th.SwapTool((ToolSlot)_i_purchaseParams[1], this);
            s_shopRef.RemoveFromDisplay(this);
            b_purchased = true;
            t_cam = _t_camera;
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
    public void SetShopRef(Shop _s_shopRef)
    {
        s_shopRef = _s_shopRef;
    }

    public bool CheckPurchaseStatus()
    {
        return Purchased;
    }

    protected IEnumerator TimeBetweenUsage()
    {
        yield return new WaitForSeconds(f_timeBetweenUsage);
        b_usable = true;
    }
}
