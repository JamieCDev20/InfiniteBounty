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

        int _i_lodeCount = UnityEngine.Random.Range(vi_numberOfLodes.x, vi_numberOfLodes.y);

        if (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.LotsOLodes))
            _i_lodeCount = Mathf.RoundToInt(vi_numberOfLodes.y * DiversifierManager.x.ReturnLodeAmountIncrease());
        else if (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.LessLodes))
            _i_lodeCount = Mathf.RoundToInt(vi_numberOfLodes.x * DiversifierManager.x.ReturnLodeAmountIncrease());


        int _i_timeAttempted = 0;

        if (b_doRayCastSpawning)
        {
            RaycastHit hit;
            for (int x = 0; x < _i_lodeCount; x++)
            {
                transform.localEulerAngles = Vector3.zero;
                transform.Rotate(new Vector3(UnityEngine.Random.Range(-85, 85), UnityEngine.Random.Range(0, 360), 0), Space.World);
                //Debug.Log(transform.eulerAngles + " | " + x);

                if (Physics.Raycast(transform.position, -transform.forward, out hit, f_zoneRadius, lm_lodeSpawnLayer, QueryTriggerInteraction.Ignore))
                {
                    if (!hit.transform.name.Contains(s_namesToIgnore) && !hit.transform.name.Contains("Lode"))
                    {
                        _go_lode = Instantiate(goA_lodesTypesToSpawn[UnityEngine.Random.Range(0, goA_lodesTypesToSpawn.Length)]);
                        //Debug.Log("spawned");

                        _lbL_spawnedLodes.Add(_go_lode.GetComponent<LodeBase>());
                        LodeBase l = _go_lode.GetComponent<LodeBase>();
                        l.SetID(LodeSynchroniser.x.RegisterLode(l));
                        _go_lode.transform.position = hit.point;
                        _go_lode.transform.up = hit.normal;
                        _go_lode.transform.Rotate(Vector3.up * UnityEngine.Random.Range(0, 360), Space.Self);
                        _go_lode.transform.localScale = Vector3.one * UnityEngine.Random.Range(v_lodeSize.x, v_lodeSize.y);

                        _go_lode.transform.localScale *= DiversifierManager.x.ReturnLodeScaler();

                        _go_lode.name += "*";
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