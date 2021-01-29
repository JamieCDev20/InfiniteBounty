using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{

    public SelectorNode(params Node[] _children)
    {
        nA_children = _children;
    }

    public override bool Activate()
    {
        for (int i = 0; i < nA_children.Length; i++)
        {
            if (nA_children[i].Activate())
                return true;
        }
        return false;
    }

}
