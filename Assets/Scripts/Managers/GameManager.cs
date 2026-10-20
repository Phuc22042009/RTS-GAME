using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameManager instance;
    public ObjectPool objectPool;

    private void Awake()
    {
        if (instance != null && instance != this)
        {   
        Destroy(this);
        return;
        }
        instance = this;
    }
}
