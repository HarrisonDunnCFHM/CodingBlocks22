using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //config params
    [SerializeField] Text winText;
    [SerializeField] Text loseText;

    [SerializeField] int targetFrameRate = 60;


    //cached refs
    public bool pickupCollected;
    public bool levelWon;
    Player player;
    Shadows shadow;
    Fade fadeLevel;
    bool canTransition;


    // Start is called before the first frame update
    void Start()
    {
        pickupCollected = false;
        winText.enabled = false;
        loseText.enabled = false;
        player = FindObjectOfType<Player>();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        shadow = FindObjectOfType<Shadows>();
        fadeLevel = FindObjectOfType<Fade>();
        fadeLevel.fadeIn = true;
        levelWon = false;
    }

    // Update is called once per frame
    void Update()
    {
        ResetLevel();
        ProcessFadeOut();
    }

    private void ProcessFadeOut()
    {
        if (fadeLevel.GetComponent<SpriteRenderer>().color.a == 1)
        {
            canTransition = true;
        }
        else
        {
            canTransition = false;
        }
    }

    private void ResetLevel()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(CoroutineReset());
        }
    }

    private IEnumerator CoroutineReset()
    {
        fadeLevel.fadeOut = true;
        player.movementDisabled = true;
        while (!canTransition) { yield return null; }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        StartCoroutine(CoroutineLoadNextLevel());
    }

    public IEnumerator CoroutineLoadNextLevel()
    {
        fadeLevel.fadeOut = true;
        while (!canTransition) { yield return null; }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadOptionsScene()
    {
        int optionsIndex = SceneManager.sceneCountInBuildSettings - 1;
        SceneManager.LoadScene(optionsIndex);
    }

    public void LoadCreditsScene()
    {
        int optionsIndex = SceneManager.sceneCountInBuildSettings - 2;
        SceneManager.LoadScene(optionsIndex);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void TriggerWinningEnd(bool ending)
    {
        if (ending)
        {
            winText.enabled = true;
            shadow.revealSelf = true;
            levelWon = true;
        }
        else
        {
            player.movementDisabled = true;
            loseText.enabled = true;
            winText.enabled = false;
            fadeLevel.fadeIn = false;
            fadeLevel.fadeOut = true;
            levelWon = false;
        }
    }
}
