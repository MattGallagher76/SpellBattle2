using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacePlayer : MonoBehaviour
{
    public bool isInWorld;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject.transform;
        //GetComponent<Image>().color = new Color32(
        //    (byte)Random.Range(128, 256),
        //    (byte)Random.Range(128, 256),
        //    (byte)Random.Range(128, 256),
        //    255
        //);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInWorld)
        {
            transform.LookAt(player);
        }
    }

    public void setIsInWorld(bool t)
    {
        isInWorld = t;
        if (!t)
            transform.localEulerAngles = Vector3.zero;
    }
}
