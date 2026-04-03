using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator animator;

    public void openDoor()
    {
        animator.SetTrigger("openDoor");        
    }
}