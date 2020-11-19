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

    private void OnEnable()
    {
        ToolHandler playerRef = null;
        ToolLoader playerTools = null;
        if (FindObjectOfType<NetworkedPlayer>())
        {
            playerRef = FindObjectOfType<NetworkedPlayer>().GetComponent<ToolHandler>();
            playerTools = FindObjectOfType<NetworkedPlayer>().GetComponent<ToolLoader>();
        }
        SetToolInRack(tl_weaponTools, L_weaponToolPos, playerTools);
        SetToolInRack(tl_mobTools, L_mobToolPos, playerTools);
    }

    private void SetToolInRack(ToolLoader _tl_loader, List<Transform> _t_toolTransform, ToolLoader _tl_playerTools)
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
            Transform parent = isWeapon ? _t_toolTransform[i * 2] : _t_toolTransform[i];
            ToolBase tb = _tl_loader.LoadTool(i, parent);
            if (!tb.Purchased)
                tb.GetComponent<MeshRenderer>().sharedMaterial = m_silhouette;
            tb.RackID.Add(toolRackID);
            tb.gameObject.SetActive(true);
            if (isWeapon)
            {
                WeaponTool wt = (WeaponTool)tb;
                if (wt.RackUpgrade)
                {
                    ToolBase dupe = _tl_loader.LoadTool(i, _t_toolTransform[i * 2 + 1]);
                    toolRackID++;
                    dupe.RackID.Add(toolRackID);
                    dupe.gameObject.SetActive(true);
                }
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

    public List<int> GetRackID(int _i_ID, bool _b_rackType)
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

        }
        else
        {
            tl_mobTools.GetToolAt(_i_ID).gameObject.SetActive(true);

        }
    }

    public void RemoveFromRack(List<int> _i_ID, bool _b_rackType)
    {
        if (_b_rackType)
            tl_weaponTools.GetToolAt(_i_ID[0]).gameObject.SetActive(false);
        else
            tl_mobTools.GetToolAt(_i_ID[0]).gameObject.SetActive(false);
    }
}
