using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    public GameObject visualObject; // The object that will blink (mesh, model, etc.)

    [Header("Blink Settings")]
    public float blinkDuration = 0.1f; // Time between on/off
    public int blinkCount = 6;         // Number of blinks

    private bool isBlinking = false;

    public bool DEBUG_TakeDamage;

    public int health;

    private void Update()
    {
        if (DEBUG_TakeDamage)
        {
            DEBUG_TakeDamage = false;
            TakeDamage(1);
        }
    }

    // Call this when the boss takes damage
    public void TakeDamage(int damage)
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkRoutine());
        }
        health--;

        // You can expand this with health logic later
        Debug.Log("Boss took damage: " + damage);
    }

    private IEnumerator BlinkRoutine()
    {
        isBlinking = true;

        Renderer[] renderers = visualObject.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < blinkCount; i++)
        {
            SetRenderersEnabled(renderers, false);
            yield return new WaitForSeconds(blinkDuration);

            SetRenderersEnabled(renderers, true);
            yield return new WaitForSeconds(blinkDuration);
        }

        isBlinking = false;
    }

    private void SetRenderersEnabled(Renderer[] rends, bool state)
    {
        foreach (Renderer r in rends)
        {
            r.enabled = state;
        }
    }
}