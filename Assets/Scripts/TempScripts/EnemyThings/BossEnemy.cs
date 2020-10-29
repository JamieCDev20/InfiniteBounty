using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{

    [Header("Boss Info")]
    [SerializeField] private List<GameObject> goL_playerObjects = new List<GameObject>();
    private int i_targetIndex;
    private GameObject go_looker;
    [SerializeField] private float f_walkSpeed;
    private Rigidbody rb_rigidbody;
    [SerializeField] private GameObject go_shipButton;

    [Header("Boss-Lode Stats")]
    [SerializeField] private GameObject go_nuggetPrefab;
    private List<GameObject> goL_nuggetPool = new List<GameObject>();
    [SerializeField] private int i_nuggetsToSpawn;
    [SerializeField] private int i_nuggetsPerBurst;
    [SerializeField] private float f_nuggetForce;
    [SerializeField] private int[] iA_healthIntervals = new int[0];
    [SerializeField] private GameObject[] goA_damageMeshes = new GameObject[3];

    [Header("Boss Attack Stats")]
    [SerializeField] private GameObject go_throwableRock;
    [SerializeField] private float f_throwPower;
    [Space, SerializeField] private GameObject go_smallLodePrefab;
    [SerializeField] private int i_rocksToSpawn;
    [SerializeField] private float f_spaceBetweenChunks;
    [SerializeField] private float f_heightPerTick;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < i_nuggetsToSpawn; i++)
        {
            goL_nuggetPool.Add(Instantiate(go_nuggetPrefab, transform));
            goL_nuggetPool[i].SetActive(false);
        }
        go_looker = new GameObject("BossLooker");
        rb_rigidbody = GetComponent<Rigidbody>();
    }

    internal override void Begin()
    {
        base.Start();
        b_isHunting = true;
        InvokeRepeating("FindNewTarget", 0, 10);
        Invoke("UseAttack", 4);
    }

    internal override void Death()
    {
        base.Death();
        go_shipButton.SetActive(true);
    }

    internal override void TakeDamage(int _i_damage)
    {
        i_currentHealth -= _i_damage;

        if (i_currentHealth <= 0) Death();

        for (int i = 0; i < iA_healthIntervals.Length; i++)
        {
            if (i_currentHealth <= iA_healthIntervals[i])
            {
                NuggetBurst();
                goA_damageMeshes[i].SetActive(false);
                iA_healthIntervals[i] = 0;
            }
        }

    }

    private void NuggetBurst()
    {
        for (int i = 0; i < i_nuggetsPerBurst; i++)
        {
            GameObject _go_nugget = goL_nuggetPool[0];
            goL_nuggetPool.RemoveAt(0);
            _go_nugget.SetActive(true);
            _go_nugget.transform.parent = null;
            _go_nugget.transform.position = transform.position + transform.localScale * (-1 + Random.value * 2) + (Vector3.up * 2);
            _go_nugget.transform.localScale = Vector3.one;
            Rigidbody _rb = _go_nugget.GetComponent<Rigidbody>();
            _rb.AddForce(new Vector3(-1 + Random.value * 2, Random.value * 2, -1 + Random.value * 2) * f_nuggetForce, ForceMode.Impulse);
            _go_nugget.transform.rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);
        }
    }

    private void Update()
    {
        if (b_isHunting)
        {
            go_looker.transform.LookAt(goL_playerObjects[i_targetIndex].transform.position);
            go_looker.transform.position = transform.position;

            transform.position += transform.forward * f_walkSpeed * Time.deltaTime;

            transform.forward = Vector3.Scale(Vector3.Lerp(transform.forward, go_looker.transform.forward, 0.3f), new Vector3(1, 0, 1));
        }
    }

    private void FindNewTarget()
    {
        i_targetIndex = Random.Range(0, goL_playerObjects.Count);
    }

    private void UseAttack()
    {
        if (Vector3.Distance(transform.position, goL_playerObjects[i_targetIndex].transform.position) > 15)
            RockThrow();
        else
            RockWall();
    }

    private void RockThrow()
    {
        if (gameObject.activeSelf)
        {
            GameObject _go_rock = Instantiate(go_throwableRock, transform.position + (Vector3.up * 10), Quaternion.identity, transform.parent);
            _go_rock.transform.LookAt(goL_playerObjects[i_targetIndex].transform);
            _go_rock.transform.Rotate(new Vector3(-Vector3.Distance(transform.position, goL_playerObjects[i_targetIndex].transform.position) * 0.5f, 0, 0));
            _go_rock.GetComponent<Rigidbody>().AddForce(_go_rock.transform.forward * f_throwPower, ForceMode.Impulse);
            _go_rock.transform.rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);

            Invoke("UseAttack", 4);
        }
    }

    private void RockWall()
    {
        GameObject[] _goA_chunks = new GameObject[i_rocksToSpawn];

        for (int i = 0; i < i_rocksToSpawn; i++)
        {
            _goA_chunks[i] = Instantiate(go_smallLodePrefab, (transform.position + (transform.forward * ((i + 1) * f_spaceBetweenChunks))) - new Vector3(0, 5, 0), new Quaternion(0, Random.value, 0, Random.value), transform.root);
        }

        if (gameObject.activeSelf)
            StartCoroutine(MoveWallChunks(_goA_chunks, 10));
    }

    private IEnumerator MoveWallChunks(GameObject[] _goA_chunks, int _i_timesToMove)
    {
        yield return new WaitForSeconds(0.03f);

        for (int i = 0; i < _goA_chunks.Length; i++)
        {
            _goA_chunks[i].transform.position += new Vector3(0, f_heightPerTick, 0);
        }

        if (_i_timesToMove > 0)
            StartCoroutine(MoveWallChunks(_goA_chunks, _i_timesToMove - 1));
        else Invoke("UseAttack", 4);
    }

}