using UnityEngine;

public class CreatureStats : MonoBehaviour
{
    public float speed = 3f; // default speed
    public float stamina = 100f;
    public float maxStamina = 100f;

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