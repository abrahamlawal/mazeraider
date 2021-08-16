//namespaces as usual
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//this class is inheriting from monobehavior, and making use of the ipointer interface for detecting mouse events
public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //public variables
    public string nameValue; //name of scene to load or url string to open

    public GameObject highlightUI; //lwhite line UI for currently highlighted button
    public AudioClip highlightSFX; //hover audio sfx to play
    public AudioClip selectedSFX; //click audio sfx to play

    //private variables
    GameObject MusicManager; //game objects our audio source component is attached to
    GameObject SFXManager;

    AudioSource musicSource; //audio source components for playing audio
    AudioSource sfxSource;

    bool fadeMusic = false;

    //this function is called as game scene starts
    void Start()
    {
        //we find the music and sfx game objects in game scene and retrieve their audio source components
        MusicManager = GameObject.Find("MusicManager");
        SFXManager = GameObject.Find("SFXManager");
        musicSource = MusicManager.GetComponent<AudioSource>();
        sfxSource = SFXManager.GetComponent<AudioSource>();
    }

    //this function is called every frame (i.e every milliseconds)
    void Update()
    {
        //if fadeMusic is true and music audio source component volume is greater than 0, then we reduce the volume by 0.3, every frame
        if (fadeMusic && musicSource.volume > 0f)
        {
            musicSource.volume -= Time.deltaTime * 0.3f;
        }
    }

    //this function is called when mouse arrow hovers or enters a UI object (text, button)
    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightUI.SetActive(true); //shows the hover white line UI

        sfxSource.PlayOneShot(highlightSFX); //play mouse hover sound effect once
    }

    //this function is called when mouse arrow leaves a UI object (text, button)
    public void OnPointerExit(PointerEventData eventData)
    {
        highlightUI.SetActive(false); //hides the hover white line UI
    }

    //this function is called when mouse clicks a UI object (text, button) that this script is attached to
    public void OnPointerClick(PointerEventData eventData)
    {
        fadeMusic = true; //now let's fade out music

        sfxSource.PlayOneShot(selectedSFX); //play button click sound effect once

        StartCoroutine(SwitchScene()); //calls the ienumerator function, we can implement delays between lines of code
    }

    //This function is responsible for switching scene or opening the URL to source code & github repo
    IEnumerator SwitchScene()
    {
        yield return new WaitWhile(()=> sfxSource.isPlaying); //wait for sound effect to finish playing before loading scene

        if (nameValue.Contains("Stage")) //if the name provided contains the string "Stage" then we know we need to load a scene
        {
            SceneManager.LoadScene(nameValue); //loads the scene that matches the provided name
        } else 
        { 
            OpenURL(); //opens url of github repo and source code in browser
        }
    }

    public void OpenURL()
    {
        Application.OpenURL(nameValue); //opens a url in browser
    }
}
