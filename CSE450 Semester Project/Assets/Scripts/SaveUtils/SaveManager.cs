using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

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
        GameObject spawner = GameObject.Find("Spawner");
        
        for (int i = 0; i < SaveData.Instance.creatures.Count; ++i)
        {
            Vector2 v = spawner.transform.GetChild(i).position;
            SaveData.Instance.creatures[i].position = new Vector2Serial(v);
        }

        SaveGameState(SaveData.Instance);
        Debug.Log($"[+] Game state saved to {_saveGamePath}.");
        return;
    }

    public void LoadGameStateButton()
    {
        // delete spawned objects before loading in saved objects
        GameObject devilSpawn = GameObject.Find("Spawner");
        foreach(Transform s in devilSpawn.transform)
        {
            Destroy(s.gameObject);
        }

        SaveData loadedData = LoadGameState();
        FileUploader fileUpload = FindObjectOfType<FileUploader>();

        foreach(var c in loadedData.creatures)
        {
            GameObject prefab = fileUpload.Ext2Prefab(c.extension);
            GameObject creature = Instantiate(prefab, c.position.SerialToVector2(), 
                prefab.transform.rotation, GameObject.Find("Spawner").transform);
            var creatureStats = creature.GetComponent<CreatureSelect>();
            creatureStats.InitCreature(c.name, c.size, c.stats);
        }
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
