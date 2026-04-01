using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int currentLevel;
    public int[] correctSequence;
    public RoomController[] roomSequence;

    public bool isDead = false;

    public GameObject youDiedScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkChoice(int choice)
    {
        if (correctSequence[currentLevel] == choice)
        {
            roomSequence[currentLevel].setAdjacentRoomVisibility(false);
            Debug.Log("Correct");
            currentLevel++;
            roomSequence[currentLevel].setAdjacentRoomVisibility(true);
        }
        else
        {
            isDead = true;
            Debug.Log("You have died");
            youDiedScreen.SetActive(true);
        }
    }
}
