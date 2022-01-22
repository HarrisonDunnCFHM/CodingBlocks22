using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLayerSort : MonoBehaviour
{
    //cached refs
    SpriteRenderer myRenderer;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        SortObjectLayer();
    }

    private void SortObjectLayer()
    {
        if(player.transform.position.y > transform.position.y)
        {
            myRenderer.sortingLayerID = SortingLayer.NameToID("Objects Below Player");
        }
        else
        {
            myRenderer.sortingLayerID = SortingLayer.NameToID("Objects Above Player");
        }
    }
}
