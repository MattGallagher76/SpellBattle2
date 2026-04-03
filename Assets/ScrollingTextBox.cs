using System.Collections;
using TMPro;
using UnityEngine;

public class BossDialogueBox : MonoBehaviour
{
    [Header("References")]
    public GameObject dialogueRoot;
    public TextMeshProUGUI dialogueText;

    [Header("Typing")]
    public float characterDelay = 0.03f;

    private Coroutine typingCoroutine;
    private string currentMessage = "";
    private bool isTyping = false;

    private void Awake()
    {
        //if (dialogueRoot != null)
        //    dialogueRoot.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";
    }

    public void ShowDialogue(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentMessage = message;
        typingCoroutine = StartCoroutine(TypeRoutine(message));
    }

    public void HideDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        if (dialogueText != null)
            dialogueText.text = "";

        if (dialogueRoot != null)
            dialogueRoot.SetActive(false);
    }

    public void SkipTyping()
    {
        if (!isTyping)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = currentMessage;
        isTyping = false;
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    private IEnumerator TypeRoutine(string message)
    {
        isTyping = true;

        Debug.Log("Set Active");

        if (dialogueRoot != null)
            dialogueRoot.SetActive(true);

        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(characterDelay);
        }

        isTyping = false;
        typingCoroutine = null;
    }
}