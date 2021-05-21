using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScroller : MonoBehaviour
{
    [SerializeField] private float f_scrollSpeed;
    [Space, SerializeField] private GameObjectArray[] goAA_bodys = new GameObjectArray[0];


    void Update()
    {

        if (transform.localPosition.y > 1100)
        {
            if (Input.anyKeyDown)
                SceneManager.LoadScene(0);
            return;
        }

        transform.localPosition += Vector3.up * Time.deltaTime * f_scrollSpeed * (Input.anyKey ? 3 : 1);
    }


    internal void UpdateApearance(int _i_head, int _i_body, int _i_limbs)
    {

    }

    [System.Serializable]
    private struct GameObjectArray
    {
        GameObject[] goA;
    }

}