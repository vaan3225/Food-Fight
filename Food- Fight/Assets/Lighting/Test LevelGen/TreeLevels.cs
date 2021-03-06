﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TreeLevels : MonoBehaviour
{
    [Header("Inputs")]
    public GameObject[] rooms;
    public Transform startPosition;
    public int maxHeight;
    public int maxBranchLength;
    public int amountOfUpRooms;


    [Header("InComp")]
    private int roomMovementUp = 6;
    private int roomMovementLeft = 10;
    private int counter = 0;
    private int currentRoom;
    private int nextLeft;
    private int nextRight;
    private int previousUp = 0;
    private List<int> previousUps = new List<int>(1);
    private int pastLeft = 0;

    // Start is called before the first frame update
    void Start()
    {

        // Inital 4x4 Room at given starting location
        // instantiate that
        Instantiate(rooms[14], transform.position, Quaternion.identity);
        nextLeft = UnityEngine.Random.Range(1, maxBranchLength);
        nextRight = UnityEngine.Random.Range(1, maxBranchLength);
    }

    List<int> upRoomSelect(int currentLeft, int currentRight) 
    {
        //THis gives the amount we have to start from the left side to start making up doors
        int maxUpLeft;
        //This gives amount we dont go to the right
        int maxUpRight;
        if (nextLeft >= currentLeft)
        {
            maxUpLeft = 0;
        } 
        else
        {
            maxUpLeft = currentLeft - nextLeft;
        }
        if (nextRight >= currentRight)
        {
            maxUpRight = currentRight;
        }
        else 
        {
            maxUpRight = nextRight;


        }
        List<int> upRooms = new List<int>(amountOfUpRooms + 1);
        for (int i = 0; i < amountOfUpRooms; i++) {

            int roomWithUp = UnityEngine.Random.Range(maxUpLeft, maxUpRight + 1 + maxUpLeft);

            upRooms.Add(roomWithUp);
        }
        upRooms.Sort();
        return upRooms;

    }



    // This will handel all of the room selections depending on a whole bunch of inputs
    private int roomSelect(int currentPos, int size, int upRooms, int downDoor) 
    {
        //left side
        if (currentPos == 0)
        {

            // left side and up
            if (currentPos == upRooms)
            {
                // Left side and up/down
                if (currentPos == downDoor)
                {
                    return 9;// left up down

                }
                return 1;// left side up 

            }
            else if (currentPos == downDoor)
            {
                return 4;// left up down

            }
            else
            {

                return 12;// only left side
            }

        }
        // right side
        else if (currentPos == size)
        {
            // Right side and up
            if (currentPos == upRooms)
            {
                // Right side and up/down
                if (currentPos == downDoor)
                {
                    return 7;// Right up down

                }
                return 2;// Right side up 

            }
            else if (currentPos == downDoor)
            {
                return 3;// left up down

            }
            else
            {

                return 11;// only left side
            }

        }
        // Up rooms This and down use math
        else if (currentPos == upRooms)
        {
            //cant have first or down in this
            // up and down
            if (currentPos == downDoor)
            { //5,6,9
                return 10;

            }
            // only up not down 1,2,8
            return 8;

        }
        //downRooms
        else if (currentPos == downDoor)
        {
            // no up left or right so easy
            //3,4,6,10
            return 6;

        }
        else {

            return 0;
        }

    }


    int downDoorChecker(int currentLeftAmout) 
    {

        return (currentLeftAmout - pastLeft) + previousUp;

    }

    /*int testtest(int currentLeftAmout)
    {
        return (currentLeftAmout - pastLeft);
    }*/

    List<int> newDoorDownChecker(int currentLeftAmout) // gets previous up from the global
    {
        List<int> returnDownDoors = new List<int>(previousUps.Count() + 1);
        if (previousUps.Count() <= 0) {
            previousUps.Add(0);
        }
        
        for (int i = 0; i < previousUps.Count(); i++) {

            returnDownDoors.Add(Mathf.Max((currentLeftAmout - pastLeft) + previousUps[i], 0));
        }
        return returnDownDoors;
    }

    //Should also return room numbers that have a chance at going upwards
    private void treeGeneration() 
    {

        //Move up by 1 Keep Original but could change
        Vector2 changePos = new Vector2(transform.position.x, (transform.position.y + roomMovementUp));
        transform.position = changePos;
        // Move Left By random up to max
        int currentLeftAmout = nextLeft;
        // Right By random by max
        int currentRightAmount = nextRight;
        //Change the next values for next time;
        nextLeft = UnityEngine.Random.Range(1, maxBranchLength);
        nextRight = UnityEngine.Random.Range(1, maxBranchLength);


        //Then change position by left by amount
        Vector2 sideWayMove = new Vector2(transform.position.x - roomMovementLeft * currentLeftAmout, transform.position.y);
        transform.position = sideWayMove;

        if (currentLeftAmout + currentRightAmount <= 2) 
        {
            currentRightAmount += 1;
        }
        List<int> upAmount = upRoomSelect(currentLeftAmout, currentRightAmount);
        int downDoor = downDoorChecker(currentLeftAmout);
        List<int> newDoorDown = newDoorDownChecker(currentLeftAmout);
        
        // This solves the level generation down door problem, figure out how and why??
        newDoorDown.Add(0);

        if (newDoorDown.Count() == 0) {
            newDoorDown.Add(downDoor);

        }
        int counter = 0;
        int sndCounter = 0;
        for (int i = 0; i < currentLeftAmout + currentRightAmount; i++) {

            
            currentRoom = roomSelect(i, currentLeftAmout + currentRightAmount - 1, upAmount[counter], newDoorDown[sndCounter]);
            
            if (upAmount[counter] == i) {

                if (counter < amountOfUpRooms - 1)
                {
                    previousUp = upAmount[counter];
                    counter++;
                }
            }
            
            //testing
            if (newDoorDown[sndCounter] == i)
            {
                if (sndCounter < newDoorDown.Count()-1)
                {
                    sndCounter += 1;
                }
            }

            Instantiate(rooms[currentRoom], transform.position, Quaternion.identity);

            Vector2 slightChange = new Vector2(transform.position.x + roomMovementLeft, transform.position.y);
            transform.position = slightChange;

        }

        //previousUps = upAmount;

        previousUps.Clear();
        for (int i = 0; i<upAmount.Count(); i++)
        {
            previousUps.Add(upAmount[i]);
        }

        transform.position = changePos;
        pastLeft = currentLeftAmout;
    }

    // Update is called once per frame
    void Update()
    {

        if (counter < maxHeight)
        {
            Debug.Log("" + counter);
            treeGeneration();
            counter += 1;

        }
        else if (counter == maxHeight)
        { // this adds extra room to the top of the maze

            int currentLeftAmout = 0;
            int downDoorLoaction = downDoorChecker(currentLeftAmout);

            Vector2 changePos = new Vector2(transform.position.x + (roomMovementLeft * downDoorLoaction), (transform.position.y + roomMovementUp));
            transform.position = changePos;

            Instantiate(rooms[13], transform.position, Quaternion.identity);
            counter += 1;
        }


    }
}
