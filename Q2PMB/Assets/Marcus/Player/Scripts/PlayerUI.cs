using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    public GameObject pauseRoot;
    public CameraMovement cam;
    public GunController gun;
    
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(pauseRoot.activeInHierarchy)
            {
                cam.isActive = true;
                gun.enabled = true;
                pauseRoot.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {

                cam.isActive = false;
                gun.enabled = false;
                pauseRoot.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }


    public void Resume()
    {
        cam.isActive = true;
        gun.enabled = true;
        pauseRoot.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        SceneManager.LoadScene("Start");

    }
}
