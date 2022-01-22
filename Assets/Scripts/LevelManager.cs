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

    public void TriggerEnding()
    {
        player.movementDisabled = true;
        if (pickupCollected)
        {
            winText.enabled = true;
        }
        else
        {
            loseText.enabled = true;
        }
    }
}
