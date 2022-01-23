using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    //unlocks 
    public bool unlockedStrength;
    public bool unlockedTorch;
    public bool unlockedJump;

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
        unlockedStrength = false;
        unlockedTorch = false;
        unlockedJump = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
