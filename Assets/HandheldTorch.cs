using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandheldTorch : MonoBehaviour
{
    //config params
    [SerializeField] float rightX;
    [SerializeField] float leftX;
    
    //cached refs
    Player player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>(); 
    }

    // Update is called once per frame
    void Update()
    {
        SwitchHands();
    }

    private void SwitchHands()
    {
        if(player.myDirection == Player.PlayerDirection.Down || player.myDirection == Player.PlayerDirection.Right)
        {
            transform.localPosition = new Vector2(rightX, transform.localPosition.y);
            transform.localScale = new Vector2 ( 1, 1);
        }
        else if (player.myDirection == Player.PlayerDirection.Up || player.myDirection == Player.PlayerDirection.Left)
        {
            transform.localPosition = new Vector2(leftX, transform.localPosition.y);
            transform.localScale = new Vector2(-1, 1);

        }
    }
}
