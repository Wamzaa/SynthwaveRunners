using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed
{
    public float duration;
    public float minSpeed;
    public float maxSpeed;

    public Speed(float _duration, float _minSpeed, float _maxSpeed)
    {
        duration = _duration;
        minSpeed = _minSpeed;
        maxSpeed = _maxSpeed;
    }
}
