using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public static FadeToBlack x;
    [SerializeField] private Image i_coverImage;


    private void Start()
    {
        if (x == null)
        {
            x = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        SceneManager.sceneLoaded += DefaultHideCover;
        HideCover(1);
    }

    private void DefaultHideCover(Scene s, LoadSceneMode m)
    {
        HideCover(1);
    }

    public void HideCover(float delay)
    {
        StartCoroutine(IChangeCover(delay, 0));
    }

    public void ShowCover(float delay)
    {
        StartCoroutine(IChangeCover(delay, 1));
    }

    public IEnumerator IChangeCover(float delay, float _f_targetAlpha)
    {
        yield return new WaitForSeconds(delay);
        i_coverImage.CrossFadeAlpha(_f_targetAlpha, 0.7f, true);
    }

}
