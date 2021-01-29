using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    protected Node n_parentNode;
    public delegate bool QueryNodeDelegate();
    public delegate void ActionNodeDelegate();

    public virtual bool Activate()
    {
        return false;
    }

}
