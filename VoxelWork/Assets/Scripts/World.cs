using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public GameObject player;
    public Material textureAtlas;
    
    public static int chunkSize = 16;
    public static int columnHeight = 16;
    public static int worldSize = 4;
    public static int radius = 4;
    //public Slider slider;
    //public Camera cam;
    //public Button playButton;
    private bool isFirstInstanceBuild = true;
    private bool isBuilding = false;

    public Vector3 lastRecordedBuildPosition;
    private CoroutineQueue _queue;
    public static uint maxCoroutinesactive = 1000;

    //public static Dictionary<string, Chunk> chunks;
    public static ConcurrentDictionary<string, Chunk> chunks;
    public static List<string> chuncksToRemove = new List<string>();
    
    public static string BuildChunkName(Vector3 position)
    {
        return (int) position.x + "_" + (int) position.y + "_" + (int) position.z;
    }
    
#region OldCode
    /*Old Not called upon anymore*/
    /*IEnumerator BuildChunkColumn()
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
    }*/
#endregion

void BuildChunkAt(int x, int y, int z)
    {
        Vector3 _chunkPosition = new Vector3(x * chunkSize, y* chunkSize, z * chunkSize);
        string _chunkName = BuildChunkName(_chunkPosition);
        Chunk _chunk;
        if (!chunks.TryGetValue(_chunkName, out _chunk))
        {
            _chunk = new Chunk(_chunkPosition, textureAtlas);
            _chunk._chunk.transform.parent = transform;
            chunks.TryAdd(_chunk._chunk.name, _chunk);
        }
    }

    IEnumerator BuildRecursiveWorld(int x, int y, int z, int rad)
    {
        rad--;
        if (rad <= 0)
        {
            Debug.Log("Radius = " + rad);
            yield break;
        }
        BuildChunkAt(x, y, z +1);
        _queue.Run(BuildRecursiveWorld(x, y, z + 1, rad));
        yield return null;
        
        BuildChunkAt(x, y, z -1);
        _queue.Run(BuildRecursiveWorld(x, y, z - 1, rad));
        yield return null;
        
        BuildChunkAt(x+1, y, z);
        _queue.Run(BuildRecursiveWorld(x-1, y, z , rad));
        yield return null;
        
        BuildChunkAt(x- 1, y, z);
        _queue.Run(BuildRecursiveWorld(x+ 1, y, z, rad));
        yield return null;
        
        BuildChunkAt(x, y -1, z );
        _queue.Run(BuildRecursiveWorld(x, y - 1, z, rad));
        yield return null;
        
        BuildChunkAt(x, y + 1, z);
        _queue.Run(BuildRecursiveWorld(x, y + 1 , z, rad));
        yield return null;
    }

    IEnumerator DrawChunks()
    {
        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            if (chunk.Value.status == Chunk.chunckStatus.DRAW)
            {
                chunk.Value.DrawChunk();
            }

            if (chunk.Value._chunk && Vector3.Distance(player.transform.position, chunk.Value._chunk.transform.position)> radius*chunkSize)
            {
                chuncksToRemove.Add(chunk.Key);
            }
            yield return null;
        }
    }
    private void BuildNearPlayer()
    {
        StopCoroutine("BuildRecursiveWorld");
        _queue.Run(BuildRecursiveWorld((int) (player.transform.position.x / chunkSize),
            (int) (player.transform.position.y / chunkSize),
            (int) (player.transform.position.z / chunkSize), radius));
    }

    IEnumerator RemoveOldChunks()
    {
        for (int i = 0; i < chuncksToRemove.Count; i++)
        {
            string chunckName = chuncksToRemove[i];
            Chunk chunck;
            if (chunks.TryGetValue(chunckName, out chunck))
            {
                Destroy(chunck._chunk);
                chunck.Save();
                chunks.TryRemove(chunckName, out chunck);
                yield return null;
            }
        }
    }
    
    void Start()
    {
        _queue = new CoroutineQueue(maxCoroutinesactive, StartCoroutine);
        GenerationUtils.perlin();
        Vector3 _playerPosition = player.transform.position;
        player.transform.position = new Vector3(_playerPosition.x,
            GenerationUtils.GenerateHeight(_playerPosition.x, _playerPosition.z) + 1, _playerPosition.z);
        lastRecordedBuildPosition = player.transform.position;
        player.SetActive(false);
        isFirstInstanceBuild = true;
        chunks = new ConcurrentDictionary<string, Chunk>();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        

        BuildChunkAt((int) (player.transform.position.x / chunkSize), (int) (player.transform.position.y/ chunkSize),
            (int) (player.transform.position.z / chunkSize));

        _queue.Run(DrawChunks());

        _queue.Run(BuildRecursiveWorld((int) (player.transform.position.x / chunkSize),
            (int) (player.transform.position.y / chunkSize),
            (int) (player.transform.position.z / chunkSize), radius));
    }
    private void Update()
    {
        Vector3 _movement = lastRecordedBuildPosition - player.transform.position;
        if (_movement.magnitude > chunkSize)
        {
            lastRecordedBuildPosition = player.transform.position;
            BuildNearPlayer();
        }
        
        if (!player.activeSelf)
        {
            player.SetActive(true);
            isFirstInstanceBuild = false;
        }
        _queue.Run(DrawChunks());
        _queue.Run(RemoveOldChunks());
    }



//    public void StartBuild()
  //  {
        //StartCoroutine(BuildWorld());
    //}
}
