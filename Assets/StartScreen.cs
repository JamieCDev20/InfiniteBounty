using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(IStartGame());
    }

    private IEnumerator IStartGame()
    {
        FadeToBlack.x.ShowCover(0, false);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("LobbyScene");
    }

}
