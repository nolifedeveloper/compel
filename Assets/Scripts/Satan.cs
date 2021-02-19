using System.Collections;
using UnityEngine;
using EZCameraShake;
using UnityEngine.SceneManagement;

public class Satan : MonoBehaviour
{
    [Header("Movement")]
    public float xMin = -0.63f;
    public float xMax = 0.63f;
    public float yMin = -0.93f;
    public float defaultY = -0.36f;
    public float movementSpeed = 50;
    public MoveMode moveMode = MoveMode.Right;

    [Header("Health")]
    [HideInInspector]public int CURRENT_HEALTH;
    public int MAX_HEALTH = 20;

    [Header("Sound Effects")]
    public AudioClip DIE;
    public AudioClip PERISH;
    public AudioClip BURN;
    public AudioClip SPITTING;
    public AudioClip deathSound;

    #region Body Parts & Component References
    private AudioSource SATAN_AUD => GetComponent<AudioSource>();
    private AudioSource MOUTH_AUD => transform.GetChild(0).GetComponent<AudioSource>();
    //Body Parts
    public GameObject HEAD => transform.GetChild(0).gameObject;
    public ParticleSystem HEAD_PARTICLES => HEAD.transform.GetChild(1).GetComponent<ParticleSystem>();

    private Vector2 MOUTH_POSITION => HEAD.transform.GetChild(0).transform.position;
    private ParticleSystem MOUTH_PARTICLES=> HEAD.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();


    public GameObject L_ARM => transform.GetChild(1).gameObject;
    public ParticleSystem L_ARM_PARTICLES => L_ARM.transform.GetChild(0).GetComponent<ParticleSystem>();


    public GameObject R_ARM => transform.GetChild(2).gameObject;
    public ParticleSystem R_ARM_PARTICLES => R_ARM.transform.GetChild(0).GetComponent<ParticleSystem>();
    #endregion


    [Header("Movement And General")]
    public float minimumMoveSeconds = 3;
    public float maxMoveSeconds = 6;
    public GameObject[] wallsToDisable;
    public GameObject[] wallsToEnable;
    private bool hasFallen = false;
    private bool canAct = false;
    private bool canMove = false;
    private bool CAN_ACT => GameManager.instance.IS_PLAYING && canAct;
    private float ReturnRandomMoveSecond => Random.Range(minimumMoveSeconds, maxMoveSeconds);

    [Header("Hand Attack Stuff")]
    public float handAttackReactionTime = 0.2f;
    public float handAttackRetractInSeconds = 1.5f;
    public float handAttackSpeed = 2;
    public SatanHandAttackMode HAND_ATTACK_MODE;
    public SatanHandAttackMode RandomHandAttackMode => (SatanHandAttackMode)Random.Range(0, 3);

    private Coroutine HAND_ATTACK;
    private Coroutine SPIT_ATTACK;
    private Coroutine ATTACK_TIMER;

    [Header("Fire Spit Attack Stuff")]
    public float fireAttackReactionTime = 0.3f;
    public float timeBetweenFireballs = 0.45f;
    public int maximumFireballs = 6;
    public int minimumFireballs = 3;
    private Vector3 ballEulerAngle => new Vector3(0, 0, 180);

    public int ReturnRandomFireballCount => Random.Range(minimumFireballs, maximumFireballs + 1);


    private void Awake()
    {
        InitSatan();
    }

    private void Update()
    {
        if(canAct && canMove)
        {
            SatanMove();
        }
    }
    private void InitSatan()
    {        
        this.gameObject.SetActive(false);
        InGameUI.Instance.SATAN_HIMSELF = this;
    }


    public void SatanMove()
    {
        switch(moveMode)
        {
            case MoveMode.Left:

                this.transform.Translate(Vector2.left * Time.deltaTime * movementSpeed);

                if(this.transform.localPosition.x <= xMin)
                {
                    moveMode = MoveMode.Right;
                }
                break;
            case MoveMode.Right:

                this.transform.Translate(Vector2.right * Time.deltaTime * movementSpeed);

                if(this.transform.localPosition.x >= xMax)
                {
                    moveMode = MoveMode.Left;
                }
                break;
        }
    }

    #region Satan Attack
    public IEnumerator SatanSpit()
    {
        AudioStuff.SetAndPlayAudio(SATAN_AUD, BURN);
        int fireballCount = ReturnRandomFireballCount;

        yield return new WaitForSeconds(fireAttackReactionTime + BURN.length);

        while(fireballCount > 0)
        {
            FireSatanSpit();
            fireballCount--;
            yield return new WaitForSeconds(timeBetweenFireballs);
        }

        ATTACK_TIMER = StartCoroutine(AttackTimer());
    }

    public void FireSatanSpit()
    {
        CameraShaker.Instance.ShakeOnce(1, 0.7f, 0.2f, 0.2f);
        AudioStuff.SetAndPlayAudio(MOUTH_AUD, SPITTING);
        MOUTH_PARTICLES.Play();
        GameObject satanBullet = ProjectileManager.instance.ReturnSatanBullet();
        satanBullet.transform.position = MOUTH_POSITION;
        satanBullet.transform.eulerAngles = ballEulerAngle;
        satanBullet.SetActive(true);
    }

