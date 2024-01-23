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


                cutscene.SetActive(false);
                Destroy(gameObject);
            }
        }



       

    }
}
