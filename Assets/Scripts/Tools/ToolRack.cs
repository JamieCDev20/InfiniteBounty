using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolRack : Shop
{
    [SerializeField] private ToolLoader tl_weaponTools;
    [SerializeField] private ToolLoader tl_mobTools;
    [SerializeField] private List<Transform> L_weaponToolPos;
    [SerializeField] private List<Transform> L_mobToolPos;
    [SerializeField] private Material m_silhouette;
    private List<int> L_weaponRackIDs = new List<int>();
    private List<int> L_mobilityRackIDs = new List<int>();

    private void OnEnable()
    {
        ToolHandler playerRef = null;
        if (FindObjectOfType<NetworkedPlayer>())
        {
            playerRef = FindObjectOfType<NetworkedPlayer>().GetComponent<ToolHandler>();
        }

        SetToolInRack(tl_weaponTools, L_weaponToolPos);
        SetToolInRack(tl_mobTools, L_mobToolPos);
    }

    private void SetToolInRack(ToolLoader _tl_loader, List<Transform> _t_toolTransform)
    {
        int tlLength = _tl_loader.ToolCount();
        int toolRackID = 0;
        bool isWeapon = true;
        try
        {
            WeaponTool weaponTesting = (WeaponTool)_tl_loader.GetPrefabTool(0);
        }
        catch (System.InvalidCastException) { isWeapon = false; }
        for (int i = 0; i < tlLength; i++)
        {
            // Mobility and weapon transforms are different
            Transform parent = isWeapon ? _t_toolTransform[i * 2] : _t_toolTransform[i];
            ToolBase tb = _tl_loader.LoadTool(i, parent);
            if (!tb.Purchased)
                tb.GetComponent<MeshRenderer>().sharedMaterial = m_silhouette;
            tb.RackID = toolRackID;
            Debug.Log(tb.name + " " + tb.RackID);
            tb.gameObject.SetActive(true);
            if (isWeapon)
            {
                L_weaponRackIDs.Add(toolRackID);
                WeaponTool wt = (WeaponTool)tb;
                if (wt.RackUpgrade)
                {
                    ToolBase dupe = _tl_loader.LoadTool(i, _t_toolTransform[i * 2 + 1]);
                    toolRackID++;
                    dupe.RackID = toolRackID;
                    dupe.gameObject.SetActive(true);
                    L_weaponRackIDs.Add(toolRackID);
                }
            }
            else
            {
                L_mobilityRackIDs.Add(toolRackID);
            }
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
        Debug.Log(_b_rackType == true ? "Weapon: " : "Mobility: " + _i_ID);
        if (_b_rackType)
            tl_weaponTools.GetToolAt(_i_ID).gameObject.SetActive(true);
        else
            tl_mobTools.GetToolAt(_i_ID).gameObject.SetActive(true);

    }

    public int RemoveFromRack(int _i_ID, bool _b_rackType)
    {
        if (_b_rackType)
        {
            ToolBase tb = tl_weaponTools.GetToolAt(_i_ID);
            tb.gameObject.SetActive(false);
            return tb.RackID;
        }
        else
        {
            ToolBase tb = tl_mobTools.GetToolAt(_i_ID);
            tb.gameObject.SetActive(false);
            return tb.RackID;
        }
    }
}
