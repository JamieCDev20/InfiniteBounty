using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public static FadeToBlack x;
    [SerializeField] private Image i_coverImage;
    [SerializeField] private Text t_coverText;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (x != null)
        {
            if (x != this)
                Destroy(gameObject);
        }
        else
            x = this;

        SceneManager.sceneLoaded += DefaultHideCover;
        HideCover(1);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= DefaultHideCover;
    }

    private void DefaultHideCover(Scene s, LoadSceneMode m)
    {
        HideCover(1);
    }

    public void HideCover(float delay)
    {
        if (this)
            StartCoroutine(IChangeCover(delay, 0));
    }

    public void ShowCover(float delay)
    {
        if (this)
            StartCoroutine(IChangeCover(delay, 1));
    }

    public IEnumerator IChangeCover(float delay, float _f_targetAlpha)
    {
        yield return new WaitForSeconds(delay);
        i_coverImage.CrossFadeAlpha(_f_targetAlpha, 0.7f, true);
        t_coverText.CrossFadeAlpha(_f_targetAlpha, 0.7f, true);
    }

}
