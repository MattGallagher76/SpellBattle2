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
    public string badEndingLine1 = "Those artifacts have no effect on me! My power grows too strong for your feeble mind to even comprehend!";
    public string badEndingLine2= "Soon the world’s knowledge will be mine!";

    [TextArea(3, 8)]
    public string goodEndingLine = "TODO";

    [TextArea(6, 20)]
    public string epilogueBody = "TODO";

    private Coroutine epilogueCoroutine;

    public GameObject introBossSprite;

    public PlayerController pc;
    public RawImage fadeImage;
    public RawImage brighteningImage;

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
        pc.audS.clip = pc.door;
        pc.audS.Play();
        if (introDoor != null)
            introDoor.openDoor();

        yield return new WaitForSeconds(openingDelay1);

        yield return StartCoroutine(pc.IntroMoveToDoor());

        yield return new WaitForSeconds(1f);

        Debug.Log("Play Single Dialouge");
        yield return StartCoroutine(PlaySingleDialogue(openingLine, lineStayTime, true));

        pc.audS.clip = pc.wind;
        pc.audS.Play();
        yield return StartCoroutine(DimLightsRoutine(fadeImage, dimDuration));

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
        yield return StartCoroutine(PlaySingleDialogue(badEndingLine1, 2f, false));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(PlaySingleDialogue(badEndingLine2, 2f, true));
    }

    private IEnumerator GoodEndingRoutine()
    {
        yield return StartCoroutine(PlaySingleDialogue(goodEndingLine, 2f, true));
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

        yield return StartCoroutine(DimLightsRoutine(brighteningImage, dimDuration * 2));

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

    private IEnumerator PlaySingleDialogue(string line, float stayTime, bool HideBoxAfter)
    {
        Debug.Log("Show Dialouge");
        dialogueBox.ShowDialogue(line);

        yield return new WaitUntil(() => !dialogueBox.IsTyping());
        yield return new WaitForSeconds(stayTime);

        if(HideBoxAfter)
            dialogueBox.HideDialogue();
    }

    private IEnumerator DimLightsRoutine(RawImage img, float dur)
    {
        img.gameObject.SetActive(true);
        if (img == null)
            yield break;

        Color c = img.color;

        float elapsed = 0f;

        // FADE TO BLACK
        while (elapsed < dur)
        {
            float t = elapsed / dur;

            c.a = Mathf.Lerp(0f, 1f, t);
            img.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        c.a = 1f;
        img.color = c;

        yield return new WaitForSeconds(0.5f);

        // Hide intro boss while screen is black
        introBossSprite.SetActive(false);

        // FADE BACK IN
        elapsed = 0f;

        while (elapsed < dur)
        {
            float t = elapsed / dur;

            c.a = Mathf.Lerp(1f, 0f, t);
            img.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        c.a = 0f;
        img.color = c;
        img.gameObject.SetActive(false);
    }
}