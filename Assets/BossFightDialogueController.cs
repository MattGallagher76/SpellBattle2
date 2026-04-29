using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    string openingLine = "So you’ve come to make a futile attempt to stop me anyways? It’s not like you actually understand the language of wizards. Flail around my secret lair if you wish, the ritual is almost complete!";

    [TextArea(2, 6)]
    string proceedLine = "Are you sure you wish to proceed?";

    [TextArea(3, 8)]
    string badEndingLine1 = "Those artifacts are useless on me! I was right! I always knew you were faking it! My power grows too strong for your feeble mind to even comprehend!";
    string badEndingLine2= "Soon the world’s knowledge will be mine!";

    [TextArea(3, 8)]
    string goodEndingLine1 = "No…       It’s not possible!          I did everything right!          I took out everyone who could possibly stop me!";
    string goodEndingLine2 = "That wretched headmaster was going to helplessly watch as I became all powerful.                  And you…                YOU!                   You were no one!";
    string goodEndingLine3 = "You don’t even have any magical power!!      YOU CAN’T DO THIS TO ME!!!";

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
        pc.audS.clip = pc.OllieIntro;
        pc.audS.Play();
        yield return StartCoroutine(PlaySingleDialogue(openingLine, lineStayTime, true, false));

        pc.audS.Stop();
        pc.audS.clip = pc.wind;
        pc.audS.Play();
        yield return StartCoroutine(DimLightsRoutine(fadeImage, dimDuration, false));

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
        pc.audS.clip = pc.badEnding;
        pc.audS.Play();
        yield return StartCoroutine(PlaySingleDialogue(badEndingLine1, 0f, false, false));
        yield return StartCoroutine(PlaySingleDialogue(badEndingLine2, 2f, true, false));
    }

    private IEnumerator GoodEndingRoutine()
    {
        pc.audS.clip = pc.goodEnding;
        pc.audS.Play();
        yield return StartCoroutine(PlaySingleDialogue(goodEndingLine1, 1f, false, true));
        yield return StartCoroutine(PlaySingleDialogue(goodEndingLine2, 2f, false, true));
        yield return StartCoroutine(PlaySingleDialogue(goodEndingLine3, 2f, true, true));
        //yield return new WaitForSeconds(2f);
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

        yield return StartCoroutine(DimLightsRoutine(brighteningImage, dimDuration * 2, true));

        yield return null;

        RectTransform textRect = epilogueText.GetComponent<RectTransform>();

        while (textRect.anchoredPosition.y < 500f)
        {
            textRect.anchoredPosition += Vector2.up * epilogueScrollSpeed * 1.2f * Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(0);
    }

    private IEnumerator PlaySingleDialogue(string line, float stayTime, bool HideBoxAfter, bool isGood)
    {
        if (isGood)
            dialogueBox.characterDelay /= 1.1f;
        Debug.Log("Show Dialouge");
        dialogueBox.ShowDialogue(line);

        yield return new WaitUntil(() => !dialogueBox.IsTyping());
        yield return new WaitForSeconds(stayTime);

        if(HideBoxAfter)
            dialogueBox.HideDialogue();
    }

    private IEnumerator DimLightsRoutine(RawImage img, float dur, bool startEpiFlag)
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

        if(startEpiFlag)
        {
            epilogueRoot.SetActive(true);
            epilogueText.text = epilogueBody;

            RectTransform textRect = epilogueText.GetComponent<RectTransform>();
            Vector2 startPos = textRect.anchoredPosition;
            textRect.anchoredPosition = new Vector2(startPos.x, -600f);
        }

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