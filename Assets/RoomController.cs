using System.Collections;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public int roomIndex;
    public DoorController LeftDoor;
    public DoorController CenterDoor;
    public DoorController RightDoor;

    public GameObject LeftRoom;
    public GameObject CenterRoom;
    public GameObject RightRoom;

    RoomController LeftController;
    RoomController CenterController;
    RoomController RightController;

    void Start()
    {
        //if (roomIndex != 0)
        //{
        //    LeftController = LeftRoom.GetComponent<RoomController>();
        //    CenterController = CenterRoom.GetComponent<RoomController>();
        //    RightController = RightRoom.GetComponent<RoomController>();
        //}
    }

    public void OpenDoor(int index)
    {
        if (index == 0)
            LeftDoor.openDoor();
        else if (index == 1)
            CenterDoor.openDoor();
        else if (index == 2)
            RightDoor.openDoor();
    }

    public void setAdjacentRoomVisibility(bool vis)
    {
        LeftRoom.SetActive(vis || LeftController.roomIndex != 0);
        CenterRoom.SetActive(vis || CenterController.roomIndex != 0);
        RightRoom.SetActive(vis || RightController.roomIndex != 0);
    }
}