using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolRack : Shop
{
    [SerializeField] private ToolLoader tl_weaponTools;
    [SerializeField] private ToolLoader tl_mobTools;
    [SerializeField] private List<EmptyToolSlot> L_weaponToolPos;
    [SerializeField] private List<EmptyToolSlot> L_mobToolPos;
    [SerializeField] private Material m_silhouette;
    [SerializeField] private TMP_Text txt_exampleText;
    [SerializeField] private Vector3 t_textOffset;
    [SerializeField] private float f_maxShake;
    [SerializeField] private float f_minShake;
    [SerializeField] private float f_shakeTime;
    private List<Material[]> L_weaponMaterial = new List<Material[]>();
    private List<Material[]> L_utilityMaterial = new List<Material[]>();
    private List<int> L_weaponRackIDs = new List<int>();
    private List<int> L_mobilityRackIDs = new List<int>();
    private bool b_currentlyShaking = false;

    private void OnEnable()
    {
        SetToolInRack(tl_weaponTools, L_weaponToolPos);
        SetToolInRack(tl_mobTools, L_mobToolPos);
    }

    /// <summary>
    /// Takes a list of tools and places them on the rack
    /// </summary>
    /// <param name="_tl_loader">Type of tools to load</param>
    /// <param name="_t_toolTransform">Transforms to load them at</param>
    private void SetToolInRack(ToolLoader _tl_loader, List<EmptyToolSlot> _t_toolTransform)
    {
        int tlLength = _tl_loader.ToolCount();
        // Weapon tools are iterated through differently
        int toolRackID = 0;
        bool isWeapon = true;
        try
        {
            WeaponTool weaponTesting = (WeaponTool)_tl_loader.GetPrefabTool(0);
        }
        catch (System.InvalidCastException) { isWeapon = false; }

        for (int i = 0; i < tlLength; i++)
        {
            // Mobility and weapon transforms are set to different transforms
            EmptyToolSlot parent = isWeapon ? _t_toolTransform[i * 2] : _t_toolTransform[i];
            ToolBase tb = _tl_loader.LoadTool(i, parent.transform.root);
            tb.transform.position = parent.transform.position;
            tb.transform.rotation = parent.transform.rotation;
            // Store a reference to the material
            if (isWeapon)
                L_weaponMaterial.Add(tb.GetComponentInChildren<MeshRenderer>().sharedMaterials);
            else
                L_utilityMaterial.Add(tb.GetComponentInChildren<MeshRenderer>().sharedMaterials);
            // Unpurchased weapons set their material to a silhouette
            ApplyMaterials(tb, i);
            if (!tb.Purchased)
            {
                TMP_Text moneyText = Instantiate<TMP_Text>(txt_exampleText);
                //moneyText.gameObject.AddComponent<Billboard>();
                moneyText.gameObject.SetActive(true);
                moneyText.gameObject.transform.parent = tb.transform;
                moneyText.gameObject.transform.position = new Vector3(tb.transform.position.x + t_textOffset.x, tb.transform.position.y + t_textOffset.y, tb.transform.position.z + t_textOffset.z);
                moneyText.text = "£" + tb.Cost.ToString();
                //Debug.Log(moneyText.transform.parent.name);
            }

            tb.RackID = toolRackID;
            tb.gameObject.SetActive(true);

            parent.ToolID = tb.ToolID;
            parent.RackID = toolRackID;
            //Debug.Log(string.Format("Tool Rack: {0} | Parent: {1}", toolRackID, parent.RackID));
            parent.gameObject.SetActive(false);
            if (isWeapon)
            {
                L_weaponRackIDs.Add(toolRackID);
                WeaponTool wt = (WeaponTool)tb;
                if (wt.RackUpgrade)
                {
                    ToolBase dupe = _tl_loader.LoadTool(i, _t_toolTransform[i * 2 + 1].transform.root);
                    dupe.transform.position = _t_toolTransform[i * 2 + 1].transform.position;
                    dupe.transform.rotation = _t_toolTransform[i * 2 + 1].transform.rotation;
                    L_weaponMaterial.Add(dupe.GetComponentInChildren<MeshRenderer>().sharedMaterials);
                    toolRackID++;
                    dupe.RackID = toolRackID;
                    dupe.Purchased = true;
                    ApplyMaterials(dupe, toolRackID);
                    dupe.gameObject.SetActive(true);
                    L_weaponToolPos[i * 2 + 1].RackID = dupe.RackID;
                    L_weaponToolPos[i * 2 + 1].ToolID = dupe.ToolID;
                    L_weaponToolPos[i * 2 + 1].gameObject.SetActive(false);
                    L_weaponRackIDs.Add(toolRackID);
                    dupe.enabled = false;
                }
            }
            else
            {
                L_mobilityRackIDs.Add(toolRackID);
            }
            tb.enabled = false;
            toolRackID++;
        }
    }

    public void SetRackID(ToolBase _tb_, bool _b_rackType)
    {
        if (_b_rackType)
            _tb_.RackID = tl_weaponTools.GetToolAt(_tb_.ToolID).RackID;
        else
            _tb_.RackID = tl_mobTools.GetToolAt(_tb_.ToolID).RackID;
    }

    public int GetRackID(int _i_ID, bool _b_rackType)
    {
        if (_b_rackType)
            return tl_weaponTools.GetToolAt(_i_ID).RackID;
        else
            return tl_mobTools.GetToolAt(_i_ID).RackID;
    }

    public void ReturnToRack(int _i_ID, bool _b_rackType)
    {
        if (_b_rackType)
        {
            tl_weaponTools.GetToolAt(_i_ID).gameObject.SetActive(true);
            ApplyMaterials(tl_weaponTools.GetToolAt(_i_ID), _i_ID);
        }
        else
        {
            tl_mobTools.GetToolAt(_i_ID).gameObject.SetActive(true);
            ApplyMaterials(tl_mobTools.GetToolAt(_i_ID), _i_ID);
        }

    }

    public int RemoveFromRack(int _i_ID, bool _b_rackType)
    {
        if (_b_rackType)
        {
            ToolBase tb = tl_weaponTools.GetToolAt(_i_ID);
            tb.gameObject.SetActive(false);
            foreach (EmptyToolSlot ts in L_weaponToolPos)
                if (ts.RackID == _i_ID && ts.ToolID == tb.ToolID)
                {
                    ts.gameObject.SetActive(true);
                    //Debug.Log(string.Format("ID: {0} | Object: {1}", _i_ID, ts.gameObject.name));
                }
            return tb.RackID;
        }
        else
        {
            ToolBase tb = tl_mobTools.GetToolAt(_i_ID);
            tb.gameObject.SetActive(false);
            L_mobToolPos[_i_ID].gameObject.SetActive(true);
            return tb.RackID;
        }
    }

    public void UnableToBuy(int _i_rackID, bool _b_rackType)
    {
        ToolBase toolRef = _b_rackType ? tl_weaponTools.GetToolAt(_i_rackID) : tl_mobTools.GetToolAt(_i_rackID);
        Transform toolOrigin = toolRef.transform;
        if (!b_currentlyShaking)
        {
            b_currentlyShaking = true;
            StartCoroutine(ShakeTool(toolRef, toolOrigin.position));
        }
    }

    private void ApplyMaterials(ToolBase _tb_toolToMat, int index)
    {
        if (!_tb_toolToMat.Purchased)
        {
            foreach (MeshRenderer mr in _tb_toolToMat.GetComponentsInChildren<MeshRenderer>())
                if (!mr.gameObject.GetComponent<TMP_Text>())
                    mr.sharedMaterial = m_silhouette;
        }
        else
        {
            switch (_tb_toolToMat)
            {
                case WeaponTool wt:
                    wt.GetComponentInChildren<MeshRenderer>().sharedMaterials = L_weaponMaterial[index];
                    break;
                case MobilityTool mt:
                    mt.GetComponentInChildren<MeshRenderer>().sharedMaterials = L_utilityMaterial[index];
                    break;
            }
        }
    }

    private IEnumerator ShakeTool(ToolBase _tb_toolToShake, Vector3 _v_origin)
    {
        float timr = 0;
        float str = Random.Range(f_minShake, f_maxShake);
        while (timr < f_shakeTime)
        {
            _tb_toolToShake.transform.position = new Vector3(_v_origin.x + Mathf.Sin(str * Time.deltaTime),
                _v_origin.y, _v_origin.z);
            timr += Time.deltaTime;
        }
        yield return new WaitForEndOfFrame();
        _tb_toolToShake.transform.position = _v_origin;
        b_currentlyShaking = false;
    }
}
