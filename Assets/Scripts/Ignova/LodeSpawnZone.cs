using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodeSpawnZone : MonoBehaviour
{
    internal float f_zoneRadius;
    [SerializeField] private Vector2Int vi_numberOfLodes;
    [SerializeField] private GameObject[] goA_lodesTypesToSpawn;
    [SerializeField] private Vector2 v_lodeSize = new Vector2(0.5f, 2);

    [Header("Raycast Things")]
    [SerializeField] private bool b_doRayCastSpawning;
    [SerializeField] private LayerMask lm_lodeSpawnLayer;
    [SerializeField] private string s_namesToIgnore = "*";

    private void Awake()
    {
        f_zoneRadius = GetComponent<SphereCollider>().radius;
    }

    internal LodeBase[] SpawnLode()
    {
        List<LodeBase> _lbL_spawnedLodes = new List<LodeBase>();
        GameObject _go_lode;
        int _i_lodeCount = 1;// UnityEngine.Random.Range(vi_numberOfLodes.x, vi_numberOfLodes.y);
        int _i_timeAttempted = 0;

        if (b_doRayCastSpawning)
        {
            RaycastHit hit;
            for (int x = 0; x < _i_lodeCount; x++)
            {
                transform.localEulerAngles = Vector3.zero;
                transform.Rotate(Vector3.right * -90);//new Vector3(UnityEngine.Random.Range(-85, 85), UnityEngine.Random.Range(0, 360), 0), Space.Self);
                //Debug.Log(transform.eulerAngles + " | " + x);

                if (Physics.Raycast(transform.position, -transform.forward, out hit, f_zoneRadius, lm_lodeSpawnLayer, QueryTriggerInteraction.Ignore))
                {
                    Debug.Log(hit.collider.name + " | " + (!hit.transform.name.Contains(s_namesToIgnore) && !hit.transform.name.Contains("Lode")));
                    if (!hit.transform.name.Contains(s_namesToIgnore) && !hit.transform.name.Contains("Lode"))
                    {
                        _go_lode = Instantiate(goA_lodesTypesToSpawn[0]);//UnityEngine.Random.Range(0, goA_lodesTypesToSpawn.Length)]);
                        Debug.Log("spawned");
                        _lbL_spawnedLodes.Add(_go_lode.GetComponent<LodeBase>());
                        _go_lode.transform.position = hit.point;
                        _go_lode.transform.up = hit.normal;
                        _go_lode.transform.Rotate(Vector3.up);// * UnityEngine.Random.Range(0, 360), Space.Self);
                        _go_lode.transform.localScale = Vector3.one;// * UnityEngine.Random.Range(v_lodeSize.x, v_lodeSize.y);
                    }
                    else
                    {
                        _i_timeAttempted++;
                        x--;
                    }
                }
                else
                {
                    _i_timeAttempted++;
                    x--;
                }

                if (_i_timeAttempted > 5)
                {
                    _i_timeAttempted = 0;
                    x++;
                }
            }
        }
        else
        {
            for (int x = 0; x < _i_lodeCount; x++)
            {
                _go_lode = Instantiate(goA_lodesTypesToSpawn[UnityEngine.Random.Range(0, goA_lodesTypesToSpawn.Length)], transform);
            }
        }
        return _lbL_spawnedLodes.ToArray();
        //print("Done with me seed");
    }

}