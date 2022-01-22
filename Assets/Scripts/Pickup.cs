using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    //config params
    [SerializeField] Image pickedUpObject;
    
    //cached refs
    Player player;
    Sprite mySprite;
    LevelManager levelManager;
    
    // Start is called before the first frame update
    void Start()
    {
        pickedUpObject.enabled = false;
        player = FindObjectOfType<Player>();
        mySprite = GetComponent<SpriteRenderer>().sprite;
        levelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ObjectPickedUp();
    }

    private void ObjectPickedUp()
    {
        if (player.transform.position == transform.position)
        {
            pickedUpObject.enabled = true;
            levelManager.pickupCollected = true;
            pickedUpObject.sprite = mySprite;
            Destroy(gameObject);
        }
    }
}
