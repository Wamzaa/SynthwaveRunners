using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public int id;

    public List<GameObject> checkpointList;

    [SerializeField]
    private int currentCheckpoint;
    [SerializeField]
    private int lap;

    private float startTime;

    private void Awake()
    {
        // MapManager est un singleton
        if (Instance == null) { 
            Instance = this; 
        }else { 
            Debug.LogError("Erreur : MapManager déjà instancié !"); 
        }
    }

    private void Start()
    {
        currentCheckpoint = 0;
        lap = 0;
        SetupCheckpoints();
        startTime = Time.time;
    }

    public void SetupCheckpoints()
    {
        for(int i=0; i<checkpointList.Count; i++)
        {
            CheckpointBehaviour checkpoint = checkpointList[i].GetComponent<CheckpointBehaviour>();
            checkpoint.SetIndex(i);
        }
    }

    private void Update()
    {
        float timer = Time.time - startTime;
        UIManager.Instance.UpdateTimer(timer);
    }

    // Met à jour le checkpoint actuel
    public void UpdateCheckpoint(int index)
    {
        if(index == currentCheckpoint+1)
        {
            currentCheckpoint = index;
        }else if(index == 0 && currentCheckpoint == checkpointList.Count-1)
        {
            currentCheckpoint = 0;
            lap += 1;
            UIManager.Instance.UpdateLapText(lap);
            if(lap == 3)
            {
                FillMapScoreFile();
            }
        }
    }

    public void FillMapScoreFile()
    {
        string bestScore = "";
        string file = File.ReadAllText("D:/Projets/DEV/FichiersTest/mapScores.json");
        MapScores scores = JsonUtility.FromJson<MapScores>(file);
        for(int i=0; i<scores.mapScores.Count; i++)
        {
            MapScore mapScore = scores.mapScores[i]; 
            if(mapScore.id == this.id)
            {
                bestScore = mapScore.bestTime;
                scores.mapScores.Remove(mapScore);
            }
        }
        MapScore newMapScore = new MapScore();
        newMapScore.id = this.id;
        newMapScore.lastTime = (Time.time - startTime).ToString();
        newMapScore.bestTime = bestScore;

        scores.mapScores.Add(newMapScore);
        string json = JsonUtility.ToJson(scores, true);
        File.WriteAllText("D:/Projets/DEV/FichiersTest/mapScores.json", json);
    }
}
