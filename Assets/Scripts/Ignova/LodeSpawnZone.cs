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

    internal void SpawnLode(Randomness _random, int seed)
    {
        StartCoroutine(ActualPawnLodes(_random, seed));
    }
    private IEnumerator ActualPawnLodes(Randomness _random, int seed)
    {
        Random.InitState(seed);
        GameObject _go_lode;
        int _i_lodeCount = Random.Range(vi_numberOfLodes.x, vi_numberOfLodes.y);

        if (b_doRayCastSpawning)
        {
            RaycastHit hit;
            for (int x = 0; x < _i_lodeCount; x++)
            {
                yield return new WaitForEndOfFrame();
                transform.localEulerAngles = new Vector3(Random.Range(-85, 85), Random.Range(-85, 85), 0);

                if (Physics.Raycast(transform.position, -transform.forward, out hit, f_zoneRadius, lm_lodeSpawnLayer, QueryTriggerInteraction.Ignore))
                {
                    if (!hit.transform.name.Contains(s_namesToIgnore) && !hit.transform.name.Contains("Lode"))
                    {
                        _go_lode = Instantiate(goA_lodesTypesToSpawn[Random.Range(0, goA_lodesTypesToSpawn.Length)]);
                        _go_lode.transform.position = hit.point;
                        _go_lode.transform.up = hit.normal;
                        _go_lode.transform.Rotate(Vector3.up * Random.Range(0, 360), Space.Self);
                        _go_lode.transform.localScale = Vector3.one * Random.Range(v_lodeSize.x, v_lodeSize.y);
                        _random.LodeSpawned(_go_lode);
                    }
                    else x--;
                }
                else x--;
            }
        }
        else
        {
            for (int x = 0; x < _i_lodeCount; x++)
            {
                _go_lode = Instantiate(goA_lodesTypesToSpawn[Random.Range(0, goA_lodesTypesToSpawn.Length)], transform);
                _random.LodeSpawned(_go_lode);
            }
        }
    }

}