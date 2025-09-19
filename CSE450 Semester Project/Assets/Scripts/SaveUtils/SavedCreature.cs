using FileAnalysis;
using Unity.VisualScripting;

[System.Serializable]
public class SavedCreature
{
    public string name;
    public long size;
    public Stats stats;
    public string extension;

    public SavedCreature(string name, long size, Stats stats, string extension)
    {
        this.name = name;
        this.size = size;
        this.stats = stats;
        this.extension = extension;
    }
}
