using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shadows : MonoBehaviour
{
    //config params
    [SerializeField] Image inventorySlot1;
    [SerializeField] float revealRate = 1f;

    //cached refs
    Sprite inventorySprite;
    Sprite collectableGift;
    Player player;
    LevelManager levelManager;
    SpriteRenderer myRenderer;
    public bool revealSelf;
    
    // Start is called before the first frame update
    void Start()
    {
        collectableGift = FindObjectOfType<Pickup>().GetComponent<SpriteRenderer>().sprite;
        player = FindObjectOfType<Player>();
        levelManager = FindObjectOfType<LevelManager>();
        myRenderer = GetComponent<SpriteRenderer>();
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
        if (distToPlayer < transform.localScale.x)
        {
            if (inventorySlot1.sprite == collectableGift)
            {
                levelManager.TriggerWinningEnd(true);
                inventorySlot1.enabled = false;
            }
            else
            {
                levelManager.TriggerWinningEnd(false);
            }
        }
    }

    public void RevealSelf()
    {
        if (revealSelf && myRenderer.color.b < 255)
        {
            float newRed = myRenderer.color.r + (revealRate * Time.deltaTime);
            float newBlue = myRenderer.color.b + (revealRate * Time.deltaTime);
            float newGreen = myRenderer.color.g + (revealRate * Time.deltaTime);
            myRenderer.color = new Color(newRed, newBlue, newGreen);
        }

    }
}
