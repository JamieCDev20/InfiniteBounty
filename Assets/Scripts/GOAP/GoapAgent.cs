using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class GoapAgent : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private int actionChainLengthLimit = 5;
    [SerializeField] private ActionIntDictionary costedActionSet;
    [SerializeField] private GoalIntDictionary prioritisedGoalSet;
    [SerializeField] private GameObject bulletPrefab;

    [Space]
    [Header("Test")]
    [SerializeField] private Action testAction;

    #endregion

    #region Private

    private Planner planner;
    private Validator validator;
    private AStar astar = new AStar();
    private Action currentAction;
    private GameObject target;

    private bool canShoot = true;

    private delegate void InstantActionDelegate();
    private delegate void LongActionDelegate(Vector3 targ);

    private InstantActionDelegate perform;
    private LongActionDelegate longPerform;

    private Action[] path;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        planner = new Planner();
        validator = new Validator(transform);

        SetupActions();

        //Debug.LogWarning(testAction.preconditions < testAction);

        astar.Init(costedActionSet.Keys.ToArray());

        Replan();

    }

    private void Update()
    {

        bool did = false;
        if (path == null)
            Replan();
        for (int i = 0; i < path.Length; i++)
        {
            if (validator.CheckActionValid(path[i]))
            {
                did = true;
                StartPerformingAction(path[i]);
                break;
            }

        }
        if (!did)
        {
            longPerform = null;
            perform = null;
            currentAction = null;
            Replan();
        }

        if (target != null)
            longPerform?.Invoke(target.transform.position);

    }

    #endregion

    #region Private Voids

    private void SetupActions()
    {
        foreach (Action a in costedActionSet.Keys)
        {
            a.SetUp();

        }
    }

    private void StartPerformingAction(Action action)
    {
        //Debug.Log("started performing: " + action.name);
        perform = null;
        longPerform = null;
        currentAction = action;

        string[] actionSignatures = action.signatures.ToArray();
        string[] currentSignature;
        for (int i = 0; i < actionSignatures.Length; i++)
        {
            currentSignature = actionSignatures[i].Split('-');

            switch (currentSignature[0])
            {
                case "0": // Has
                    break;
                case "3": // NextTo
                    if (currentSignature[1] == "1")
                    {
                        target = validator.FindClosest(TagManager.x.GetTagSet(currentSignature[2]), transform.position);
                        longPerform += Goto;
                    }
                    break;
                case "4": // Hit
                    if (currentSignature[1] == "1")
                    {
                        target = validator.FindClosest(TagManager.x.GetTagSet(currentSignature[2]), transform.position);
                        longPerform += Shoot;
                    }
                    break;
                default:
                    break;
            }

        }

        perform?.Invoke();

    }

    private void Shoot(Vector3 _target)
    {
        if (!canShoot)
            return;
        transform.LookAt(_target);

        GameObject b = PoolManager.x.SpawnNewObject(bulletPrefab, transform.position + transform.forward + Vector3.up, transform.rotation);
        b.GetComponent<Rigidbody>().AddForce(transform.forward * 15, ForceMode.Impulse);
        StartCoroutine(ShootCooldown());
        Replan();

    }

    private void Replan()
    {
        foreach (Goal g in prioritisedGoalSet.Keys)
        {
            float tim = Time.realtimeSinceStartup;
            path = astar.GeneratePath(g.goalConditions, ref validator, 5);
            //Debug.Log("time taken: " + (Time.realtimeSinceStartup - tim));
        }
    }

    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(1);
        canShoot = true;

    }

    private void Goto(Vector3 _target)
    {
        Debug.Log("GOTO");
        if(validator.path == null)
        {
            Debug.Log("going back to mesh");
            NavMeshHit hit;
            NavMesh.SamplePosition(transform.position, out hit, 5, -1);
            transform.rotation = Quaternion.LookRotation((hit.position  - transform.position), Vector3.up);
            transform.Translate((hit.position - transform.position).normalized * 15 * Time.deltaTime);
        }
        if (Vector3.Distance(transform.position, _target) > 0.25f)
        {
            transform.rotation = Quaternion.LookRotation((validator.path.corners[1] - transform.position), Vector3.up);
            transform.Translate((validator.path.corners[1] - transform.position).normalized * 15 * Time.deltaTime);
        }
    }

    #endregion

    #region Public Voids

    public void UpdateDomain(string key, bool val)
    {
        validator.UpdateDomain(key, val);
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns

    public NavMeshPath Path()
    {
        return validator.path;
    }

    #endregion

}

public struct Validator
{

    private RaycastHit hitInfo;
    public NavMeshPath path;
    private Transform t;
    public Dictionary<string, bool> domain;


