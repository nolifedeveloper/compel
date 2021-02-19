using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public List<GameObject> ROOM_UI_OBJECTS
    {
        get
        {
            List<GameObject> listToReturn = new List<GameObject>();

            for(int i = 0; i < this.gameObject.transform.GetChild(0).childCount; i++)
            {
                listToReturn.Add(this.gameObject.transform.GetChild(0).GetChild(i).gameObject);
            }

            return listToReturn;
        }
    }
    public List<GameObject> PATH_UI_OBJECTS
    {
        get
        {
            List<GameObject> listToReturn = new List<GameObject>();

            for (int i = 0; i < this.gameObject.transform.GetChild(1).childCount; i++)
            {
                listToReturn.Add(this.gameObject.transform.GetChild(1).GetChild(i).gameObject);
            }

            return listToReturn;
        }
    }

    public GameObject SATAN_HEALTH_BAR_OBJ => this.transform.GetChild(2).gameObject;
    private Image SATAN_HEALTH_BAR => SATAN_HEALTH_BAR_OBJ.GetComponent<Image>();
    public Satan SATAN_HIMSELF;

    private int currentRoomUIIndex = 0;
    private int currentpathUIIndex = 0;

    public static InGameUI Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance == null && Instance != this)
        {
            Destroy(this.gameObject);

        }
    }

    //Initilize boss stuff
    public void UpdateRoomUI()
    {
        ROOM_UI_OBJECTS[ROOM_UI_OBJECTS.Count - (1 + currentRoomUIIndex)].SetActive(true);
        currentRoomUIIndex++;
    }

    public void UpdatePathUI()
    {
        PATH_UI_OBJECTS[PATH_UI_OBJECTS.Count - (1 + currentpathUIIndex)].SetActive(true);
        currentpathUIIndex++;
    }

    public void UpdateUIMode()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false); //Disable room UI.
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false); //Disable path UI.
        this.gameObject.transform.GetChild(2).gameObject.SetActive(true); //Activate demon health bar.
        StartCoroutine(PrepareBossBar());
    }
    
    public IEnumerator PrepareBossBar()
    {
        float t = 0;

        while(t < 1)
        {
            t += Time.deltaTime * 2;
            SATAN_HEALTH_BAR.fillAmount = t;
            yield return null;
        }

        UpdateBossHealthBar();
    }

    public void UpdateBossHealthBar()
    {
        float amount = (float)SATAN_HIMSELF.CURRENT_HEALTH / (float)SATAN_HIMSELF.MAX_HEALTH;
        SATAN_HEALTH_BAR.fillAmount = amount;
    }
}
