using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class RangedDemon : Demon
{
    private Transform demonGunBarrel => gameObject.transform.GetChild(1);
    public AudioClip demonGunSound;
    private ParticleSystem gunParticle => demonGunBarrel.GetComponent<ParticleSystem>();
    private Vector2 targetPos => demonRoom.RandomPosition;
    private bool canShoot = true;


    private bool shouldMove = false;
    private bool destinationSet = false;
    private Vector2 target;

    private float lookAtPlayerPercentage = 0;

    private float lookAtTargetPosPercentage = 0;


    private void Update()
    {
        if(ABLE_TO_ACT)
        {
            LookAt(playerPosition, ref lookAtPlayerPercentage);

            if(canShoot)
            {
                LookAt(playerPosition, ref lookAtPlayerPercentage);

                if(lookAtPlayerPercentage >= 1)
                {
                    Shoot();
                }
            }
            
            else if(shouldMove)
            {
                if(!destinationSet)
                {
                    target = targetPos;
                    destinationSet = true;
                }

                LookAtLocal(target, ref lookAtTargetPosPercentage);

                if(lookAtTargetPosPercentage >= 1)
                {
                    Move();
                }
            }
        }
    }

    private void ResetAll()
    {
        canShoot = true;
        shouldMove = false;
        destinationSet = false;
        lookAtPlayerPercentage = 0;
        lookAtTargetPosPercentage = 0;
    }

    public override IEnumerator OnSpawn()
    {
        ResetAll();
        StartCoroutine(base.OnSpawn());
        yield return null;
    }

    public void Shoot()
    {
        canShoot = false;
        gunParticle.Play();
        CameraShaker.Instance.ShakeOnce(1, 0.7f, 0.2f, 0.2f);
        AudioStuff.SetAndPlayAudio(audioSource, demonGunSound);
        GameObject bullet = ProjectileManager.instance.ReturnDemonProjectile();
        bullet.transform.position = demonGunBarrel.transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.SetActive(true);
        ResetMove(); 
    }

    public void Move()
    {
        if(Vector2.Distance(this.transform.localPosition,target) > 0.25f)
        {
            rb.velocity = movementSpeed * Time.deltaTime * (target - (Vector2)transform.localPosition).normalized;
        }
        else
        {
            rb.velocity = Vector2.zero;
            shouldMove = false;
            ResetShoot();
        }
    }

    public void ResetShoot()
    {
        lookAtPlayerPercentage = 0;
        canShoot = true;
    }

    public void ResetMove()
    {
        lookAtTargetPosPercentage = 0;
        shouldMove = true;
        destinationSet = false;
    }


    public void LookAt(Vector2 target, ref float timerValue)
    {
        float AngleRad = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad - 90;
        float defaultAngle = transform.eulerAngles.z;
        AngleDeg = AngleDeg < 0 ? AngleDeg + 360 : AngleDeg;

        if(timerValue < 1)
        {
            timerValue += Time.deltaTime * 2.1f; 

            timerValue = Mathf.Clamp(timerValue, 0, 1);

            float Angle = Mathf.LerpAngle(defaultAngle, AngleDeg, timerValue);
            transform.rotation = Quaternion.Euler(0, 0, Angle);
        }      
    }

    public void LookAtLocal(Vector2 target, ref float timerValue)
    {
        float AngleRad = Mathf.Atan2(target.y - transform.localPosition.y, target.x - transform.localPosition.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad - 90;
        float defaultAngle = transform.eulerAngles.z;
        AngleDeg = AngleDeg < 0 ? AngleDeg + 360 : AngleDeg;

        if (timerValue < 1)
        {
            timerValue += Time.deltaTime * 2.1f;

            timerValue = Mathf.Clamp(timerValue, 0, 1);

            float Angle = Mathf.LerpAngle(defaultAngle, AngleDeg, timerValue);
            transform.rotation = Quaternion.Euler(0, 0, Angle);
        }
    }
}
