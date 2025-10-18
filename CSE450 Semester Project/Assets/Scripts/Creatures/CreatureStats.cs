using FileAnalysis;
using UnityEngine;

public class CreatureStats : MonoBehaviour {

    // save reference to base stats generated
    private Stats baseStats;
    private long fileSize;

    public float speed = 3f; // default speed
    public float stamina = 100f;
    public float maxStamina = 100f;

    public Stats GetStats() { return baseStats; }
    public int GetTossStat() { return baseStats.DoughHandling; }
    public int GetTopStat() { return baseStats.Toppings; }
    public int GetOvensStat() { return baseStats.Cooking; }
    public int GetCutStat() { return baseStats.Cutting; }

    public void SetStats(Stats s) {
        baseStats = s;
        fileSize = 0; // TODO: this is just so everything doesn't break
        
        speed = Mathf.Lerp(2f, 10f, s.Speed / 100f); // maps 0-100 speed stat to 2-10 speed
        maxStamina = Mathf.Lerp(50f, 100f, s.Stamina / 100f); // maps 0-100 stat to 50-100 stamina
        stamina = maxStamina; // start at full stamina
        
        UpdateVisuals();
    }

    public void SetStats(Stats s, long size)
    {
        baseStats = s;
        fileSize = size;

        speed = Mathf.Lerp(2f, 10f, s.Speed / 100f); // maps 0-100 speed stat to 2-10 speed
        maxStamina = Mathf.Lerp(50f, 100f, s.Stamina / 100f); // maps 0-100 stat to 50-100 stamina
        stamina = maxStamina; // start at full stamina

        UpdateVisuals();
    }

    // call after setting stats
    public void UpdateVisuals()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // map:
        // Cooking -> red
        // Cutting -> green
        // DoughHandling -> blue

        float r = Mathf.Clamp01(baseStats.Cooking / 100f);
        float g = Mathf.Clamp01(baseStats.Cutting / 100f);
        float b = Mathf.Clamp01(baseStats.DoughHandling / 100f);
        sr.color = new Color(r, g, b);

        // map:
        // small files ~0.5x size, big files ~2x size
        float fileScale = Mathf.Clamp01(Mathf.Log10(fileSize) / 6f);
        float scaleFactor = Mathf.Lerp(0.5f, 2f, fileScale);

        // TODO: reimplement this without affecting scale of game object handling physics
        // transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    // call this when character performs a task
    public bool TryPerformTask(float staminaCost)
    {
        if (stamina >= staminaCost)
        {
            stamina -= staminaCost;
            return true;
        }
        else
        {
            return false;
        }
    }

    // call this to recover stamina when not completing tasks (idle)
    public void RecoverStamina(float amount)
    {
        stamina = Mathf.Min(stamina + amount, maxStamina);
    }

}