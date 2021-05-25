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
    [SerializeField] private Image i_postcard;


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

        i_postcard.CrossFadeAlpha(0, 0.1f, true);
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
        {
            StartCoroutine(IChangeCover(delay, 0, false));
            StartCoroutine(IChangeCover(delay, 0, true));
        }
    }

    public void ShowCover(float delay, bool _b_isPostCard)
    {
        if (this)
            StartCoroutine(IChangeCover(delay, 1, _b_isPostCard));
    }

    public IEnumerator IChangeCover(float delay, float _f_targetAlpha, bool _b_isPostCard)
    {
        yield return new WaitForSeconds(delay);
        if (_b_isPostCard)
            i_postcard.CrossFadeAlpha(_f_targetAlpha, 0.7f, true);
        else
        {
            i_coverImage.CrossFadeAlpha(_f_targetAlpha, 0.7f, true);
            t_coverText.CrossFadeAlpha(_f_targetAlpha, 0.7f, true);
        }
    }

}
