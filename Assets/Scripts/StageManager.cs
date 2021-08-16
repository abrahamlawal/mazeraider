using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    [Header("Scene Variables")]
    public int noOfOrbs;
    public int noOfOrbsCollected;
    public float timeToComplete;

    [Header("UI Elements")]
    public Text orbsRemaining;
    public Text Timer;
    public Text extraTip;
    public GameObject pausePanel;

    public bool timerIsRunning = false;
    Character character;

    [Header("Sounds")]

    public AudioSource source;
    public AudioSource musicSource;
    public AudioClip doorLocked;
    public AudioClip timer;
    public AudioClip complete;
    public AudioClip failed;
    AudioSource[] audioSources;

    //get and cache components required by some functions in this class
    private void Start()
    {
        character = FindObjectOfType<Character>();
        audioSources = FindObjectsOfType<AudioSource>();
        UpdateUI();
        timerIsRunning = true;
        Timer.text = "200";
    }

    //display player progress in HUD
    public void UpdateUI()
    {
        orbsRemaining.text = noOfOrbsCollected.ToString() + " OF " + noOfOrbs.ToString() + " COLLECTED";
    }
    void Update()
    {
        //we reduce timer by 1sec and display on HUD
        if (timerIsRunning)
        {
            if (timeToComplete > 0)
            {
                timeToComplete -= Time.deltaTime;
                Timer.text = Mathf.FloorToInt(timeToComplete).ToString();
            }
            else
            {
                Debug.Log("Time has run out!");
                timeToComplete = 0;
                Timer.text = "0";
                timerIsRunning = false;
                source.PlayOneShot(failed);
                musicSource.Stop();
                Invoke(nameof(LoadGameOver), 4f);
            }

            //if character dies, then show game over screen

            if (character.hasDied)
            {
                timerIsRunning = false;
                source.PlayOneShot(failed);
                musicSource.Stop();
                Invoke(nameof(LoadGameOver), 4f);
            }

            //if timer is less than 30secs, play countdown sound effect
            if (timeToComplete < 30 && timerIsRunning)
            {
                if (!source.isPlaying)
                {
                    source.PlayOneShot(timer);
                }
            }
        }

        //change the timer text UI color to red, so player knows they don't have much time left

        if (timeToComplete < 30)
        {
            Timer.color = Color.red;
        }

        //if player presses escape key, then pause or unpause the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpauseGame();
        }

        
    }

    //function to show stage complete scene
    public void LoadGameWin()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("WinStage");
    }

    //function to show stage failed scene
    void LoadGameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("LoseStage");
    }

    //if game is paused, then unpause the game, and vice versa.
    void PauseUnpauseGame()
    {
        if (Time.timeScale == 0f)
        {
            pausePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;

            //unpause all sounds in scene
            foreach (AudioSource a in audioSources)
            {
                a.UnPause();
            }
        }
        else
        {
            pausePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;

            //pause all sounds in scene
            foreach(AudioSource a in audioSources)
            {
                a.Pause();
            }
        }
    }

    //function to show game main menu scene
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuStage");
    }

}
