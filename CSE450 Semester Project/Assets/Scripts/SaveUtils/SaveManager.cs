using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Unity.VisualScripting;

public class SaveManager : MonoBehaviour
{
    private static string _saveFileName = "/gamesave.bin";
    private static string _saveGamePath = Path.Combine(Application.persistentDataPath, _saveFileName);

    public static void SaveGameState(object save)
    {
        BinaryFormatter binFormatter = new BinaryFormatter();
        FileStream fs = new FileStream(_saveGamePath, FileMode.Create, FileAccess.Write);

        if (fs == null)
        {
            Debug.LogError("[!] Failed to open savefile!");
            goto _CloseFs;
        }

        binFormatter.Serialize(fs, save);

    _CloseFs:
        fs.Close();

        return;
    }

    public static object LoadGameState()
    {
        if (!File.Exists(_saveGamePath))
        {
            Debug.LogError("[!] Game save does not exist!");
            return null;
        }

        try
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            FileStream fs = new FileStream(_saveGamePath, FileMode.Open, FileAccess.Read);
            object saveData = binFormatter.Deserialize(fs);
            fs.Close();
            return saveData;
        }
        catch(Exception e)
        {
            Debug.LogError($"[!] Error while loading saved data:\n{e.Message}");
            return null;
        }
    }
}
