using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriestBullet : Bullet
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Demon demon = collision.gameObject.GetComponent<Demon>();
        Satan satan = collision.gameObject.transform.parent.GetComponent<Satan>();

        if(demon != null)
        {
            demon.Death();
        }
        else if(satan != null)
        {
            satan.DamageSatan();
        }
       
        base.OnCollisionEnter2D(collision);
    }
}
