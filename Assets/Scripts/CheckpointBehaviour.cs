using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    private int index;

    public void SetIndex(int _index)
    {
        index = _index;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            MapManager.Instance.UpdateCheckpoint(index);
        }
    }
}
