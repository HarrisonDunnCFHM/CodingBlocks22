using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] [Range(0f, 1f)] public float musicVolume;
    [SerializeField] [Range(0f, 1f)] public float effectVolume;
    [SerializeField] [Range(0f, 1f)] public float masterVolume;
    [SerializeField] Slider masterVol;
    [SerializeField] Slider effectVol;
    [SerializeField] Slider musicVol;

    private void Awake()
    {
        int numberOfManagers = FindObjectsOfType<AudioManager>().Length;
        if (numberOfManagers > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        musicSource.volume = musicVolume * masterVolume * .5f;
        UpdateSliders();
    }
    public void ResetSliders()
    {
        var sliders = FindObjectsOfType<Slider>();
        foreach (Slider slider in sliders)
        {
            if (slider.name == "Master Volume") { masterVol = slider; }
            if (slider.name == "SFX Volume") { effectVol = slider; }
            if (slider.name == "Music Volume") { musicVol = slider; }
        }
        masterVol.value = masterVolume;
        effectVol.value = effectVolume;
        musicVol.value = musicVolume;
    }

    private void UpdateSliders()
    {
        masterVolume = masterVol.value;
        effectVolume = effectVol.value;
        musicVolume = musicVol.value;
    }
}
