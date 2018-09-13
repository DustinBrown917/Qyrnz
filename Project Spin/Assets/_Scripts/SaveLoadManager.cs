using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager {

    private static string savePath = Application.persistentDataPath + "/save.json";

    /// <summary>
    /// Writes the player data to a .json file.
    /// </summary>
    /// <param name="playerData"></param>
    public static void SavePlayerData(PlayerData playerData)
    {
        PlayerDataContainer container = new PlayerDataContainer(playerData);

        string json = JsonUtility.ToJson(container);

        StreamWriter writer = new StreamWriter(savePath, false);
        writer.WriteLine(json);
        writer.Close();
    }

    /// <summary>
    /// Loads player data from .json file.
    /// </summary>
    /// <returns>PlayerDataContainer containing the loaded player data.</returns>
    public static PlayerDataContainer LoadPlayerData()
    {
        if (File.Exists(savePath))
        {
            StreamReader reader = new StreamReader(savePath);
            string json = reader.ReadToEnd();

            PlayerDataContainer container = JsonUtility.FromJson<PlayerDataContainer>(json);

            reader.Close();
            return container;
        }
        else
        {
            return null;
        }
    }

    public static void ClearPlayerData()
    {
        File.Delete(savePath);
    }
}

[Serializable]
public class PlayerDataContainer
{
    public int[] HighScores;
    public bool TutorialEnabled;

    public PlayerDataContainer(PlayerData playerData)
    {
        HighScores = playerData.HighScores;
        TutorialEnabled = playerData.TutorialEnabled;
    }
}
