using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject satanBullet;
    public GameObject demonProjectilePrefab;

    public static ProjectileManager instance;
    [HideInInspector]public Priest PRIEST;
    [HideInInspector]public List<GameObject> DEMON_PROJECTILES;
    [HideInInspector]public List<GameObject> PRIEST_BULLETS;
    [HideInInspector] public List<GameObject> SATAN_BULLETS;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
    }


    public GameObject ReturnPriestBullet()
    {
        foreach(GameObject bullet in PRIEST_BULLETS)
        {
            if(!bullet.activeSelf)
            {
                return bullet;
            }
        }

        GameObject blt = Instantiate(bulletPrefab);
        blt.SetActive(false);
        blt.gameObject.transform.SetParent(this.transform);
        PRIEST_BULLETS.Add(blt);
        return blt;
    }

    public GameObject ReturnDemonProjectile()
    {
        foreach (GameObject demonProjectile in DEMON_PROJECTILES)
        {
            if (!demonProjectile.activeSelf)
            {
                return demonProjectile;
            }
        }

        GameObject dmnPrj = Instantiate(demonProjectilePrefab);

        dmnPrj.SetActive(false);
        dmnPrj.gameObject.transform.SetParent(this.transform);
        DEMON_PROJECTILES.Add(dmnPrj);
        return dmnPrj;
    }

    public GameObject ReturnSatanBullet()
    {
        foreach (GameObject satanBlt in SATAN_BULLETS)
        {
            if (!satanBlt.activeSelf)
            {
                return satanBlt;
            }
        }

        GameObject stnBlt = Instantiate(satanBullet);

        stnBlt.SetActive(false);
        stnBlt.gameObject.transform.SetParent(this.transform);
        SATAN_BULLETS.Add(stnBlt);
        return stnBlt;
    }
}
