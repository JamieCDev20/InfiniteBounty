using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : LeafNode
{

    private ActionNodeDelegate nodeAction;

    public ActionNode(ActionNodeDelegate _action)
    {
        nodeAction = _action;
    }

    public override bool Activate()
    {
        nodeAction();
        return true;
    }
}
