﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject block;
    public int worldSize = 2;

    public IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldSize; z++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                for (int x = 0; x < worldSize; x++)
                {
                    Vector3 pos = new Vector3(x,y,z);
                    GameObject cube = GameObject.Instantiate(block, pos, Quaternion.identity);
                    cube.name = x + "_" + y + "_" + z;
                    cube.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                }
                
                yield return null;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BuildWorld());
    }
}
