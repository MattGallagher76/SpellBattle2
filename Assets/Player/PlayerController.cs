using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public AnimationCurve moveCurve;
    public float moveDurationMultiplier = 1f;
    public float turnDurationMultiplier = 1f;

    public float doorWaitDuration;

    public float forwardDistFirst;
    public float tunnelDist;
    public float finalDist;

    public RoomController currentRoom;
    public int currentDirection;

    private bool isMoving = false;

    GameController gameController;
    BossController bossController;
    InventoryManager invManager;

    public AudioClip door; //
    public AudioClip wind; //
    public AudioClip walk; //
    public AudioClip item; //
    public AudioClip fireball; //
    public AudioClip win; //
    public AudioClip loose; //

    public AudioClip OllieIntro;
    public AudioClip goodEnding;
    public AudioClip badEnding;

    public AudioSource audS;
    IdleResetToScene idleReset;

    private void Start()
    {
        idleReset = FindObjectOfType<IdleResetToScene>();
        Screen.SetResolution(3840, 1080, false);
        gameController = FindObjectOfType<GameController>();
        bossController = FindObjectOfType<BossController>();
        invManager = FindObjectOfType<InventoryManager>();
    }

    //void Update()
    //{
    //    if (isMoving || gameController.isDead)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        StartCoroutine(StartMoveDirection(0));
    //    }
    //    else if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        StartCoroutine(StartMoveDirection(3));
    //    }
    //    else if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        StartCoroutine(StartMoveDirection(1));
    //    }
    //}

    public void directionalButtonPressed(int i)
    {
        if (!isMoving)
        {
            if (idleReset != null)
                idleReset.ResetIdleTimer();

            StartCoroutine(StartMoveDirection(i));
        }
    }

    public void spinInPlace(int i)
    {
        if (!isMoving)
            StartCoroutine(StartSpinInPlace(i));
    }

    IEnumerator StartSpinInPlace(int dirIndex)
    {
        isMoving = true;

        if (dirIndex == 0)
        {
            yield return StartCoroutine(RotateTo(transform.rotation * Quaternion.Euler(0f, -90f, 0f), turnDurationMultiplier));
            currentDirection -= 1;
        }
        else if (dirIndex == 1)
        {
            yield return StartCoroutine(RotateTo(transform.rotation * Quaternion.Euler(0f, 90f, 0f), turnDurationMultiplier));
            currentDirection += 1;
        }

        isMoving = false;
    }

    IEnumerator StartMoveDirection(int dirIndex)
    {
        invManager.disregardOffer();
        int doorDirRef = ((dirIndex + currentDirection) % 4 + 4) % 4;

        GameObject nextRoomObj = currentRoom.GetNextRoomObject(doorDirRef);

        if (nextRoomObj == null)
            yield break;

        RoomController nextRoom = nextRoomObj.GetComponent<RoomController>();

        isMoving = true;

        Vector3 roomCenterPosition = transform.position;

        if (dirIndex == 3)
        {
            yield return StartCoroutine(RotateTo(transform.rotation * Quaternion.Euler(0f, -90f, 0f), turnDurationMultiplier));
            currentDirection -= 1;
        }
        else if (dirIndex == 0)
        {
        }
        else if (dirIndex == 1)
        {
            yield return StartCoroutine(RotateTo(transform.rotation * Quaternion.Euler(0f, 90f, 0f), turnDurationMultiplier));
            currentDirection += 1;
        }
        else
        {
            Debug.LogError("Bad move direction call");
            isMoving = false;
            yield break;
        }

        yield return StartCoroutine(MoveTo(transform.position + transform.forward * forwardDistFirst, moveDurationMultiplier * forwardDistFirst));

        if (currentRoom.DoorRequiresProceedPrompt(doorDirRef))
        {
            currentRoom.ShowProceedPrompt(doorDirRef);

            yield return new WaitUntil(() => currentRoom.HasProceedChoiceBeenMade());

            bool proceed = currentRoom.WasProceedApproved();
            currentRoom.ResetProceedChoiceState();

            if (!proceed)
            {
                float returnDistance = Vector3.Distance(transform.position, roomCenterPosition);
                yield return StartCoroutine(MoveTo(roomCenterPosition, moveDurationMultiplier * returnDistance));

                isMoving = false;
                yield break;
            }

            if (gameController != null && gameController.moveMenu != null)
                gameController.moveMenu.SetActive(false);

            audS.clip = door;
            audS.Play();
            currentRoom.OpenDoor(doorDirRef == 0 ? 'n' : doorDirRef == 1 ? 'e' : doorDirRef == 2 ? 's' : doorDirRef == 3 ? 'w' : 'X');
            yield return new WaitForSeconds(doorWaitDuration);

            yield return StartCoroutine(MoveTo(transform.position + transform.forward * tunnelDist * 2.5f, moveDurationMultiplier * tunnelDist * 2.5f));
            yield return StartCoroutine(RotateTo(transform.rotation * Quaternion.Euler(-5f, 0f, 0f), turnDurationMultiplier*3));

            isMoving = false;


            if (bossController.PlayerHasCorrectTools())
                StartCoroutine(bossController.GoodEndingRoutine());
            else
                StartCoroutine(bossController.BadEndingRoutine());

            yield break;
        }

        audS.clip = door;
        audS.Play();
        currentRoom.OpenDoor(doorDirRef == 0 ? 'n' : doorDirRef == 1 ? 'e' : doorDirRef == 2 ? 's' : doorDirRef == 3 ? 'w' : 'X');
        yield return new WaitForSeconds(doorWaitDuration);

        yield return StartCoroutine(MoveTo(transform.position + transform.forward * tunnelDist, moveDurationMultiplier * tunnelDist));

        audS.clip = door;
        audS.Play();
        nextRoom.OpenDoor(doorDirRef == 0 ? 's' : doorDirRef == 1 ? 'w' : doorDirRef == 2 ? 'n' : doorDirRef == 3 ? 'e' : 'X');
        yield return new WaitForSeconds(doorWaitDuration);

        yield return StartCoroutine(MoveTo(transform.position + transform.forward * finalDist, moveDurationMultiplier * finalDist));

        gameController.checkChoice(dirIndex - 1);
        currentRoom = nextRoom;
        isMoving = false;
        if (currentRoom.storedItem != null)
            invManager.offerItemChoice();
    }

    public IEnumerator IntroMoveToDoor()
    {
        isMoving = true;
        yield return StartCoroutine(MoveTo(new Vector3(0, 3.35f, -10f), moveDurationMultiplier * 6.25f));
    }

    public IEnumerator IntroMoveToCenter()
    {
        yield return StartCoroutine(MoveTo(new Vector3(0, 3.35f, 0), moveDurationMultiplier * 10f));
        isMoving = false;
    }

    IEnumerator MoveTo(Vector3 pos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        if (duration <= 0f)
        {
            transform.position = pos;
            yield break;
        }

        // START WALK SOUND
        if (audS != null && walk != null)
        {
            audS.clip = walk;
            audS.loop = true;
            audS.Play();
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curvedT = moveCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPos, pos, curvedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = pos;

        // STOP WALK SOUND
        if (audS != null && audS.isPlaying)
        {
            audS.Stop();
            audS.loop = false;
        }
    }

    IEnumerator RotateTo(Quaternion targetRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        if (duration <= 0f)
        {
            transform.rotation = targetRot;
            yield break;
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curvedT = moveCurve.Evaluate(t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, curvedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;
    }

    public void resetScene()
    {
        SceneManager.LoadScene(0);
    }
}