using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDemon : Demon
{  
    private void Update()
    {
        if(ABLE_TO_ACT)
        {
            LookAtPlayer();
        }
    }

    private void FixedUpdate()
    {      
        MoveAtPlayer();
    }

    public void LookAtPlayer()
    {
        float AngleRad = Mathf.Atan2(playerPosition.y - transform.position.y, playerPosition.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad - 90;

        transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
    }

    public void MoveAtPlayer()
    {
        rb.velocity = movementSpeedFormula* transform.up * Time.deltaTime;
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Priest layer
        if(collision.gameObject.layer == 9)
        {
            collision.gameObject.GetComponent<Priest>().Death();
        }
    }
}
