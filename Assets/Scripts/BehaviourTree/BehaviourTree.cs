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
        Debug.Log("THIS SHOULDN'T HAPPEN FOR NICK");
        n_startNode.Activate();
    }

}
