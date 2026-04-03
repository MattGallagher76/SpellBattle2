using System.Collections;
using UnityEngine;
using TMPro;

public class RoomController : MonoBehaviour
{
    public int roomIndex;

    public DoorController WestDoor;
    public DoorController NorthDoor;
    public DoorController EastDoor;
    public DoorController SouthDoor;

    public GameObject WestRoom;
    public GameObject NorthRoom;
    public GameObject EastRoom;
    public GameObject SouthRoom;

    public ItemController storedItem;

    [Header("Proceed Door Prompt")]
    public string proceedDoorTag = "ProceedPromptDoor";

    public GameObject proceedMenu;
    public GameObject proceedMenuSecondScreen;
    public GameObject moveMenu;
    public BossDialogueBox proceedPromptText;

    [TextArea(2, 6)]
    public string proceedPromptMessage = "Are you sure you wish to proceed?";

    private bool waitingForProceedChoice = false;
    private bool proceedChoiceMade = false;
    private bool proceedApproved = false;

    public void OpenDoor(char c)
    {
        Debug.Log("Room " + this.name + " Opening: " + c);
        if (c == 'n')
            NorthDoor.openDoor();
        else if (c == 's')
            SouthDoor.openDoor();
        else if (c == 'e')
            EastDoor.openDoor();
        else if (c == 'w')
            WestDoor.openDoor();
        else
            Debug.LogError("Opening door: " + c);
    }

    public DoorController GetDoorController(int dir)
    {
        if (dir == 0)
            return NorthDoor;
        if (dir == 1)
            return EastDoor;
        if (dir == 2)
            return SouthDoor;
        if (dir == 3)
            return WestDoor;

        return null;
    }

    public GameObject GetNextRoomObject(int dir)
    {
        if (dir == 0)
            return NorthRoom;
        if (dir == 1)
            return EastRoom;
        if (dir == 2)
            return SouthRoom;
        if (dir == 3)
            return WestRoom;

        return null;
    }

    public bool DoorRequiresProceedPrompt(int dir)
    {
        DoorController dc = GetDoorController(dir);

        if (dc == null)
            return false;

        return dc.gameObject.CompareTag(proceedDoorTag);
    }

    public void ShowProceedPrompt(int dir)
    {
        DoorController dc = GetDoorController(dir);

        if (dc == null)
            return;

        waitingForProceedChoice = true;
        proceedChoiceMade = false;
        proceedApproved = false;

        proceedPromptText.ShowDialogue("Are you sure you wish to proceed?");

        if (moveMenu != null)
            moveMenu.SetActive(false);

        if (proceedMenu != null)
            proceedMenu.SetActive(true);

        if (proceedMenuSecondScreen != null)
            proceedMenuSecondScreen.SetActive(true);
    }

    public void confirmProceed()
    {
        if (!waitingForProceedChoice)
            return;

        proceedApproved = true;
        proceedChoiceMade = true;
        waitingForProceedChoice = false;

        HideProceedPromptUI();
    }

    public void cancelProceed()
    {
        if (!waitingForProceedChoice)
            return;

        proceedApproved = false;
        proceedChoiceMade = true;
        waitingForProceedChoice = false;

        HideProceedPromptUI();
    }

    public bool IsWaitingForProceedChoice()
    {
        return waitingForProceedChoice;
    }

    public bool HasProceedChoiceBeenMade()
    {
        return proceedChoiceMade;
    }

    public bool WasProceedApproved()
    {
        return proceedApproved;
    }

    public void ResetProceedChoiceState()
    {
        waitingForProceedChoice = false;
        proceedChoiceMade = false;
        proceedApproved = false;
    }

    private void HideProceedPromptUI()
    {
        if (proceedMenu != null)
            proceedMenu.SetActive(false);

        if (proceedMenuSecondScreen != null)
            proceedMenuSecondScreen.SetActive(false);

        if (moveMenu != null)
            moveMenu.SetActive(true);
    }
}