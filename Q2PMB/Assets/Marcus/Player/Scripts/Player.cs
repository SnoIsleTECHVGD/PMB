using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class Player : HealthController
{
    [SerializeField] Transform hitParticle;

    public Volume volume;
    private Vignette vignette;
    private float previousHealth;
    private float bloodDuration = 0;
    private float timer = 0;

    public Transform Death;
    public Transform Win;

    void Start()
    {
        volume.profile.TryGet<Vignette>(out vignette);

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (CurrentHealth < previousHealth)
        {

            if (CurrentHealth <= 0)
            {
                DeathScreen();

                GetComponent<PlayerMovement>().enabled = false;
                GetComponent<GunController>().enabled = false;
                GetComponent<GunController>().cameraHolder.GetComponent<CameraMovement>().enabled = false;
                GetComponent<GunController>().cameraHolder.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Sway>().enabled = false;
            }
            timer = 0;

            if (CurrentHealth < 20)
            {
                bloodDuration = 20;

                vignette.intensity.value = .5f;
            }
            else if (CurrentHealth < 50 && CurrentHealth > 20)
            {
                bloodDuration = 10;

                vignette.intensity.value = .45f;
            }
            else if (CurrentHealth < 80 && CurrentHealth > 50)
            {
                bloodDuration = 5;

                vignette.intensity.value = .4f;
            }
            else
            {
                bloodDuration = 3;
                vignette.intensity.value = .3f;
            }

            vignette.color.value = Color.red;
        }

        previousHealth = CurrentHealth;


        if (timer < bloodDuration)
        {

        }
        else
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.21f, Time.deltaTime);
            vignette.color.value = Color.Lerp(vignette.color.value, Color.black, Time.deltaTime);

            if (CurrentHealth < MaxHealth)
            {
                CurrentHealth += regenRate * Time.deltaTime;
            }
        }



    }


    public void DeathScreen()
    {
        StartCoroutine(died());
    }

    IEnumerator died()
    {
        Death.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);

    }

    public void WinScreen()
    {
        StartCoroutine(win());
    }

    IEnumerator win()
    {
        Win.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);

    }

    public override void OnHit(Vector3 pos)
    {


        var particle = Instantiate(hitParticle);
        particle.localScale = new Vector3(.23f, .23f, .23f);
        particle.position = pos;

        Destroy(particle.gameObject, 2);
    }
}
