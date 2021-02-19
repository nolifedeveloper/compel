using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 50;
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private bool canMove = true;
    private Vector2 movementFormula => canMove ? bulletSpeed * transform.up * Time.deltaTime : Vector3.zero;

    private BoxCollider2D boxColl => this.GetComponent<BoxCollider2D>();
    private ParticleSystem particle => this.GetComponent<ParticleSystem>();


    private void OnEnable()
    {
        ChangeDefaultStuff(true);
    }

    private void FixedUpdate()
    {
        Move();
    }
    
    private void Move()
    {
        rb.velocity = movementFormula;
    }
    


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(BulletCollided());
        //Add stuff such as if it hits a demon, kill them or damage them before doing these etc.
    }


    public IEnumerator BulletCollided()
    {
        ChangeDefaultStuff(false);
        particle.Play();
        yield return new WaitForSeconds(particle.main.duration);
        this.gameObject.SetActive(false);
    }

    public void ChangeDefaultStuff(bool value)
    {
        this.transform.gameObject.GetComponent<SpriteRenderer>().enabled = value;
        this.boxColl.enabled = value;
        canMove = value;
    }
}

