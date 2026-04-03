using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaInputManager : MonoBehaviour
{
    public Sigil[] sigils;
    public string correctID;
    public AudioClip bad;
    public AudioClip good;
    public AudioSource audS;

    public void eval()
    {
        string str = "";
        foreach(Sigil s in sigils)
        {
            str += s.getID() + ", ";
        }
        Debug.Log(str);
        if (str.Replace(" ", "").Equals(correctID.Replace(" ", "")))
        {
            audS.clip = good;
        }
        else
            audS.clip = bad;
        audS.Play();
    }

}
