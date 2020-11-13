using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolRack : MonoBehaviour
{
    [SerializeField] private List<WeaponTool> L_weaponTools;
    [SerializeField] private List<MobilityTool> L_mobTools;
    [SerializeField] private List<Transform> L_weaponToolPos;
    [SerializeField] private List<Transform> L_mobToolPos;
    [SerializeField] private Material m_silhouette;

    private void OnEnable()
    {
        for(int i = 0; i < L_weaponTools.Count; i++)
        {
            Debug.Log(i);
            WeaponTool wt = Instantiate(L_weaponTools[i]);
            wt.transform.position = L_weaponToolPos[i].position;
            wt.transform.rotation = L_weaponToolPos[i].rotation;
            wt.transform.parent = transform;
            wt.gameObject.SetActive(true);

            if (wt.RackUpgrade)
            {
                WeaponTool dupeTool = Instantiate(L_weaponTools[i]);
                dupeTool.transform.position = L_weaponToolPos[i + 1].position;
                dupeTool.transform.rotation = L_weaponToolPos[i + 1].rotation;
                dupeTool.transform.parent = transform;
            }
        }
    }

}
