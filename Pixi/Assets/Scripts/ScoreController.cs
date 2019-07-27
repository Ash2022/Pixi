using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance;

    private void Awake()
    {
        Instance = this;
    }
    
}
