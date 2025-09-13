using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public sealed class SaveData
{
    // Reference: https://csharpindepth.com/articles/singleton

    private static SaveData _instance       = null;
    private static readonly object _lock    = new object();

    // lock only necessary if our game ends up being multithreaded.
    public static SaveData Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new SaveData();
                }
                return _instance;
            }
        }
    }

    // TODO: implement save and necessary serializatio surrogates after creature and kitchen/station classes are complete.
    /*
        e.g. 
        public List<CreatureStats> creatures; 
    */
}
