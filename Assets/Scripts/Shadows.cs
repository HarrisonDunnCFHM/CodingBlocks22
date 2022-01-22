using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shadows : MonoBehaviour
{
    //config params
    [SerializeField] Image inventorySlot1;

    //cached refs
    Sprite inventorySprite;
    Sprite collectableGift;
    Player player;
    LevelManager levelManager;
    
    // Start is called before the first frame update
    void Start()
    {
        collectableGift = FindObjectOfType<Pickup>().GetComponent<SpriteRenderer>().sprite;
        player = FindObjectOfType<Player>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGift();
    }

    private void CheckForGift()
    {
        var distToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distToPlayer < transform.localScale.x)
        {
            if (inventorySlot1.sprite == collectableGift)
            {
                levelManager.TriggerWinningEnd(true);
            }
            else
            {
                levelManager.TriggerWinningEnd(false);
            }
        }
    }
}
