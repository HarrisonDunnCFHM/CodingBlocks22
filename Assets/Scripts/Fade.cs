using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    //config params
    [SerializeField] float fadeRate = 0.1f;

    //cached refs
    SpriteRenderer myRenderer;
    public bool fadeIn;
    public bool fadeOut;

    
    // Start is called before the first frame update
    void Start()
    {
        fadeIn = true;
        fadeOut = false;
        myRenderer = GetComponent<SpriteRenderer>();
        myRenderer.color = new Color (myRenderer.color.r, myRenderer.color.g, myRenderer.color.b,1);
    }

    // Update is called once per frame
    void Update()
    {
        ManageFade();
    }

    private void ManageFade()
    {
        if(fadeOut && myRenderer.color.a >= 0)
        {
            var newAlpha = myRenderer.color.a + (fadeRate * Time.deltaTime);
            myRenderer.color = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, newAlpha);
            if (myRenderer.color.a >=1 )
            {
                myRenderer.color = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, 1);
                fadeOut = false;
            }
        }
        else if (fadeIn && myRenderer.color.a <= 1)
        {
            var newAlpha = myRenderer.color.a - (fadeRate * Time.deltaTime);
            myRenderer.color = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, newAlpha);
            if (myRenderer.color.a <= 0)
            {
                myRenderer.color = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, 0);
                fadeIn = false;
            }
        }
    }
}
