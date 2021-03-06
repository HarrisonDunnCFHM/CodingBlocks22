using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    //unlocks 
    public bool unlockedStrength;
    public bool unlockedTorch;
    public bool unlockedJump;
    public bool unlockedFloat;
    public bool unlockedReset;

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

    public void ResetUnlocks()
    {
        unlockedFloat = false;
        unlockedStrength = false;
        unlockedReset = false;
        unlockedJump = false;
        unlockedTorch = false;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
