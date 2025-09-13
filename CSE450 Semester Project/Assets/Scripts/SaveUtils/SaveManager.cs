using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class SaveManager : MonoBehaviour
{
    private static string _saveFileName = "gamesave.json";
    private static string _saveGamePath;

    private void Awake()
    {
        _saveGamePath = Path.Combine(Application.persistentDataPath, _saveFileName);
    }

    public void SaveGameStateButton()
    {
        SaveData save = new SaveData();
        SaveGameState(save);
        Debug.Log("[+] Game state saved.");
        return;
    }

    public void LoadGameStateButton()
    {
        SaveData loadedData = LoadGameState();
        Debug.Log("Loaded game state");
        return;
    }

    public void SaveGameState(SaveData save)
    {
        if (String.IsNullOrEmpty(_saveGamePath))
        {
            Debug.Log("[-] Save path not yet initialized");
            return;
        }

        try
        {
            using (FileStream fs = new FileStream(_saveGamePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string jsonData = JsonUtility.ToJson(save, true);
                    sw.Write(jsonData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[!] Failed to save game state:\n{e.Message}");
        }

        return;
    }

    public SaveData LoadGameState()
    {
        if (!File.Exists(_saveGamePath))
        {
            Debug.LogError("[!] Game save does not exist!");
            return null;
        }

        try
        {
            string saveData = "";
            using (FileStream fs = new FileStream(_saveGamePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    saveData = sr.ReadToEnd();
                }
            }

            if (saveData == "")
            {
                Debug.LogError("[!] Loaded empty save file!");
                return null;
            }

            SaveData objSaveData = JsonUtility.FromJson<SaveData>(saveData);
            return objSaveData;
        }
        catch(Exception e)
        {
            Debug.LogError($"[!] Error while loading saved data:\n{e.Message}");
            return null;
        }
    }
}
