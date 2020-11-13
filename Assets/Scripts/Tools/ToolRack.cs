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
        int id = FindObjectOfType<NetworkedPlayer>().PlayerID;
        int index = 0;
        foreach(WeaponTool tool in L_weaponTools)
        {
            tool.transform.position = L_weaponToolPos[index].transform.position;
            if(!tool.Purchased)
                tool.gameObject.GetComponent<MeshRenderer>().sharedMaterial = m_silhouette;
            if (tool.RackUpgrade)
                Instantiate(tool, L_weaponToolPos[index + 1]);

            index += 2;
        }
        index = 0;
        foreach(MobilityTool mtool in L_mobTools)
        {
            mtool.transform.position = L_mobToolPos[index].position;
            if (!mtool.Purchased)
                mtool.gameObject.GetComponent<MeshRenderer>().sharedMaterial = m_silhouette;
            index++;
        }
    }

}
