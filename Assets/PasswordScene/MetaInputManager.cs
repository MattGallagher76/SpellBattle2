using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaInputManager : MonoBehaviour
{
    public Sigil[] sigils;
    public string correctID;

    public AudioClip bad;
    public AudioClip good;
    public AudioSource audS;

    public string nextSceneName;

    public void eval()
    {
        string str = "";
        foreach (Sigil s in sigils)
        {
            str += s.getID() + ", ";
        }

        Debug.Log(str);

        bool isCorrect = str.Replace(" ", "").Equals(correctID.Replace(" ", ""));

        StartCoroutine(HandleResult(isCorrect));
    }

    IEnumerator HandleResult(bool isCorrect)
    {
        if (isCorrect)
        {
            audS.clip = good;
            audS.Play();

            // wait for sound to finish
            yield return new WaitForSeconds(good.length);

            // load next scene
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            audS.clip = bad;
            audS.Play();

            // wait for sound (optional, but feels better)
            yield return new WaitForSeconds(bad.length);
        }
    }
}