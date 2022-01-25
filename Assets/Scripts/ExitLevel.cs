using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;


public class ExitLevel : MonoBehaviour
{
    //config params
    [SerializeField] float lightFadeSpeed = .4f;
    [SerializeField] float lightMaxIntensity = 2f;
    [SerializeField] Text endLevelText;

    //cached refs
    Player player;
    LevelManager levelManager;
    Light2D myLight;
    bool exitActive;
    Fade fade;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        levelManager = FindObjectOfType<LevelManager>();
        myLight = GetComponent<Light2D>();
        myLight.intensity = 0;
        exitActive = false;
        fade = FindObjectOfType<Fade>();
        endLevelText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        TriggerExitOpen();
        TriggerEndLevel();
    }

    private void TriggerExitOpen()
    {
        if (levelManager.levelWon && myLight.intensity < lightMaxIntensity)
        {
            myLight.intensity += lightFadeSpeed * Time.deltaTime;
            exitActive = true;
            player.levelCompleted = true;
        }
    }

    private void TriggerEndLevel()
    {
        if (exitActive)
        {
            if(player.transform.position.x > 18)
            {
                fade.fadeOut = true;
                player.movementDisabled = true;
                levelManager.winText.enabled = false;
                endLevelText.enabled = true;
                levelManager.canReset = false;
                if (Input.GetMouseButtonDown(0))
                {
                    levelManager.LoadNextLevel();
                }
            }
        }
    }
}
