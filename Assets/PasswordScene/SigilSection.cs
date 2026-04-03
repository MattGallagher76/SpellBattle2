using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SigilSection : MonoBehaviour
{
    public bool state;
    public GameObject onStateObject;
    public GameObject offStateObject;

    private void Start()
    {
        setState(state);
    }

    public void setState(bool target)
    {
        state = target;
        onStateObject.SetActive(state);
        offStateObject.SetActive(!state);
    }
}
