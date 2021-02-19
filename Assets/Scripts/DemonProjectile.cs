using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonProjectile : Bullet
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Priest priest = collision.gameObject.GetComponent<Priest>();

        if(priest != null)
        {
            priest.Death();
        }

        base.OnCollisionEnter2D(collision);
    }
}
