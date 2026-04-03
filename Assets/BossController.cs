using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    public GameObject visualObject;
    public Animator bossFireball;
    public BossFightDialogueController dialogueController;
    public InventoryManager inventoryManager;
    public GameController gameController;

    [Header("Blink Settings")]
    public float blinkDuration = 0.1f;
    public int blinkCount = 6;

    [Header("Boss Stats")]
    public int health = 3;
    public bool DEBUG_TakeDamage;

    [Header("Correct Item IDs")]
    public int[] requiredItemIDs = new int[4];

    private bool isBlinking = false;
    private bool endingStarted = false;

    private void Update()
    {
        if (DEBUG_TakeDamage)
        {
            DEBUG_TakeDamage = false;
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (endingStarted)
            return;

        if (!isBlinking)
            StartCoroutine(BlinkRoutine());

        health -= damage;
        Debug.Log("Boss took damage: " + damage);

        if (health <= 0)
        {
            endingStarted = true;

            if (PlayerHasCorrectTools())
                StartCoroutine(GoodEndingRoutine());
            else
                StartCoroutine(BadEndingRoutine());
        }
    }

    private bool PlayerHasCorrectTools()
    {
        if (inventoryManager == null || inventoryManager.inventory == null)
            return false;

        for (int i = 0; i < requiredItemIDs.Length; i++)
        {
            bool found = false;

            for (int j = 0; j < inventoryManager.inventory.Length; j++)
            {
                if (inventoryManager.inventory[j] != null &&
                    inventoryManager.inventory[j].itemID == requiredItemIDs[i])
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;
        }

        return true;
    }

    private IEnumerator GoodEndingRoutine()
    {
        if (dialogueController != null)
            dialogueController.StartGoodEnding();

        yield return new WaitForSeconds(3f);

        Debug.Log("Launch magical fireball at Ollie here");

        yield return new WaitForSeconds(2f);

        if (dialogueController != null)
            dialogueController.StartEpilogue();
    }

    private IEnumerator BadEndingRoutine()
    {
        if (dialogueController != null)
            dialogueController.StartBadEnding();

        yield return new WaitForSeconds(3f);

        if (bossFireball != null)
            bossFireball.SetTrigger("fire");

        yield return new WaitForSeconds(1.5f);

        if (gameController != null)
        {
            gameController.isDead = true;

            if (gameController.youDiedScreen != null)
                gameController.youDiedScreen.SetActive(true);
        }
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