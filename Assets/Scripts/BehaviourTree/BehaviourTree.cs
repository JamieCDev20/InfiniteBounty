using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree
{

    private Node n_startNode;

    public BehaviourTree(Node _StartingNode)
    {
        n_startNode = _StartingNode;
    }

    public void DoTreeIteration()
    {
        n_startNode.Activate();
    }

}
