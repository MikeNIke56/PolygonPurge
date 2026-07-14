using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using static UpgradesManager;

/**
 * the main driver of the game
 */
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        None,
        MainMenu,
        InGame,
        UpgradesMenu,
        Pause
    }
    public GameState curState;
    public GameState prevState;

    public float multipleUpgradesDelayTime;
    public int curWave = 0;

    public TextMeshProUGUI waveText;

    public static GameManager i { get; private set; }

    private void Awake()
    {
        if (i != null)
        {
            Destroy(gameObject);
        }
        else
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //for testing, comment out when running game for real
        SetCurrentState(GameState.InGame);

        //SetCurrentState(GameState.MainMenu);
    }

    public void MoveScenes(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /**
     * brings up the upgrades menu and pauses the active game
     */
    public IEnumerator HandleUpgradesMenu()
    {
        SetCurrentState(GameState.UpgradesMenu);

        int tempLevel = LevelSystem.i.levelsGained;
        while(tempLevel > 0)
        {
            UpgradesManager.i.InitializeUpgradesMenu();

            yield return new WaitUntil(() => tempLevel != 
                LevelSystem.i.levelsGained);
            tempLevel = LevelSystem.i.levelsGained;

            yield return new WaitForSecondsRealtime(multipleUpgradesDelayTime);
        }

        SetCurrentState(GameState.InGame);
        Time.timeScale = 1f;
        yield return null;
    }

    public IEnumerator LoadSceneAsync(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        PlayerObjs.i.SetUpPlayerObjs();
        waveText = PlayerObjs.i.waveText;

        switch(sceneName)
        {
            case ("Arena"):
                SetCurrentState(GameState.InGame);
                break;
            case ("MainMenu"):
                SetCurrentState(GameState.MainMenu);
                break;
            default: 
                break;
        }

        yield return new WaitForSecondsRealtime(1f); 
    }

    public void SetCurrentState(GameState state)
    {
        prevState = curState;
        curState = state;
    }
    public void SetWaveNumber(int wave)
    {
        curWave = wave;
        waveText.text = wave.ToString();
    }
}
