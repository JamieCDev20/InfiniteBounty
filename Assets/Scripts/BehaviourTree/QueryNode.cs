using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueryNode : LeafNode
{

    private QueryNodeDelegate nodeAction;

    public QueryNode(QueryNodeDelegate _query)
    {
        nodeAction = _query;
    }

    public override bool Activate()
    {
        return nodeAction();
    }

}
