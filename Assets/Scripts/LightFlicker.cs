using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class LightFlicker : MonoBehaviour
{
    //config params
    [SerializeField] float minIntensity;
    [SerializeField] float maxIntensity;
    [SerializeField] int maxFramesBetweenFlickers;
    [SerializeField] int minFramesBetweenFlickers;
    
    //cached refs
    Light2D myLight;
    int flickerFrameCount;
    
    // Start is called before the first frame update
    void Start()
    {
        myLight = GetComponent<Light2D>();
        flickerFrameCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        FlickerLight();
    }

    private void FlickerLight()
    {
        if (flickerFrameCount <= 0)
        { 
            float newIntensity = UnityEngine.Random.Range(minIntensity, maxIntensity);
            myLight.intensity = newIntensity;
            flickerFrameCount = UnityEngine.Random.Range(minFramesBetweenFlickers,maxFramesBetweenFlickers);
        }
        else
        {
            flickerFrameCount--;
        }
    }
}
