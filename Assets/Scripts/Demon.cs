using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Demon : MonoBehaviour
{
    protected bool isDead = false;
    protected Room demonRoom
    {
        get
        {
            return gameObject.GetComponentInParent<Room>();
        }
    }
    private bool canAct = false;
    protected bool ABLE_TO_ACT => (GameManager.instance.IS_PLAYING && canAct && !isDead);
    public float totalParticleDelay => particle.main.duration + particle.main.startLifetime.constant;

    [Header("Variables")]
    public float movementSpeed = 38;
    protected float movementSpeedFormula => ABLE_TO_ACT ? movementSpeed : 0;
    public AudioClip spawnSound;
    public AudioClip deathSound;

    protected BoxCollider2D boxColl => GetComponent<BoxCollider2D>();
    protected Rigidbody2D rb => GetComponent<Rigidbody2D>();
    protected SpriteRenderer sr => GetComponent<SpriteRenderer>();
    protected AudioSource audioSource => GetComponent<AudioSource>();

    public ParticleSystem particle => transform.GetChild(0).GetComponent<ParticleSystem>();

    private Vector2 defaultPosition;
    private Vector3 defaultRotation;

    protected Vector2 playerPosition
    {
        get
        {
            return ProjectileManager.instance.PRIEST.gameObject.transform.position;
        }
    }

    private void Start()
    {
        this.defaultPosition = this.transform.localPosition;
        this.defaultRotation = this.transform.localEulerAngles;
        demonRoom.DEMONS.Add(this);
        gameObject.SetActive(false);      
    }



    public virtual void Death()
    {
        if(!isDead)
        {
            StartCoroutine(OnDeath());
        }
    }

    public void CanAct(bool val)
    {
        canAct = val;
    }

    public IEnumerator OnDeath()
    {
        AudioStuff.SetAndPlayAudio(audioSource, deathSound);
        isDead = true;
        CanAct(false);
        sr.enabled = false;
        boxColl.enabled = false;
        rb.velocity = Vector2.zero;
        particle.Play();
        yield return new WaitForSeconds(totalParticleDelay);
        gameObject.SetActive(false);
        demonRoom.CheckDemons();
    }

    public virtual IEnumerator OnSpawn()
    {
        this.GetComponent<SoundSource>().InitSource();
        AudioStuff.SetAndPlayAudio(audioSource, spawnSound);
        isDead = false;
        this.transform.localPosition = this.defaultPosition;
        this.transform.localEulerAngles = this.defaultRotation;
        CanAct(false);
        sr.enabled = true;
        boxColl.enabled = true;
        particle.Play();
        yield return new WaitForSeconds(totalParticleDelay);
    }
}
