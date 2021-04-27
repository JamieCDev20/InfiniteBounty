using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrettAdvertController : MonoBehaviour
{
    private MeshRenderer mr_mesh;
    private Vector4 up = new Vector4(0, 1, 0, 0);


    private void Start()
    {
        mr_mesh = GetComponent<MeshRenderer>();
        StartCoroutine(DoTheThing());
    }


    private IEnumerator DoTheThing()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);

            for (int i = 0; i < 100; i++)
            {
                mr_mesh.material.SetVector("Offsets", mr_mesh.material.GetVector("Offsets") + up * 0.003333333f);

                yield return new WaitForSeconds(0.01f);
            }
        }
    }


}
