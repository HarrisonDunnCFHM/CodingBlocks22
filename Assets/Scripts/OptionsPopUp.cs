using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPopUp : MonoBehaviour
{
    //config params
    [SerializeField] GameObject optionsMenu;
    [SerializeField] Vector3 hiddenPosition;
    [SerializeField] Vector3 revealedPosition;

    //cached refs
    AudioManager audioManager;
    
    // Start is called before the first frame update
    void Start()
    {
        optionsMenu.transform.localPosition = hiddenPosition;
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.ResetSliders();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleMenu()
    {
        if(optionsMenu.transform.localPosition == hiddenPosition)
        {
            optionsMenu.transform.localPosition = revealedPosition;
        }
        else
        {
            optionsMenu.transform.localPosition = hiddenPosition;
        }
    }
}
