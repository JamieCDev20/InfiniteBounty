using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public static FadeToBlack x;
    [SerializeField] private Image i_coverImage;


    private IEnumerator Start()
    {
        x = this;
        yield return new WaitForSeconds(1);
        HideCover();
    }

    public void HideCover()
    {
        i_coverImage.CrossFadeAlpha(0, 0.7f, true);
    }

    public void ShowCover()
    {
        i_coverImage.CrossFadeAlpha(1, 0.7f, true);
    }

}
