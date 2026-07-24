using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteEntity : MonoBehaviour
{
    public static MeteoriteEntity Instance { get; private set; }

    public RequestProperty[] RequestProperties;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}

[Serializable]
public class RequestProperty
{
    public Color RequestColor;
    public RequestType RequestType;
}

public enum RequestType
{
    TurnOnTheLight,
    TurnOffTheLight,
    Cooling,
    Heating,
    ShowBrainrot,
    ShowNonFiction,
    Insult,
    Praise,
    Food
}