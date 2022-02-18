using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public List<MapSelectionButton> mapButtonList;

    public void LaunchMap(string mapName)
    {
        SceneManager.LoadScene(mapName);
    }

    public void UpdateMapSelectionButtons()
    {
        string file = File.ReadAllText("D:/Projets/DEV/FichiersTest/mapScores.json");
        MapScores scores = JsonUtility.FromJson<MapScores>(file);
        foreach(MapSelectionButton mapButton in mapButtonList)
        {
            for(int i = 0; i<scores.mapScores.Count; i++)
            {
                if(mapButton.id == scores.mapScores[i].id)
                {
                    mapButton.UpdateScore(scores.mapScores[i].bestTime, scores.mapScores[i].lastTime);
                    scores.mapScores.Remove(scores.mapScores[i]);
                    break;
                }
            }
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
