using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class Shadows : MonoBehaviour
{
    enum Gift { None, Strength, Torch, Jump, Float, Reset};
    
    //config params
    [SerializeField] List<Image> inventorySlots;
    [SerializeField] float revealRate = 1f;
    [SerializeField] Gift myGift;
    [SerializeField] float engageRange = 1f;
    [SerializeField] int desiredGift = 0;
    [SerializeField] List<Pickup> collectableGifts;

    //cached refs
    [SerializeField] Sprite myWantedGift;
    Player player;
    LevelManager levelManager;
    SpriteRenderer myRenderer;
    public bool revealSelf;
    AudioManager audioManager;
    GameData gameData;
    Light2D myLight;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        levelManager = FindObjectOfType<LevelManager>();
        myRenderer = GetComponent<SpriteRenderer>();
        audioManager = FindObjectOfType<AudioManager>();
        gameData = FindObjectOfType<GameData>();
        myLight = GetComponent<Light2D>();
        myLight.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGift();
        RevealSelf();
    }

    private void CheckForGift()
    {
        var distToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distToPlayer < engageRange)
        {
            if (inventorySlots[desiredGift].sprite == myWantedGift)
            {
                levelManager.TriggerWinningEnd(true);
                inventorySlots[desiredGift].enabled = false;
                UnlockGift();
            }
            else
            {
                levelManager.levelLost = true;
                levelManager.TriggerWinningEnd(false);
            }
        }
    }

    private void UnlockGift()
    {
        switch (myGift)
        {
            case Gift.Strength:
                gameData.unlockedStrength = true;
                break;
            case Gift.Jump:
                gameData.unlockedJump = true;
                break;
            case Gift.Torch:
                gameData.unlockedTorch = true;
                break;
            case Gift.Float:
                gameData.unlockedFloat = true;
                break;
            case Gift.Reset:
                gameData.unlockedReset = true;
                break;
            default:
                break;
        }
        player.UpdateUnlocks();
        
    }

    public void RevealSelf()
    {
        if (revealSelf && myRenderer.color.b < 255)
        {
            float newRed = myRenderer.color.r + (revealRate * Time.deltaTime);
            float newBlue = myRenderer.color.b + (revealRate * Time.deltaTime);
            float newGreen = myRenderer.color.g + (revealRate * Time.deltaTime);
            myRenderer.color = new Color(newRed, newBlue, newGreen);
            myLight.enabled = true;
        }

    }
}
