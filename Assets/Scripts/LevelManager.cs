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
    Player player;

    
    // Start is called before the first frame update
    void Start()
    {
        pickupCollected = false;
        winText.enabled = false;
        loseText.enabled = false;
        player = FindObjectOfType<Player>();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        ResetLevel();
    }

    private void ResetLevel()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void TriggerWinningEnd(bool ending)
    {
        player.movementDisabled = true;
        if (ending)
        {
            winText.enabled = true;
        }
        else
        {
            loseText.enabled = true;
        }
    }
}
