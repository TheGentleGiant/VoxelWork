using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public GameObject player;
    public static int chunkSize = 16;
    public Material textureAtlas;
    public static int columnHeight = 16;
    public static int worldSize = 4;
    public static int radius = 1;
    public Slider slider;
    public Camera cam;
    public Button playButton;
    private bool isFirstInstanceBuild = true;
    private bool isBuilding = false; 

    public static Dictionary<string, Chunk> chunks;
    void Start()
    {
        player.SetActive(false);
        GenerationUtils.perlin();
        chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        //Instantiate(PlayerPrefab, new Vector3(5, 160, 5), Quaternion.identity);
        //BuildWorld();
    }

    private void Update()
    {
        if (!isBuilding && !isFirstInstanceBuild)
        {
            StartCoroutine(BuildWorld());
        }
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
        isBuilding = true;
        int posX = (int)Mathf.Floor(player.transform.position.x / chunkSize);
        int posZ = (int) Mathf.Floor(player.transform.position.z / chunkSize);

        float totalChuncks = (Mathf.Pow(radius * 2 + 1, 2) * columnHeight) * 2;
        int processCount = 0;
        
        for (int z = -radius; z <= radius; z++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 _chunkPosition = new Vector3((x + posX)*chunkSize, y*chunkSize, (z + posZ)*chunkSize);
                    Chunk _chunk;
                    string chunckName = BuildChunkName(_chunkPosition);
                    if (chunks.TryGetValue(chunckName, out _chunk))
                    {
                        _chunk.status = Chunk.chunckStatus.KEEP;
                        break;
                    }
                    else
                    {
                        _chunk = new Chunk(_chunkPosition, textureAtlas);
                        _chunk._chunk.transform.parent = this.transform;
                        chunks.Add(_chunk._chunk.name, _chunk);
                    }

                    if (isFirstInstanceBuild)
                    {
                        processCount++;
                        slider.value = processCount / totalChuncks * 100;   
                    }
                    
                    yield return null;
                }
            }
        }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.status == Chunk.chunckStatus.DRAW)
            {
                c.Value.DrawChunk();
                c.Value.status = Chunk.chunckStatus.KEEP;
            }

            c.Value.status = Chunk.chunckStatus.DONE;
            if (isFirstInstanceBuild)
            {
                processCount++;
                slider.value = processCount / totalChuncks * 100;

            }
            yield return null;
        }

        if (isFirstInstanceBuild)
        {
            player.SetActive(true);
            slider.gameObject.SetActive(false);
            cam.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            isFirstInstanceBuild = false;
        }

        isBuilding = false;
    }

    public void StartBuild()
    {
        StartCoroutine(BuildWorld());
    }
}
