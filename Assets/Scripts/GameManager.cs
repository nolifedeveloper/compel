using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*TODO
 * How to start the game?
 * Enemy AI. (Pathfinding, Combat)
 * Room generation
 * BOSS (Art, Design, Programming)
 * Room design (programming, design) and a better way to design rooms.
*/
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public GameObject PREGAME_UI;
    public GameObject INGAME_UI;
    public GameObject GAMEOVER_UI;
    public GameObject PAUSE_UI;

    public Texture2D CrosshairCursor;

    private List<GameObject> UI_LIST => new List<GameObject>() { PREGAME_UI, INGAME_UI, GAMEOVER_UI, PAUSE_UI };

    public bool IS_PLAYING
    {
        get
        {
            if(GAME_STATE == GameState.Playing)
            {
                return true;
            }
            
            else
            {
                return false;
            }
        }
    }


    [Header("Announcer Clips")]
    public AudioClip DOOMED;
    public AudioClip COMPEL;
    private AudioSource src => GetComponent<AudioSource>(); 

    private GameState GAME_STATE = GameState.PreStart;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this && instance != null)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        OnGameModeChanged();
    }


    private void Update()
    {
        ManageInput();
    }

    private void ManageInput()
    {
        StartingInput();
        PauseOrQuitInput();
        RestartInput();
    }

    private void StartingInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(GAME_STATE == GameState.PreStart)
            {
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        GAME_STATE = GameState.Playing;
        RoomManager.instance.StartingRoom();
        OnGameModeChanged();
    }

    public void InitiateEndGame()
    {
        GAME_STATE = GameState.Endgame;
        OnGameModeChanged();
    }
    private void PauseOrQuitInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            switch (GAME_STATE)
            {
                case GameState.PreStart:
                    QuitGame();
                    break;
                case GameState.Playing:
                    PauseGame();
                    OnGameModeChanged();
                    break;
                case GameState.Paused:
                    ResumeGame();
                    OnGameModeChanged();
                    break;
                case GameState.Over:
                    QuitGame();
                    break;
            }
        }
    }

      
    private void RestartInput()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(GAME_STATE == GameState.Over || GAME_STATE == GameState.Paused)
            {
                RestartGame();
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
    }

    public void GameOver()
    {
        GAME_STATE = GameState.Over;
        Time.timeScale = 0;
        OnGameModeChanged();
    }

    public void PauseGame()
    {
        GAME_STATE = GameState.Paused;
        Time.timeScale = 0;
        AudioManager.instance.PauseAllAudioSources();
        OnGameModeChanged();
    }


    public void ResumeGame()
    {
        GAME_STATE = GameState.Playing;
        Time.timeScale = 1;
        AudioManager.instance.ResumeAllAudioSources();
        OnGameModeChanged();
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    private void CursorChanger()
    {
        if(IS_PLAYING)
        {
            Cursor.SetCursor(CrosshairCursor, new Vector2(7,7), CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null,Vector2.zero,CursorMode.Auto);
        }
    }

    private void OnGameModeChanged()
    {
        CursorChanger();
        
        if(InGameUI.Instance.SATAN_HIMSELF != null)
        {
            InGameUI.Instance.UpdateBossHealthBar();
        }

        switch(GAME_STATE)
        {
            case GameState.PreStart:
                UICheck(PREGAME_UI);
                AudioStuff.SetAndPlayAudio(src, COMPEL);
                break;
            case GameState.Playing:
                UICheck(INGAME_UI);
                break;
            case GameState.Paused:
                UICheck(PAUSE_UI);
                break;
            case GameState.Over:
                UICheck(GAMEOVER_UI);
                AudioStuff.SetAndPlayAudio(src, DOOMED);
                break;
            case GameState.Endgame:
                {
                    UICheck(null);
                    break;
                }
        }
    }

    private void UICheck(GameObject currentMode)
    {
        for(int i = 0; i < UI_LIST.Count; i++)
        {
            if(UI_LIST[i] == currentMode)
            {
                UI_LIST[i].SetActive(true);
            }
            else
            {
                UI_LIST[i].SetActive(false);
            }
        }
    }

}

public enum GameState
{
    PreStart,
    Playing,
    Paused,
    Over,
    Endgame,
}