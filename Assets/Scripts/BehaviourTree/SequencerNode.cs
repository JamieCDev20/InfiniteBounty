using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{

    public SequencerNode(params Node[] _children)
    {
        nA_children = _children;
    }

    public override bool Activate()
    {
        if (nA_children.Length < 1)
            return false;

        for (int i = 0; i < nA_children.Length; i++)
        {
            if (!nA_children[i].Activate())
                return false;
        }
        return true;
    }

}
