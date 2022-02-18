using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionButton : MonoBehaviour
{
    public int id;
    public string bestTime;
    public string lastTime;

    public Text bestTimeText;
    public Text lastTimeText;

    public void UpdateScore(string _bestTime, string _lastTime)
    {
        bestTime = _bestTime;
        lastTime = _lastTime;
        bestTimeText.text = bestTime;
        lastTimeText.text = lastTime;
    }
}
