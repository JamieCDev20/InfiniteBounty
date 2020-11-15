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
            tb.gameObject.SetActive(true);
            if (isWeapon)
            {
                WeaponTool wt = (WeaponTool)tb;
                if (wt.RackUpgrade)
                {
                    ToolBase dupe = _tl_loader.LoadTool(i, _t_toolTransform[i * 2 + 1]);
                    dupe.gameObject.SetActive(true);
                }
            }
        }
    }
}
