using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Text currentSpeedText;
    public Slider speedSlider;
    public Text currentSpeedIndex;
    public Text lapText;
    public Text timerText;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdateSpeed(float _currentSpeed, float _percent, int _currentSpeedIndex)
    {
        currentSpeedText.text = ((int)_currentSpeed).ToString();
        speedSlider.value = _percent;
        currentSpeedIndex.text = _currentSpeedIndex.ToString();
    }

    public void UpdateLapText(int _lapNumber)
    {
        string newLapText = "Lap : " + _lapNumber.ToString() + "/3";
        lapText.text = newLapText;
    }

    public void UpdateTimer(float time)
    {
        int timeInt = (int) Mathf.Floor(time * 100);
        string timerVal = "";

        int temp = timeInt % 100;
        timeInt = timeInt / 100;
        timerVal = temp.ToString() + " '";
        temp = timeInt % 60;
        timeInt = timeInt / 60;
        timerVal = temp.ToString() + " : " + timerVal;
        temp = timeInt % 60;
        timeInt = timeInt / 60;
        timerVal = timeInt.ToString() + " : " + temp.ToString() + " : " + timerVal;

        timerText.text = timerVal;
    }
}
