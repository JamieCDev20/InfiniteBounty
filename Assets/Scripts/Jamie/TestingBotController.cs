using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingBotController : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private CameraController cam;

    private void Start()
    {
        cam.SetFollow(transform);
    }

    private void Update()
    {
        transform.Translate(transform.forward * Input.GetAxisRaw("Vertical") * speed * Time.deltaTime);
        transform.Translate(transform.right * Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime);

        cam.SetLookInput(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));


    }

}
