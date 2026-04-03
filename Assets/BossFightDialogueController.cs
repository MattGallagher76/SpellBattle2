using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossFightDialogueController : MonoBehaviour
{
    [Header("Dialogue Box")]
    public BossDialogueBox dialogueBox;

    [Header("Optional UI")]
    public GameObject proceedPromptRoot;
    public TextMeshProUGUI epilogueText;
    public GameObject epilogueRoot;

    [Header("Opening Sequence Objects")]
    public DoorController introDoor;
    public Light[] lightsToDim;
    public DoorController lockedDoor;

    [Header("Timing")]
    public float lineStayTime = 2f;
    public float openingDelay1 = 1f;
    public float openingDelay2 = 1f;
    public float dimDuration = 1f;
    public float epilogueScrollSpeed = 20f;

    [Header("Story Text")]
    [TextArea(3, 8)]
    public string openingLine = "You’ve discovered my secret lair! No matter, you’ll never stop me before the ritual is complete!";

    [TextArea(2, 6)]
    public string proceedLine = "Are you sure you wish to proceed?";

    [TextArea(3, 8)]
    public string badEndingLine = "Those artifacts have no effect on me! My power grows too strong for your feeble mind to even comprehend! Soon the world’s knowledge will be mine!";

    [TextArea(3, 8)]
    public string goodEndingLine = "TODO";

    [TextArea(6, 20)]
    public string epilogueBody = "TODO";

    private Coroutine epilogueCoroutine;

    public GameObject introBossSprite;

    public PlayerController pc;
    public RawImage fadeImage;

    private void Start()
    {
        StartOpeningSequence();

        //if (proceedPromptRoot != null)
        //    proceedPromptRoot.SetActive(false);

        //if (epilogueRoot != null)
        //    epilogueRoot.SetActive(false);
    }

    public void StartOpeningSequence()
    {
        StartCoroutine(OpeningSequence());
    }

    private IEnumerator OpeningSequence()
    {
        if (introDoor != null)
            introDoor.openDoor();

        yield return new WaitForSeconds(openingDelay1);

        yield return StartCoroutine(pc.IntroMoveToDoor());

        yield return new WaitForSeconds(1f);

        Debug.Log("Play Single Dialouge");
        yield return StartCoroutine(PlaySingleDialogue(openingLine, lineStayTime));

        yield return StartCoroutine(DimLightsRoutine());

        yield return new WaitForSeconds(openingDelay2);

        yield return StartCoroutine(pc.IntroMoveToCenter());
    }

    public void ShowProceedPrompt()
    {
        StartCoroutine(PlayProceedPromptRoutine());
    }

    private IEnumerator PlayProceedPromptRoutine()
    {
        if (proceedPromptRoot != null)
            proceedPromptRoot.SetActive(true);

        dialogueBox.ShowDialogue(proceedLine);

        yield return new WaitUntil(() => !dialogueBox.IsTyping());
    }

    public void HideProceedPrompt()
    {
        Debug.Log("HideProceedPrompt");
        dialogueBox.HideDialogue();

        if (proceedPromptRoot != null)
            proceedPromptRoot.SetActive(false);
    }

    public void StartBadEnding()
    {
        StartCoroutine(BadEndingRoutine());
    }

    public void StartGoodEnding()
    {
        StartCoroutine(GoodEndingRoutine());
    }

    private IEnumerator BadEndingRoutine()
    {
        yield return StartCoroutine(PlaySingleDialogue(badEndingLine, 2f));
    }

    private IEnumerator GoodEndingRoutine()
    {
        yield return StartCoroutine(PlaySingleDialogue(goodEndingLine, 2f));
    }

    public void StartEpilogue()
    {
        if (epilogueCoroutine != null)
            StopCoroutine(epilogueCoroutine);

        epilogueCoroutine = StartCoroutine(EpilogueRoutine());
    }

    private IEnumerator EpilogueRoutine()
    {
        if (epilogueRoot == null || epilogueText == null)
            yield break;

        epilogueRoot.SetActive(true);
        epilogueText.text = epilogueBody;

        RectTransform textRect = epilogueText.GetComponent<RectTransform>();
        Vector2 startPos = textRect.anchoredPosition;
        textRect.anchoredPosition = new Vector2(startPos.x, -600f);

        yield return null;

        while (textRect.anchoredPosition.y < 1200f)
        {
            textRect.anchoredPosition += Vector2.up * epilogueScrollSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator PlaySingleDialogue(string line, float stayTime)
    {
        Debug.Log("Show Dialouge");
        dialogueBox.ShowDialogue(line);

        yield return new WaitUntil(() => !dialogueBox.IsTyping());
        yield return new WaitForSeconds(stayTime);

        dialogueBox.HideDialogue();
    }

    private IEnumerator DimLightsRoutine()
    {
        if (fadeImage == null)
            yield break;

        Color c = fadeImage.color;

        float elapsed = 0f;

        // FADE TO BLACK
        while (elapsed < dimDuration)
        {
            float t = elapsed / dimDuration;

            c.a = Mathf.Lerp(0f, 1f, t);
            fadeImage.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;

        yield return new WaitForSeconds(0.5f);

        // Hide intro boss while screen is black
        introBossSprite.SetActive(false);

        // FADE BACK IN
        elapsed = 0f;

        while (elapsed < dimDuration)
        {
            float t = elapsed / dimDuration;

            c.a = Mathf.Lerp(1f, 0f, t);
            fadeImage.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
    }
}