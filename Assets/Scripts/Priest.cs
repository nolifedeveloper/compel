using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraShake;

[RequireComponent(typeof(Rigidbody2D))]
public class Priest : MonoBehaviour
{
    [Header("Shooting")]
    public Sprite graveStone;
    public AudioClip revolverSound;
    public float fireRateInSeconds = 0.15f;
    private Transform barrelTransform => this.transform.GetChild(0).transform;
    private ParticleSystem gunParticle => this.transform.GetChild(0).GetComponent<ParticleSystem>();
    private bool shootReady = true;
    private bool canShoot = true;

    private AudioSource audioSource => this.GetComponent<AudioSource>();

    [Header("Movement")]
    public float movementSpeed = 5;
    private Vector2 movementDirection => new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
    private Vector2 movementFormula => canMove && GameManager.instance.IS_PLAYING ? movementDirection * Time.deltaTime * movementSpeed : Vector2.zero;
    private Rigidbody2D rb => this.GetComponent<Rigidbody2D>();
    private bool canMove = true;
    private bool canLookAt = true;


    private void Awake()
    {
        ProjectileManager.instance.PRIEST = this;
    }

    private void FixedUpdate()
    {
        Movement(); 
    }


    private void Update()
    {
        LookAtMousePos();
        ShootControl();
    }

    #region Shooting

    private void ShootControl()
    {
        if(shootReady && Input.GetMouseButtonDown(0) && canShoot && GameManager.instance.IS_PLAYING)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        shootReady = false;
        gunParticle.Play();
        CameraShaker.Instance.ShakeOnce(1, 0.7f, 0.2f, 0.2f);
        AudioStuff.SetAndPlayAudio(audioSource, revolverSound);
        GameObject bullet = ProjectileManager.instance.ReturnPriestBullet();
        bullet.transform.position = barrelTransform.transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.SetActive(true);
        yield return new WaitForSeconds(fireRateInSeconds);
        shootReady = true;
    }

    #endregion

    #region Character Rotation
    private void LookAtMousePos()
    {
        if(canLookAt && GameManager.instance.IS_PLAYING)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float AngleRad = Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x);
            float AngleDeg = (180 / Mathf.PI) * AngleRad - 90;

            transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
        }
    }
    #endregion

    private void Movement()
    {
        rb.velocity = movementFormula;
    }

    //Can be changed to more.
    public void Death()
    {
        CanAct(false);
        this.gameObject.GetComponent<SpriteRenderer>().sprite = graveStone;
        GameManager.instance.GameOver();
    }

    public void CanAct(bool val)
    {
        canMove = val;
        canShoot = val;
        canLookAt = val;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Satan satan = collision.gameObject.GetComponentInParent<Satan>();

        if(satan != null)
        {
            Death();
        }
    }
}

public static class AudioStuff
{
    public static void SetAndPlayAudio(AudioSource aud,AudioClip audClip)
    {
        aud.clip = audClip;
        aud.Play();
    }
}
