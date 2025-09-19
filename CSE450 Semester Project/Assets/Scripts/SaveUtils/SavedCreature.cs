using FileAnalysis;
using UnityEngine;

[System.Serializable]
public class SavedCreature
{
    public string name;
    public long size;
    public Stats stats;
    public string extension;
    public Vector2Serial position;

    public SavedCreature(string name, long size, Stats stats, string extension, Vector2 position)
    {
        this.name = name;
        this.size = size;
        this.stats = stats;
        this.extension = extension;
        this.position = new Vector2Serial(position);
    }
}
