using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapScore
{
    public int id;
    public string bestTime;
    public string lastTime;
}

[Serializable]
public class MapScores
{
    public List<MapScore> mapScores;
}
