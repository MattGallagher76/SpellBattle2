using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [Header("References")]
    public GameObject visualObject;
    public Animator bossFireball;
    public Animator hunterFireball;
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

    public PlayerController pc;

    public bool PlayerHasCorrectTools()
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

    public IEnumerator GoodEndingRoutine()
    {
        if (dialogueController != null)
            dialogueController.StartGoodEnding();

        yield return new WaitForSeconds(3f);
        hunterFireball.gameObject.SetActive(true);
        hunterFireball.SetTrigger("hunterFire");

        yield return new WaitForSeconds(1.5f);
        pc.audS.clip = pc.fireball;
        pc.audS.Play();
        yield return new WaitForSeconds(0.5f);
        hunterFireball.gameObject.SetActive(false);

        yield return StartCoroutine(BlinkRoutine());
        visualObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        pc.audS.clip = pc.win;
        pc.audS.Play();
        yield return new WaitForSeconds(5f);

        if (dialogueController != null)
            dialogueController.StartEpilogue();
    }

    public IEnumerator BadEndingRoutine()
    {
        if (dialogueController != null)
            dialogueController.StartBadEnding();

        yield return new WaitForSeconds(7f);
        bossFireball.gameObject.SetActive(true);
        if (bossFireball != null)
            bossFireball.SetTrigger("fire");

        yield return new WaitForSeconds(1.5f);
        pc.audS.clip = pc.fireball;
        pc.audS.Play();
        yield return new WaitForSeconds(0.5f);
        bossFireball.gameObject.SetActive(false);

        if (gameController != null)
        {
            gameController.isDead = true;

            if (gameController.youDiedScreen != null)
                gameController.youDiedScreen.SetActive(true);
        }
        yield return new WaitForSeconds(1f);
        pc.audS.clip = pc.loose;
        pc.audS.Play();
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
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

    public void DEBUG_FillInventoryWithCorrectItems()
    {
        if (inventoryManager == null || inventoryManager.inventory == null)
        {
            Debug.LogError("InventoryManager not set");
            return;
        }

        for (int i = 0; i < requiredItemIDs.Length; i++)
        {
            int targetID = requiredItemIDs[i];
            ItemController foundItem = null;

            // Search ALL items in scene for matching ID
            ItemController[] allItems = FindObjectsOfType<ItemController>();

            for (int j = 0; j < allItems.Length; j++)
            {
                if (allItems[j].itemID == targetID)
                {
                    foundItem = allItems[j];
                    break;
                }
            }

            if (foundItem == null)
            {
                Debug.LogWarning("Could not find item with ID: " + targetID);
                continue;
            }

            // Assign to inventory slot
            inventoryManager.inventory[i] = foundItem;

            // Move it into inventory UI location
            foundItem.transform.parent = inventoryManager.inventoryLocations[i];
            foundItem.transform.localPosition = Vector3.zero;
            foundItem.transform.localScale = Vector3.one * inventoryManager.itemUIScale;

            // Mark as not in world
            if (foundItem.fp != null)
                foundItem.fp.setIsInWorld(false);
        }

        Debug.Log("Filled inventory with correct items");
    }
}