    public Validator(Transform _t)
    {
        t = _t;
        hitInfo = new RaycastHit();
        path = new NavMeshPath();
        domain = new Dictionary<string, bool>();
    }

    public bool CheckPathTo(Vector3 _start, Vector3 _destination)
    {

        path = new NavMeshPath();

        NavMesh.CalculatePath(_start, _destination, -1, path);

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            path = null;
            return false;
        }

        return true;

    }

    public bool CheckPathTo(Vector3 _start, HashSet<GameObject> _destinations)
    {
        if (_destinations == null)
            return false;
        GameObject closest = FindClosest(_destinations, _start);
        if (closest != null)
            return CheckPathTo(_start, closest.transform.position);
        return false;
    }

    public bool CheckCanSee(Vector3 _start, GameObject _target)
    {
        //Debug.DrawRay(_start, (_target.transform.position - _start) * 10, Color.red);
        if (!Physics.Raycast(_start, (_target.transform.position - _start), out hitInfo, 100))
            return false;

        return hitInfo.collider.gameObject == _target;
    }

    public bool CheckCanSee(Vector3 _start, HashSet<GameObject> set)
    {
        if (set == null)
            return false;
        foreach (GameObject i in set)
        {
            if (CheckCanSee(_start, i))
                return true;
        }
        return false;
    }

    public bool CheckNextTo(Vector3 _start, GameObject _target)
    {
        return (_target.transform.position - _start).sqrMagnitude <= 0.25f;
    }

    public bool CheckNextTo(Vector3 _start, HashSet<GameObject> set)
    {
        if (set == null)
            return false;
        foreach (GameObject i in set)
        {
            if (CheckNextTo(_start, i))
                return true;
        }
        return false;
    }

    public bool CheckDomainCondition(string _key)
    {

        if (domain.ContainsKey(_key))
            return domain[_key];
        else
            UpdateDomain(_key, false);

        return false;
    }

    public void UpdateDomain(string _key, bool _val)
    {
        if (domain.ContainsKey(_key))
        {
            domain[_key] = _val;
        }
        else
        {
            domain.Add(_key, _val);
        }
    }

    public GameObject FindClosest(HashSet<GameObject> set, Vector3 origin)
    {

        GameObject closest = null;
        float dist = 10000;
        Vector3 v;
        foreach (GameObject i in set)
        {
            v = i.transform.position - origin;
            if (v.sqrMagnitude < dist * dist)
            {
                dist = v.magnitude;
                closest = i;
            }
        }
        return closest;

    }

    public bool CheckActionValid(Action action)
    {

        for (int i = 0; i < action.preconditions.Length; i++)
        {
            if (!CheckConditionMet(action.preconditions[i]))
                return false;
        }

        return true;
    }

    public bool CheckConditionMet(Condition con)
    {
        bool met;

        switch (con.prefix)
        {
            case Prefix.has:
                met = CheckDomainCondition(con.fill) == con.value;
                break;
            case Prefix.pathTo:
                met = CheckPathTo(t.position, TagManager.x.GetTagSet(con.fill)) == con.value;
                break;
            case Prefix.canSee:
                met = CheckCanSee(t.position, TagManager.x.GetTagSet(con.fill)) == con.value;
                break;
            case Prefix.nextTo:
                met = CheckNextTo(t.position, TagManager.x.GetTagSet(con.fill)) == con.value;
                break;
            case Prefix.hit:
                met = true;
                break;
            default:
                Debug.LogErrorFormat("\"{0}\" prefix has not been implemented yet \n Please log a work order with your local AI programmer so that he can add it to his growing work load. \n Oh my Gary that is a big work load... \n Maybe he can split it up with the other programmer? \n Wait he has a huge workload too? \n Shit... \n Oh well, it's probably fine.", con.prefix);
                met = false;
                break;
        }

        return met;
    }

}

public struct Planner
{

    public List<Action> actions;

    public bool FindActionPath(Action[] availableActions, Goal goal, int limit)
    {
        Action[] startingActions = GetLinkedActions(goal.goalConditions, availableActions);
        if (startingActions.Length > 0)
            for (int i = 1; i < limit; i++)
            {

            }

        return false;
    }

    public Action[] GetLinkedActions(Action baseAction, Action[] availableActions)
    {
        return GetLinkedActions(baseAction.preconditions, availableActions);
    }

    public Action[] GetLinkedActions(Condition[] pres, Action[] availableActions)
    {

        List<Action> linked = new List<Action>();

        for (int i = 0; i < availableActions.Length; i++)
        {
            if (CheckForMatchingConditions(pres, availableActions[i].postconditions))
                linked.Add(availableActions[i]);
        }

        return linked.ToArray();
    }

    public bool CheckForMatchingConditions(Condition[] pres, Condition[] post)
    {

        return Array.TrueForAll(pres, v => post.Contains(v));

    }

}