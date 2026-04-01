using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public AnimationCurve moveCurve;
    public float moveDurationMultiplier = 1f;
    public float turnDurationMultiplier = 1f;

    public float doorWaitDuration;

    public float turnDistFirst;
    public float turnDistAfter;
    public float forwardDistFirst;
    public float primaryDist;


    private bool isMoving = false;

    GameController gameController;

    private void Start()
    {
        Screen.SetResolution(3840, 1080, false);
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        if (isMoving || gameController.isDead)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(StartMoveDirection(2));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(StartMoveDirection(1));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(StartMoveDirection(3));
        }
    }

    public void directionalButtonPressed(int i)
    {
        StartCoroutine(StartMoveDirection(i));
    }

    IEnumerator StartMoveDirection(int dirIndex)
    {
        isMoving = true;

        if (dirIndex == 1)
        {
            yield return StartCoroutine(MoveTo(transform.position + transform.forward * turnDistFirst, moveDurationMultiplier * turnDistFirst));
            yield return StartCoroutine(RotateTo(transform.rotation * Quaternion.Euler(0f, -90f, 0f), turnDurationMultiplier));
            yield return StartCoroutine(MoveTo(transform.position + transform.forward * turnDistAfter, moveDurationMultiplier * turnDistAfter));
        }
        else if (dirIndex == 2)
        {
            yield return StartCoroutine(MoveTo(transform.position + transform.forward * forwardDistFirst, moveDurationMultiplier * forwardDistFirst));
        }
        else if (dirIndex == 3)
        {
            yield return StartCoroutine(MoveTo(transform.position + transform.forward * turnDistFirst, moveDurationMultiplier * turnDistFirst));
            yield return StartCoroutine(RotateTo(transform.rotation * Quaternion.Euler(0f, 90f, 0f), turnDurationMultiplier));
            yield return StartCoroutine(MoveTo(transform.position + transform.forward * turnDistAfter, moveDurationMultiplier * turnDistAfter));;
        }
        else
        {
            Debug.LogError("Bad move direction call");

        }
        gameController.roomSequence[gameController.currentLevel].OpenDoor(dirIndex - 1);

        yield return new WaitForSeconds(doorWaitDuration);

        yield return StartCoroutine(MoveTo(transform.position + transform.forward * primaryDist, moveDurationMultiplier * primaryDist));
        gameController.checkChoice(dirIndex - 1);
        isMoving = false;
    }

    IEnumerator MoveTo(Vector3 pos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curvedT = moveCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPos, pos, curvedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = pos;
    }

    IEnumerator RotateTo(Quaternion targetRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

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