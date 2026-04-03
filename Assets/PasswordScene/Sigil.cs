using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sigil : MonoBehaviour
{
    public bool[] sigil = new bool[6];

    public SigilSection[] sigilSections;
    //public SigilMenuOption smo;

    public SigilSection midLine;

    private void Start()
    {
        //sigil[0] = Random.Range(0, 2) == 0;
        //sigil[1] = Random.Range(0, 2) == 0;
        //sigil[2] = Random.Range(0, 2) == 0;
        //sigil[3] = Random.Range(0, 2) == 0;
        //sigil[4] = Random.Range(0, 2) == 0;
        //sigil[5] = Random.Range(0, 2) == 0;

        updateSigilVisual();
    }

    public void changeMode(int cell)
    {
        sigil[cell] = !sigil[cell];
        updateSigilVisual();
    }

    public void mirrorMode(Sigil s)
    {
        if (s == null || s.sigil == null || s.sigil.Length != sigil.Length) return;

        // Copy values, don't share the same array reference
        System.Array.Copy(s.sigil, sigil, sigil.Length);
        updateSigilVisual();
    }

    public void SetState(bool[] newState)
    {
        if (newState == null || newState.Length != sigil.Length) return;

        // Copy values, don't share reference
        System.Array.Copy(newState, sigil, sigil.Length);
        updateSigilVisual();
    }

    public void updateSigilVisual()
    {
        for (int i = 0; i < 6; i ++)
        {
            sigilSections[i].setState(sigil[i]);
        }
        midLine.setState(sigilSections[4].state || sigilSections[5].state);
    }

    public string getID()
    {
        string str = "";
        for(int i = 0; i < sigil.Length; i ++)
        {
            if (sigil[i])
                str += (i + 1);
        }
        return str;
    }
}
