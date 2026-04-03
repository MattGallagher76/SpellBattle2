using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IdleResetToScene : MonoBehaviour
{
    public float idleTimeLimit = 300f; // 5 minutes
    
    private float idleTimer = 0f;

    void Update()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTimeLimit)
        {
            Debug.Log("Idle timeout reached - resetting scene");
            SceneManager.LoadScene(0);
        }
    }

    public void ResetIdleTimer()
    {
        idleTimer = 0f;
    }
}