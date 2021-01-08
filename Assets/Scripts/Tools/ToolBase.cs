using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolBase : MonoBehaviourPun, IPurchasable
{
    #region Private Vars

    private bool b_audioable = true;

    #endregion

    #region Protected Vars
    protected int i_toolId;
    protected int i_rackId;
    protected bool b_usable = true;
    protected bool b_active = false;
    protected Transform t_cam;
    #endregion

    #region Serialized Vars
    [SerializeField] protected int i_cost;
    [SerializeField] protected bool b_purchased;
    [SerializeField] protected EnergyGauge eg_gauge;
    [SerializeField] protected float f_timeBetweenUsage;
    [Header("Audio")]
    [SerializeField] protected AudioClip ac_activationSound;
    [SerializeField] protected AudioClip ac_hitSound;
    [SerializeField] protected AudioClip ac_diegeticAudio;
    [SerializeField] private bool b_oneTimeAudio;
    [SerializeField] Transform t_raycastPoint;
    [SerializeField] protected bool b_releaseActivated;
    [SerializeField] protected GameObject go_particles;

    #endregion

    #region get/set
    public bool Purchased { get { return b_purchased; } set { b_purchased = value; } }
    public bool ReleaseActivated { get { return b_releaseActivated; } }
    public int ToolID { get { return i_toolId; } set { i_toolId = value; } }
    public int RackID { get { return i_rackId; } set { i_rackId = value; } }
    public int Cost { get { return i_cost; } }

    #endregion

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
        // Get the tool handler and swap the tool
        ToolHandler th = _go_owner.GetComponent<ToolHandler>();
        if (th)
        {
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

    public void PlayAudio(AudioClip _ac_aud)
    {
        if (_ac_aud != null)
        {
            AudioSource ass = GetComponent<AudioSource>();
            //ass.clip = _ac_aud;
            ass.pitch = Random.Range(0.95f, 1.05f);
            ass.PlayOneShot(_ac_aud);
        }
    }

    public virtual void StopAudio()
    {
        if (b_oneTimeAudio) return;
        AudioSource ass = GetComponent<AudioSource>();
        ass.Stop();
    }

    public virtual void SetActive(bool val)
    {

    }

    public bool GetActive()
    {
        return b_active;
    }

    public virtual void SetInfo(object[] infos)
    {

    }

    public virtual void PlayParticles(bool val)
    {
        go_particles?.SetActive(val);
    }
}
