using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public ItemController[] inventory;
    public Transform[] inventoryLocations;

    //public GameObject offerMenu;
    //public GameObject offerMenuSecondScreen;
    public GameObject moveMenu;
    public Transform offerItemLocation;

    PlayerController pc;

    ItemController offeredItem;

    public Vector3 itemPosOffset;
    public float itemUIScale;

    public bool DEBUG_Inventory;

    public Color original;
    public Color highlight;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        pc = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (DEBUG_Inventory)
    //    {
    //        DEBUG_Inventory = false;
    //        FindObjectOfType<BossController>().DEBUG_FillInventoryWithCorrectItems();
    //    }
    //}

    public void offerItemChoice()
    {
        Debug.Log("Offer Trade");
        if (pc.currentRoom == null || pc.currentRoom.storedItem == null)
            return;
        offeredItem = pc.currentRoom.storedItem;

        foreach(Transform t in inventoryLocations)
        {
            t.gameObject.GetComponent<RawImage>().color = highlight;
        }

        offeredItem.transform.parent = this.transform;
        offeredItem.gameObject.transform.position = offerItemLocation.position;
        //moveMenu.SetActive(false);
        //offerMenu.SetActive(true);
        //offerMenuSecondScreen.SetActive(true);
    }

    public void disregardOffer()
    {
        Debug.Log("Disregard Offer");
        foreach (Transform t in inventoryLocations)
        {
            t.gameObject.GetComponent<RawImage>().color = original;
        }
        //offerMenu.SetActive(false);
        //offerMenuSecondScreen.SetActive(false);
        moveMenu.SetActive(true);
        if (offeredItem != null)
        {
            offeredItem.transform.position = pc.currentRoom.transform.position + itemPosOffset;
            offeredItem.transform.parent = GetComponent<PlayerController>().currentRoom.transform;
        }
    }

    public void addItemToInventory(int indexItemToRemove)
    {
        if (offeredItem == null)
            return;

        pc.audS.clip = pc.item;
        pc.audS.Play();

        Debug.Log("Accept Offer - " + indexItemToRemove);

        int idx = indexItemToRemove - 1;

        offeredItem.fp.setIsInWorld(false);

        pc.currentRoom.storedItem = inventory[idx];

        if (pc.currentRoom.storedItem != null)
        {
            ItemController removedItem = inventory[idx];

            removedItem.transform.SetParent(pc.currentRoom.transform, false);
            removedItem.transform.localPosition = itemPosOffset;
            removedItem.transform.localRotation = Quaternion.identity;
            removedItem.transform.localScale = removedItem.worldScale;

            removedItem.fp.setIsInWorld(true);
        }

        inventory[idx] = offeredItem;

        inventory[idx].transform.SetParent(inventoryLocations[idx], false);
        inventory[idx].transform.localPosition = Vector3.zero;
        inventory[idx].transform.localRotation = Quaternion.identity;
        inventory[idx].transform.localScale = inventory[idx].inventoryScale;

        moveMenu.SetActive(true);
        offeredItem = null;

        foreach (Transform t in inventoryLocations)
        {
            t.gameObject.GetComponent<RawImage>().color = original;
        }
    }
}
