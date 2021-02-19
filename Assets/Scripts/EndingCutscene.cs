using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCutscene : MonoBehaviour
{
    [Header("GameObject References")]
    public GameObject Priest;
    public GameObject Car;
    private ParticleSystem exhaustL => Car.transform.GetChild(0).GetComponent<ParticleSystem>();
    private ParticleSystem exhaustR => Car.transform.GetChild(1).GetComponent<ParticleSystem>();
    [Header("UI References")]
    public GameObject title;
    public GameObject buttons;
    [Header("Transform References")]
    public Transform priestTarget;
    public Transform carTarget;

    private Vector2 priestStartPos;
    private Vector2 carStartPos;

    [Header("Speed Values")]
    public float priestSpeed = 1.5f;
    public float carSpeed = 2.5f;
    [Header("Time Values In Seconds")]
    public float waitBeforeEnteringTheCar = 0.2f;
    public float waitBeforeStartingTheCar = 0.5f;
    public float waitBeforeSteeringTheCar = 0.5f;
    public float waitBeforeActivatingTitle = 1.2f;
    [Header("Audio Clips")]
    public AudioClip compelledClip;
    public AudioClip enteringCar;
    public AudioClip carStart;
    public AudioClip carMoving;

    private AudioSource audioSource => this.GetComponent<AudioSource>();
    private AudioSource carAudioSource => Car.GetComponent<AudioSource>();
    private AudioSource priestAudioSource => Priest.GetComponent<AudioSource>();

    private void Start()
    {
        priestStartPos = Priest.transform.position;
        carStartPos = Car.transform.position;

        StartCoroutine(Ending());
    }

    private IEnumerator Ending()
    {
        float lerpP = 0;

        while(lerpP < 1)
        {
            lerpP = Mathf.Clamp(lerpP + (Time.deltaTime * priestSpeed), 0, 1);
            Priest.transform.position = Vector2.Lerp(priestStartPos, transform.TransformPoint(priestTarget.position), lerpP);
            yield return null;
        }

        yield return new WaitForSeconds(waitBeforeEnteringTheCar);

        Priest.GetComponent<SpriteRenderer>().enabled = false;

        AudioStuff.SetAndPlayAudio(priestAudioSource, enteringCar);

        yield return new WaitForSeconds(waitBeforeStartingTheCar + enteringCar.length);

        PlayExhaustParticles();

        AudioStuff.SetAndPlayAudio(carAudioSource, carStart);
        yield return new WaitForSeconds(waitBeforeSteeringTheCar + carStart.length);

        lerpP = 0;

        AudioStuff.SetAndPlayAudio(carAudioSource, carMoving);

        while(lerpP < 1)
        {
            lerpP = Mathf.Clamp(lerpP + (Time.deltaTime * carSpeed),0,1);
            Car.transform.position = Vector2.Lerp(carStartPos, transform.TransformPoint(carTarget.position), lerpP);
            yield return null;
        }


        yield return new WaitForSeconds(waitBeforeActivatingTitle + carMoving.length);

        title.SetActive(true);

        AudioStuff.SetAndPlayAudio(audioSource, compelledClip);

        yield return new WaitForSeconds(compelledClip.length + 1);

        buttons.SetActive(true);
    }

    private void PlayExhaustParticles()
    {
        exhaustL.Play();
        exhaustR.Play();
    }
}
