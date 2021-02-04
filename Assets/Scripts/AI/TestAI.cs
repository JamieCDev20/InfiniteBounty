using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAI : MonoBehaviour
{

    BehaviourTree tree;

    private void Awake()
    {


        SequencerNode sequence = new SequencerNode(new QueryNode(testFalse), new QueryNode(testFalse));

        SelectorNode select = new SelectorNode(sequence, new ActionNode(testAction));


        tree = new BehaviourTree(select);

    }

    private void Update()
    {
        //tree.DoTreeIteration();
    }

    public bool testTrue()
    {
        Debug.Log("Tested True");
        return true;
    }

    public bool testFalse()
    {
        Debug.Log("Tested False");
        return false;
    }

    public void testAction()
    {
        Debug.Log("Testing Action");
    }

}
