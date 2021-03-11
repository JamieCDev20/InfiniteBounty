using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeNode : Node
{

    protected Node[] nA_children;

    public Node[] GetChildren()
    {
        return nA_children;
    }

}