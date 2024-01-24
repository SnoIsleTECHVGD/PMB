using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SkipCutscene : MonoBehaviour
{
    public GameObject text;
    public GameObject cutscene;
    public GameObject DOF;
    void Start()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        player.GetComponent<GunController>().enabled = false;
        player.GetComponent<GunController>().cameraHolder.GetComponent<CameraMovement>().enabled = false;
        player.GetComponent<GunController>().cameraHolder.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Sway>().enabled = false;

        cutscene.GetComponent<PlayableDirector>().stopped += OnCutsceneEnd;
        if (PlayerPrefs.GetInt("alreadyPlayed") == 1)
        {
            text.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("alreadyPlayed", 1);
        }
    }

    void Update()
    {
        if (text.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

                player.GetComponent<GunController>().enabled = true;
                player.GetComponent<GunController>().cameraHolder.GetComponent<CameraMovement>().enabled = true;
                player.GetComponent<GunController>().cameraHolder.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Sway>().enabled = true;


                cutscene.SetActive(false);
                Destroy(gameObject);
            }
        }
    }


    void OnCutsceneEnd(PlayableDirector director)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        player.GetComponent<GunController>().enabled = true;
        player.GetComponent<GunController>().cameraHolder.GetComponent<CameraMovement>().enabled = true;
        player.GetComponent<GunController>().cameraHolder.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Sway>().enabled = true;

        cutscene.SetActive(false);
        Destroy(gameObject);
    }
}
