using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatGuageUI : MonoBehaviour
{
    private ToolHandler tl_tools;

    [Header("Heat Guages")]
    [SerializeField] private RectTransform rt_leftHeatGuage;
    [SerializeField] private RectTransform rt_rightHeatGuage;
    private ToolBase tb_leftTool;
    private ToolBase tb_rightTool;

    private void Start()
    {
        tl_tools = GetComponent<ToolHandler>();
        tb_leftTool = tl_tools.GetToolBase(0);
        tb_rightTool = tl_tools.GetToolBase(1);
    }

    public void SetNewTools(ToolBase _tb_newLeft, ToolBase _tb_newRight)
    {
        tb_leftTool = _tb_newLeft;
        tb_rightTool = _tb_newRight;
    }

    private void Update()
    {
        /*
        if (tb_leftTool)
            if (tb_leftTool.CURRENTHEAT < tb_leftTool.MAXHEAT)
            {
                SetLeftHeatGuage(tb_leftTool.CURRENTHEAT, tb_leftTool.MAXHEAT);
                rt_rightHeatGuage.gameObject.SetActive(true);
            }
        rt_rightHeatGuage.gameObject.SetActive(false);
        if (tb_rightTool)
            if (tb_rightTool.CURRENTHEAT < tb_rightTool.MAXHEAT)
            {
                SetLeftHeatGuage(tb_rightTool.CURRENTHEAT, tb_rightTool.MAXHEAT);
                rt_leftHeatGuage.gameObject.SetActive(true);
            }
            else
                rt_leftHeatGuage.gameObject.SetActive(false);
        */
    }

    public void SetLeftHeatGuage(int _i_currentHeat, int _i_maxHeat)
    {
        rt_leftHeatGuage.localScale = new Vector3(1 - (_i_currentHeat / _i_maxHeat), 1, 1);
    }

    public void SetRightHeatGuage(int _i_currentHeat, int _i_maxHeat)
    {
        rt_rightHeatGuage.localScale = new Vector3(1 - (_i_currentHeat / _i_maxHeat), 1, 1);
    }
}