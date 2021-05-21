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
                StartCoroutine(EndScene());
            return;
        }

        transform.localPosition += Vector3.up * Time.deltaTime * f_scrollSpeed * (Input.anyKey ? 3 : 1);
    }

    internal void UpdateApearance(int _i_head, int _i_body, int _i_limbs)
    {
        goAA_bodys[0].goA[_i_head].SetActive(true);
        goAA_bodys[1].goA[_i_body].SetActive(true);
        goAA_bodys[2].goA[_i_limbs].SetActive(true);
    }

    private IEnumerator EndScene()
    {
        FadeToBlack.x.ShowCover(0);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

    [System.Serializable]
    private struct GameObjectArray
    {
        public GameObject[] goA;
    }

}