    public IEnumerator SatanGrasp()
    {
        canMove = false;
        AudioStuff.SetAndPlayAudio(SATAN_AUD, PERISH);
        CameraShaker.Instance.ShakeOnce(1, 0.7f, 0.2f, 0.2f);
        yield return new WaitForSeconds(handAttackReactionTime);

        HAND_ATTACK_MODE = RandomHandAttackMode;

        float t = 0;

        while(t < 1)
        {
            t = Mathf.Clamp(t + (Time.deltaTime * handAttackSpeed), 0, 1);

            switch (HAND_ATTACK_MODE)
            {
                case SatanHandAttackMode.BothHands:
                    L_ARM.transform.localPosition = Vector2.Lerp(L_ARM.transform.localPosition, new Vector2(L_ARM.transform.localPosition.x, yMin),t);
                    R_ARM.transform.localPosition = Vector2.Lerp(R_ARM.transform.localPosition, new Vector2(R_ARM.transform.localPosition.x, yMin), t);
                    break;

                case SatanHandAttackMode.LHand:
                    L_ARM.transform.localPosition = Vector2.Lerp(L_ARM.transform.localPosition, new Vector2(L_ARM.transform.localPosition.x, yMin), t);
                    break;

                case SatanHandAttackMode.RHand:
                    R_ARM.transform.localPosition = Vector2.Lerp(R_ARM.transform.localPosition, new Vector2(R_ARM.transform.localPosition.x, yMin), t);
                    break;
            }

            yield return null;
        }

       yield return new WaitForSeconds(handAttackRetractInSeconds);
       t = 0;           

        while (t < 1)
        {
            t = Mathf.Clamp(t + (Time.deltaTime * handAttackSpeed), 0, 1);

            switch (HAND_ATTACK_MODE)
            {
                case SatanHandAttackMode.BothHands:
                    L_ARM.transform.localPosition = Vector2.Lerp(L_ARM.transform.localPosition, new Vector2(L_ARM.transform.localPosition.x, defaultY), t);
                    R_ARM.transform.localPosition = Vector2.Lerp(R_ARM.transform.localPosition, new Vector2(R_ARM.transform.localPosition.x, defaultY), t);
                    break;

                case SatanHandAttackMode.LHand:
                    L_ARM.transform.localPosition = Vector2.Lerp(L_ARM.transform.localPosition, new Vector2(L_ARM.transform.localPosition.x, defaultY), t);
                    break;

                case SatanHandAttackMode.RHand:
                    R_ARM.transform.localPosition = Vector2.Lerp(R_ARM.transform.localPosition, new Vector2(R_ARM.transform.localPosition.x, defaultY), t);
                    break;
            }
            yield return null;
        }

        canMove = true;
        ATTACK_TIMER = StartCoroutine(AttackTimer());
    }

    public IEnumerator AttackTimer()
    {
        float timeToWait = ReturnRandomMoveSecond;
        yield return new WaitForSeconds(timeToWait);
        int boolVal = Random.Range(0, 2);
        bool attackMode = boolVal == 0 ? true : false;

        if(attackMode)
        {
            SPIT_ATTACK = StartCoroutine(SatanSpit());
        }
        else
        {
           HAND_ATTACK = StartCoroutine(SatanGrasp());
        }
    }
    #endregion

    public IEnumerator UnleashSatan()
    {
        RoomManager.instance.SetCheckpoint(true);
        this.gameObject.SetActive(true);
        CURRENT_HEALTH = MAX_HEALTH;
        HEAD_PARTICLES.Play();
        L_ARM_PARTICLES.Play();
        R_ARM_PARTICLES.Play();
        AudioStuff.SetAndPlayAudio(SATAN_AUD, DIE);
        yield return new WaitForSeconds(DIE.length);
        ProjectileManager.instance.PRIEST.CanAct(true);
        canAct = true;
        canMove = true;
        ATTACK_TIMER = StartCoroutine(AttackTimer());
    }

    public void DamageSatan()
    {
        CURRENT_HEALTH--;
        InGameUI.Instance.UpdateBossHealthBar();

        if(CURRENT_HEALTH <= 0 && !hasFallen)
        {
            StartCoroutine(SatanDeath());
        }
    }

    [ContextMenu("Kill Satan.")]
    private void KillSatan()
    {
        StartCoroutine(SatanDeath());
    }

    private IEnumerator SatanDeath()
    {
        RoomManager.instance.SetCheckpoint(false);
        CameraShaker.Instance.ShakeOnce(1.5f, 1.2f, 1, 2);
        DisableSatanColliders();
        canMove = false;
        StopSatanAttacks();
        SATAN_AUD.Stop();
        GameManager.instance.InitiateEndGame();
        HEAD_PARTICLES.Play();
        L_ARM_PARTICLES.Play();
        R_ARM_PARTICLES.Play();
        DisableSatanVisuals();
        SetObjectsInArray(wallsToDisable, false);
        SetObjectsInArray(wallsToEnable, true);
        AudioStuff.SetAndPlayAudio(SATAN_AUD, deathSound);
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(1);
    }

    [ContextMenu("Stop attacks")]
    private void StopSatanAttacks()
    {
        if(HAND_ATTACK != null)
        {
            StopCoroutine(HAND_ATTACK);
        }
        if(SPIT_ATTACK != null)
        {
            StopCoroutine(SPIT_ATTACK);
        }
        if(ATTACK_TIMER != null)
        {
            StopCoroutine(ATTACK_TIMER);
        }
    }

    private void DisableSatanVisuals()
    {
        HEAD.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        L_ARM.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        R_ARM.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void DisableSatanColliders()
    {
        HEAD.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        L_ARM.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        R_ARM.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void SetObjectsInArray(GameObject[] arr,bool value)
    {
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i].SetActive(value);
        }
    }
}

public enum SatanHandAttackMode
{
    LHand = 0,
    RHand = 1,
    BothHands = 2,
}

public enum MoveMode
{
    Left,
    Right
}
