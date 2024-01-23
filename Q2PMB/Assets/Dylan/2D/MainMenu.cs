using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public GameObject settings;
    public GameObject menu;
    public Transform highlight;

    public Transform highButton;
    public Transform lowButton;

    private void Start()
    {
        if(PlayerPrefs.GetString("graphics") == "")
        {
            PlayerPrefs.SetString("graphics", "High");
        }


        if(PlayerPrefs.GetString("graphics") == "High")
        {
            QualitySettings.SetQualityLevel(0);

        }
        else
        {
            QualitySettings.SetQualityLevel(1);

        }
    }
    public void Play()
    {
        SceneManager.LoadScene(2);
    }

    public void Credits()
    {
        SceneManager.LoadScene(1);
    }

    public void BackSettings()
    {
        settings.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
    }
    public void Settings()
    {
        settings.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);

        if(PlayerPrefs.GetString("graphics") == "High")
        {
            highlight.localPosition = highButton.localPosition;
        }
        else
        {
            highlight.localPosition = lowButton.localPosition;
        }
    }

    public void HighButton()
    {
        QualitySettings.SetQualityLevel(0);
        PlayerPrefs.SetString("graphics", "High");
        if (PlayerPrefs.GetString("graphics") == "High")
        {
            highlight.localPosition = highButton.localPosition;
        }
        else
        {
            highlight.localPosition = lowButton.localPosition;
        }
    }

    public void LowButton()
    {
        QualitySettings.SetQualityLevel(1);
        PlayerPrefs.SetString("graphics", "Low");

        if (PlayerPrefs.GetString("graphics") == "High")
        {
            highlight.localPosition = highButton.localPosition;
        }
        else
        {
            highlight.localPosition = lowButton.localPosition;
        }
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}
