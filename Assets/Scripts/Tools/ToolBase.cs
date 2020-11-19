using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolBase : MonoBehaviourPun, IPurchasable
{
    #region Private Vars

    #endregion

    #region Protected Vars
    protected int i_toolId;
    protected int i_rackId;
    protected bool b_usable = true;
    protected Transform t_cam;
    protected AugmentType[] at_augments = new AugmentType[0];
    [SerializeField] protected bool b_purchased;
    #endregion

    #region Serialized Vars
    [SerializeField] protected EnergyGauge eg_gauge;
    [SerializeField] protected float f_timeBetweenUsage;
    [Header("Audio")]
    [SerializeField] protected AudioClip ac_activationSound;
    [SerializeField] protected AudioClip ac_hitSound;
    [SerializeField] protected AudioClip ac_diegeticAudio;
    [SerializeField] Transform t_raycastPoint;
    [SerializeField] protected bool b_releaseActivated;
    #endregion

    #region get/set
    public bool Purchased { get { return b_purchased; } }
    public bool ReleaseActivated { get { return b_releaseActivated; } }
    public int ToolID { get { return i_toolId; } set { i_toolId = value; } }
    public int RackID { get { return i_rackId; } set { i_rackId = value; } }

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

    public virtual void NetUse(Vector3 _v_forwards)
    {

    }

    public virtual void Use(Vector3 _v_forwards)
    {

    }

    public void Purchase(GameObject _go_owner, Transform _t_camera, Shop _sh_shopRef, params int[] _i_purchaseParams)
    {
        GetComponent<Collider>().isTrigger = true;
        // Get the tool handler and swap the tool
        ToolHandler th = _go_owner.GetComponent<ToolHandler>();
        if (th)
        {
            //th.CallSwapTool((ToolSlot)_i_purchaseParams[1], i_toolId);

            switch (_sh_shopRef)
            {
                case ToolRack tr:
                    switch (this)
                    {
                        case WeaponTool wt:
                            //i_rackId = tr.RemoveFromRack(RackID, true);
                            break;
                        case MobilityTool mt:
                            //i_rackId = tr.RemoveFromRack(RackID, false);
                            break;
                    }
                    break;
            }

            //_sh_shopRef.RemoveFromDisplay(this);
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

    public bool CheckPurchaseStatus()
    {
        return Purchased;
    }

    public void SetCamera(Transform _t_cam)
    {
        t_cam = _t_cam;
    }

    protected IEnumerator TimeBetweenUsage()
    {
        yield return new WaitForSeconds(f_timeBetweenUsage);
        b_usable = true;
    }
}
