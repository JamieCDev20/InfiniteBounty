﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolLoader : MonoBehaviour
{
    [SerializeField] private ToolBase[] tb_tools;
    [SerializeField] private List<ToolBase> tb_loadedTools;
    [SerializeField] private ToolSlot ts_slot;
    public ToolSlot Slot { get { return ts_slot; } }

    public void LoadTools(Transform _t_parent)
    {

        for (int i = 0; i < tb_tools.Length; i++)
        {
            LoadTool(i, _t_parent);
        }
    }
    public ToolBase LoadTool(int _i_index, Transform _t_parent)
    {
        ToolBase go_tool = Instantiate(tb_tools[_i_index]);
        go_tool.transform.position = _t_parent.position;
        go_tool.transform.rotation = _t_parent.rotation;
        go_tool.transform.parent = _t_parent;
        go_tool.transform.localScale = Vector3.one;
        go_tool.ToolID = _i_index;
        go_tool.gameObject.SetActive(false);
        tb_loadedTools.Add(go_tool);
        return go_tool;
    }
    public int ToolCount()
    {
        return tb_tools.Length;
    }
    public ToolBase GetToolAt(int _i_index)
    {
        return tb_loadedTools[_i_index];
    }

    public ToolBase GetPrefabTool(int _i_index)
    {
        return tb_tools[_i_index];
    }

    public ToolBase GetPrefabTool(ToolBase _go_toolRef)
    {
        foreach (ToolBase go_tool in tb_tools)
            if (_go_toolRef == go_tool)
                return go_tool;
        return null;
    }

}
