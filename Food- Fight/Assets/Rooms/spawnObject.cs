﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnObject : MonoBehaviour
{

    public GameObject[] objects;
    // Start is called before the first frame update

    void Start()
    {
        if(objects.Length <= 0)
        {
            return;
        }
        int rand = Random.Range(0, objects.Length);
        Instantiate(objects[rand], transform.position, Quaternion.identity);

    }

}
