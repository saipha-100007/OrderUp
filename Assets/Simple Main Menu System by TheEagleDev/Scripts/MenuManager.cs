using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MenuManager : MonoBehaviour
{
    [Header("Menu Variables")]
    public string gameName = "Placeholder";

    [Header("Components")]
    public TextMeshProUGUI titleText;

    [Header("Social Media Buttons")]
    public SocialMediaButton buttonOne;
    public SocialMediaButton buttonTwo;
    public SocialMediaButton buttonThree;
    public SocialMediaButton buttonFour;

    [Header("Button Actions")]
    public UnityEvent playButtonEvent;
    public UnityEvent optionsButtonEvent;
    public UnityEvent creditsButtonEvent;
    public UnityEvent quitButtonEvent;

    void Update()
    {
        if(titleText.text !=  gameName) titleText.text = gameName;

        if (buttonOne.prevIcon != buttonOne.icon) { buttonOne.button.GetComponent<Image>().sprite = buttonOne.icon; buttonOne.prevIcon = buttonOne.icon; }
        if (buttonTwo.prevIcon != buttonTwo.icon) { buttonTwo.button.GetComponent<Image>().sprite = buttonTwo.icon; buttonTwo.prevIcon = buttonTwo.icon; }
        if (buttonThree.prevIcon != buttonThree.icon) { buttonThree.button.GetComponent<Image>().sprite = buttonThree.icon; buttonThree.prevIcon = buttonThree.icon; }
        if (buttonFour.prevIcon != buttonFour.icon) { buttonFour.button.GetComponent<Image>().sprite = buttonFour.icon; buttonFour.prevIcon = buttonFour.icon; }
    }

    public void InvokePlayButton() => playButtonEvent?.Invoke();
    public void InvokeOptionsButton() => optionsButtonEvent?.Invoke();
    public void InvokeCreditsButton() => creditsButtonEvent?.Invoke();
    public void InvokeQuitButton() => quitButtonEvent?.Invoke();

    public void CloseApplication() => Application.Quit();
    public void SetScene(string sceneName = "SampleScene") => SceneManager.LoadScene(sceneName);

    public void InvokeSocialMediaButtonOne() => Application.OpenURL(buttonOne.link);
    public void InvokeSocialMediaButtonTwo() => Application.OpenURL(buttonTwo.link);
    public void InvokeSocialMediaButtonThree() => Application.OpenURL(buttonThree.link);
    public void InvokeSocialMediaButtonFour() => Application.OpenURL(buttonFour.link);
}

[System.Serializable]
public class SocialMediaButton
{
    public bool enabled = true;
    public string link;
    public Sprite icon;
    [HideInInspector] public Sprite prevIcon;
    public GameObject button;
}
