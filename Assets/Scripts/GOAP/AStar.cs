using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{

    private int maxChainLength = 5;
    private Node[] nodes;
    private List<Node> open = new List<Node>();
    private List<Node> tempOpen = new List<Node>();

    public void Init(Action[] actions)
    {
        nodes = ConvertToNode(actions).ToArray();
        for (int i = nodes.Length - 1; i > 0; i--)
        {
            nodes[i] = nodes[i].GenerateSuccessors(nodes, i);
        }
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = nodes[i].GenerateSuccessors(nodes, i);
        }
    }

    public Action[] GeneratePath(Condition[] _end, ref Validator valid, int _maxChainLength)
    {
        maxChainLength = _maxChainLength;
        List<Action> path = new List<Action>();

        List<Node> closed = new List<Node>();

        SetClosedList(out closed, ref valid);
        if (closed.Count < 1)
            return new Action[0];

        int shortestPath = maxChainLength + 1;

        for (int i = 0; i < closed.Count; i++)
        {
            tempOpen = new List<Node>();

            if (CheckSuccessors(closed[i], _end, 0))
            {
                if(!tempOpen.Contains(closed[i]))
                    tempOpen.Add(closed[i]);
                if (tempOpen.Count < shortestPath)
                {
                    open = tempOpen;
                    shortestPath = tempOpen.Count;
                }
            }
        }

        open.Reverse();

        for (int i = 0; i < open.Count; i++)
        {
            path.Add(open[i].reference);
        }

        return path.ToArray();

    }

    private bool CheckSuccessors(Node parent, Condition[] goal, int chainLength)
    {
        if (chainLength >= maxChainLength)
            return false;

        int index = 0;
        int curCost = 100;

        if (parent.successors.Length == 0)
        {
            if (!(parent.reference > goal))
                return false;
            else
            {
                tempOpen.Add(parent);
                return true;
            }

        }
        for (int i = 0; i < parent.successors.Length; i++)
        {
            if (nodes[parent.successors[i]].reference > goal)
            {
                tempOpen.Add(nodes[parent.successors[i]]);
                return true;
            }
            else if (tempOpen.Contains(nodes[parent.successors[i]]))
                continue;
            int c = Condition.Compare(goal, nodes[parent.successors[i]].reference.postconditions);
            if (c < curCost)
            {
                curCost = c;
                index = i;
            }
        }
        if (CheckSuccessors(nodes[parent.successors[index]], goal, chainLength + 1))
        {
            tempOpen.Add(parent);
            return true;
        }
        return false;
    }

    private void SetClosedList(out List<Node> _closedList, ref Validator valid)
    {
        _closedList = new List<Node>();

        for (int i = 0; i < nodes.Length; i++)
        {
            if (valid.CheckActionValid(nodes[i].reference))
                _closedList.Add(nodes[i]);
        }
    }

    private List<Node> ConvertToNode(Action[] _points)
    {
        List<Node> nodes = new List<Node>();
        for (int i = 0; i < _points.Length; i++)
        {
            nodes.Add(ConvertToNode(_points[i]));
        }
        return nodes;
    }

    private Node ConvertToNode(Action point)
    {
        return new Node(0, 0, point);
    }

}

public struct Node
{
    public float goal;
    public float heuristic;
    public float fitness;
    public Action reference;
    public int parentID;
    public int myID;
    public int[] successors;

    public Node(float _goal, float _heuristic, Action _reference)
    {
        goal = _goal;
        heuristic = _heuristic;
        fitness = goal + heuristic;
        reference = _reference;
        parentID = -1;
        myID = -1;
        successors = new int[0];
    }

    public void SetF()
    {
        fitness = goal + heuristic;
    }

    public void SetG(Condition[] cond, int plus)
    {

    }

    public Node GenerateSuccessors(Node[] nodes, int myId)
    {
        myID = myId;
        List<int> t = new List<int>();
        for (int i = 0; i < nodes.Length; i++)
        {
            //if (i == myId)
            //    continue;
            if (reference > nodes[i].reference)
            {
                t.Add(nodes[i].myID);
                nodes[i].parentID = myId;
            }

        }
        successors = t.ToArray();

        return this;
    }

}