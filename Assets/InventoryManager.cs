using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public ItemController[] inventory;
    public Transform[] inventoryLocations;

    public GameObject offerMenu;
    public GameObject offerMenuSecondScreen;
    public GameObject moveMenu;
    public Transform offerItemLocation;

    PlayerController pc;

    ItemController offeredItem;

    public Vector3 itemPosOffset;
    public float itemUIScale;

    // Start is called before the first frame update
    void Start()
    {
        pc = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void offerItemChoice()
    {
        Debug.Log("Offer Trade");
        if (pc.currentRoom == null || pc.currentRoom.storedItem == null)
            return;
        offeredItem = pc.currentRoom.storedItem;

        offeredItem.gameObject.transform.position = offerItemLocation.position;
        moveMenu.SetActive(false);
        offerMenu.SetActive(true);
        offerMenuSecondScreen.SetActive(true);
    }

    public void disregardOffer()
    {
        Debug.Log("Disregard Offer");
        offerMenu.SetActive(false);
        offerMenuSecondScreen.SetActive(false);
        moveMenu.SetActive(true);
        offeredItem.transform.position = pc.currentRoom.transform.position + itemPosOffset;
    }

    public void addItemToInventory(int indexItemToRemove)
    {
        Debug.Log("Accept Offer - " + indexItemToRemove);
        offeredItem.fp.setIsInWorld(false);
        pc.currentRoom.storedItem = inventory[indexItemToRemove-1];

        if(pc.currentRoom.storedItem != null)
        {
            inventory[indexItemToRemove - 1].transform.parent = pc.currentRoom.transform;
            inventory[indexItemToRemove - 1].transform.localScale = Vector3.one / itemUIScale;
            inventory[indexItemToRemove - 1].transform.position = pc.currentRoom.transform.position + itemPosOffset;
            inventory[indexItemToRemove - 1].fp.setIsInWorld(true);
        }
        inventory[indexItemToRemove - 1] = offeredItem;

        inventory[indexItemToRemove - 1].transform.parent = inventoryLocations[indexItemToRemove - 1].transform;
        inventory[indexItemToRemove - 1].transform.localScale = Vector3.one * itemUIScale;
        offeredItem.transform.position = inventoryLocations[indexItemToRemove-1].transform.position;
        offerMenu.SetActive(false);
        offerMenuSecondScreen.SetActive(false);
        moveMenu.SetActive(true);
        offeredItem = null;
    }
}
