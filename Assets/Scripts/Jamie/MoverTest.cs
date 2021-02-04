using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverTest : MonoBehaviour
{

    private MoverBase mover;

    private void Start()
    {
        mover = GetComponent<MoverBase>();

    }

    private void Update()
    {
        mover.Move(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
    }

}
