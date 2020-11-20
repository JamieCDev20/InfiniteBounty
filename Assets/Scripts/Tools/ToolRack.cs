using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolRack : Shop
{
    [SerializeField] private ToolLoader tl_weaponTools;
    [SerializeField] private ToolLoader tl_mobTools;
    [SerializeField] private List<EmptyToolSlot> L_weaponToolPos;
    [SerializeField] private List<EmptyToolSlot> L_mobToolPos;
    [SerializeField] private Material m_silhouette;
    private List<int> L_weaponRackIDs = new List<int>();
    private List<int> L_mobilityRackIDs = new List<int>();

    private void OnEnable()
    {
        SetToolInRack(tl_weaponTools, L_weaponToolPos);
        SetToolInRack(tl_mobTools, L_mobToolPos);
    }

    private void SetToolInRack(ToolLoader _tl_loader, List<EmptyToolSlot> _t_toolTransform)
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
            // Mobility and weapon transforms are set to different transforms
            EmptyToolSlot parent = isWeapon ? _t_toolTransform[i * 2] : _t_toolTransform[i];
            ToolBase tb = _tl_loader.LoadTool(i, parent.transform.root);
            tb.transform.position = parent.transform.position;
            // Unpurchased weapons set their material to a silhouette
            if (!tb.Purchased)
                tb.GetComponent<MeshRenderer>().sharedMaterial = m_silhouette;
            tb.RackID = toolRackID;
            tb.gameObject.SetActive(true);

            parent.ToolID = tb.ToolID;
            parent.RackID = tb.RackID;
            parent.gameObject.SetActive(false);
            if (isWeapon)
            {
                L_weaponRackIDs.Add(toolRackID);
                WeaponTool wt = (WeaponTool)tb;
                if (wt.RackUpgrade)
                {
                    ToolBase dupe = _tl_loader.LoadTool(i, _t_toolTransform[i * 2 + 1].transform.root);
                    dupe.transform.position = _t_toolTransform[i * 2 + 1].transform.position;
                    toolRackID++;
                    dupe.RackID = toolRackID;
                    dupe.gameObject.SetActive(true);
                    L_weaponToolPos[i * 2 + 1].gameObject.SetActive(false);
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
            L_weaponToolPos[_i_ID].gameObject.SetActive(true);
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
}
