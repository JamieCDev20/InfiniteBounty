using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManagerManager : MonoBehaviour
{

    [SerializeField] private Toggle t_tutorialToggle;


    private void Start()
    {
        t_tutorialToggle.SetIsOnWithoutNotify(SaveManager.x.SaveData.Equals(null));
        ToggleTutorial();
    }

    public void ToggleTutorial()
    {
        print(t_tutorialToggle.isOn);
        TutorialManager.x.SetShouldTutorial(t_tutorialToggle.isOn);
    }

}
