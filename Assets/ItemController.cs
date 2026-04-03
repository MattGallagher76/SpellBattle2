using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public int itemID;
    public FacePlayer fp;

    [Header("Scales")]
    public Vector3 worldScale = Vector3.one;
    public Vector3 inventoryScale = Vector3.one;

}