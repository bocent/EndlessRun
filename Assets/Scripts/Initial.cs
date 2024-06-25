using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initial : MonoBehaviour
{
    public bool isDev = false;

    public static Initial Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
