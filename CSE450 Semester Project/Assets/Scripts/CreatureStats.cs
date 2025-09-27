using FileAnalysis;
using UnityEngine;

public class CreatureStats : MonoBehaviour
{

    // save reference to base stats generated
    private Stats baseStats;

    public float speed = 3f; // default speed
    public float stamina = 100f;
    public float maxStamina = 100f;

    public Stats GetStats() { return baseStats; }
    public void SetStats(Stats s)
    {
        baseStats = s;
        speed = Mathf.Lerp(2f, 10f, s.Speed / 100f); // maps 0-100 speed stat to 2-10 speed
        maxStamina = Mathf.Lerp(50f, 100f, s.Stamina / 100f); // maps 0-100 stat to 50-100 stamina
        stamina = maxStamina; // start at full stamina
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