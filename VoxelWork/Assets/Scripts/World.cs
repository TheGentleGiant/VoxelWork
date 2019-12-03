﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static int chunkSize = 16;
    public Material textureAtlas;
    public static int columnHeight = 16;
    public static int worldSize = 4;

    public static Dictionary<string, Chunk> chunks;
    void Start()
    {
        GenerationUtils.perlin();
        chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(BuildWorld());
    }


    public static string BuildChunkName(Vector3 position)
    {
        return (int) position.x + "_" + (int) position.y + "_" + (int) position.z;
    }
    
#pragma region Old
    /*Old Not called upon anymore*/
    IEnumerator BuildChunkColumn()
    {
        for (int i = 0; i < columnHeight; i++)
        {
            Vector3 _chunkPosition = new Vector3(this.transform.position.x, i*chunkSize, this.transform.position.z);
            Chunk _chunk = new Chunk(_chunkPosition, textureAtlas);
            _chunk._chunk.transform.parent = this.transform;
            chunks.Add(_chunk._chunk.name, _chunk);
        }

        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            chunk.Value.DrawChunk();
            yield return null;
        }
    }
#pragma endregion
    
    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldSize; z++)
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 _chunkPosition = new Vector3(x*chunkSize, y*chunkSize, z*chunkSize);
                    Chunk _chunk = new Chunk(_chunkPosition, textureAtlas);
                    _chunk._chunk.transform.parent = this.transform;
                    chunks.Add(_chunk._chunk.name, _chunk);
                }
            }
        }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }
    }
}
