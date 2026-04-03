using UnityEngine;

public class ProceedPromptButtons : MonoBehaviour
{
    private PlayerController pc;

    private void Start()
    {
        pc = FindObjectOfType<PlayerController>();
    }

    public void PressYes()
    {
        if (pc != null && pc.currentRoom != null)
            pc.currentRoom.confirmProceed();
    }

    public void PressNo()
    {
        if (pc != null && pc.currentRoom != null)
            pc.currentRoom.cancelProceed();
    }
}