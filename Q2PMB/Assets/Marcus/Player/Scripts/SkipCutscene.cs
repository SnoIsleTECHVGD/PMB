using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipCutscene : MonoBehaviour
{
    public GameObject text;
    public GameObject cutscene;
    public GameObject DOF;
    void Start()
    {
        if(PlayerPrefs.GetInt("alreadyPlayed") == 1)
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
        if(text.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                cutscene.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}
