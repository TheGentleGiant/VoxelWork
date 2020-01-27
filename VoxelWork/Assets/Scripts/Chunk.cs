using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
[Serializable]
class BlockData
{
    public Block.BlockType[,,] blockMatrix;
    public BlockData(){}

    public BlockData(Block[,,] block)
    {
        blockMatrix = new Block.BlockType[World.chunkSize, World.chunkSize,World.chunkSize];
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {

                    blockMatrix[x, y, z] = block[x, y, z].bType;
                }
            }
        }
    }
}
public class Chunk
{
    public Material _material;
    public Block[,,] _chunkData;
    public GameObject _chunk;
    public enum chunckStatus { DRAW, DONE, KEEP };
    public chunckStatus status;
    private BlockData _blockData;

    string CreateChunkFileName(Vector3 chunckPosition)
    {
        return Application.persistentDataPath + "/savedata/Chunck_" + (int) chunckPosition.x + "_" +
               (int) chunckPosition.y + "_" + (int) chunckPosition.z + "_" + World.chunkSize + "_" + World.radius +
               "_" + ".dat";
    }

    bool Load()
    {
        string chunkFile = CreateChunkFileName(_chunk.transform.position);
        if (File.Exists(chunkFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.Open);
            _blockData = new BlockData();
            _blockData = (BlockData) bf.Deserialize(file);
            Debug.Log("Loading chunk file at: " + chunkFile);
            file.Close();
            return true;
        }
        return false;
    }

    public void Save()
    {
        string chunkFile = CreateChunkFileName(_chunk.transform.position);
        if (!File.Exists(chunkFile))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
        _blockData = new BlockData();
        bf.Serialize(file, _blockData);
        file.Close();
    }

    void BuildChunk()
    {
        bool dataFromFile = false;
        dataFromFile = Load();
        
        _chunkData = new Block[World.chunkSize,World.chunkSize,World.chunkSize];
        /*Create Chunk Data*/
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x,y,z);
                    Vector3 chunkPosition = _chunk.transform.position;
                    int worldX = (int) (x + chunkPosition.x);
                    int worldY = (int) (y + chunkPosition.y);
                    int worldZ = (int) (z + chunkPosition.z);

                    if (dataFromFile)
                    {
                        _chunkData[x,y,z] = new Block(_blockData.blockMatrix[x,y,z], pos, _chunk.gameObject, this);
                        continue;
                    }
                    
                    //Debug.Log(GenerationUtils.GenerateHeight(worldX, worldZ));
                    //if (GenerationUtils._BrownianMotion3D(worldX, worldY, worldZ, 3, 0.5f) < 0.40f)
                    if (GenerationUtils.BrownianMotion3D(worldX, worldY, worldZ, 0.1f, 3) < 0.42f)
                    {
                        _chunkData[x,y,z] = new Block(Block.BlockType.AIR, pos, _chunk.gameObject, this);
                    }
                    else if (worldY <= GenerationUtils.GenerateStoneHeight(worldX, worldZ))
                    {
                        if (GenerationUtils.BrownianMotion3D(worldX,worldY,worldZ, 0.1f, 2)< 0.4f && worldY <= 40)
                        {
                            _chunkData[x,y,z] = new Block(Block.BlockType.DIAMOND, pos, _chunk.gameObject, this);
                            Debug.Log("Placing Diamonds");
                        }
                        else if (GenerationUtils.BrownianMotion3D(worldX, worldY, worldZ, 0.3f, 3) < 0.41f &&
                                 worldY <= 20)
                        {
                            _chunkData[x,y,z] = new Block(Block.BlockType.REDSTONE, pos, _chunk.gameObject, this);
                            Debug.Log("Placing Red stone");
                        }
                        else
                        {
                            _chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, _chunk.gameObject, this);
                        }
                    }
                    else if (worldY == GenerationUtils.GenerateHeight(worldX, worldZ))
                    {
                        _chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, _chunk.gameObject, this);
                    }
                    else if (worldY <= GenerationUtils.GenerateHeight(worldX, worldZ))
                    {
                        _chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, _chunk.gameObject, this);
                    }
                    else
                    {
                        _chunkData[x,y,z] = new Block(Block.BlockType.AIR, pos, _chunk.gameObject, this);
                    }

                    status = chunckStatus.DRAW;
                }
            }
        }
       
       
      
    }

    public void DrawChunk()
    {
        /*Draw Chunk*/
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    _chunkData[x,y,z].Draw();
                }
            }
        }
        //Old drawing Logic
        /*for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector3 pos = new Vector3(x,y,z);
                    Block _block = new Block(Block.BlockType.DIRT, pos, this.gameObject, _material);
                    _block.Draw();
                    yield return null;
                }
            }
        }*/
        CombineQuads(); 
        MeshCollider _blockCollider = _chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        _blockCollider.sharedMesh = _chunk.transform.GetComponent<MeshFilter>().mesh;
        status = chunckStatus.DONE;

    }

    public Chunk(Vector3 position, Material material)
    {
        _chunk = new GameObject(World.BuildChunkName(position));
        _chunk.transform.position = position;
        _material = material;
        BuildChunk();
    }
    

    //batching all the triangles together makes it easier for unity to handle the drawing, also have less draw calls
    //doing this by combining all the quads together
    void CombineQuads()
    {
        MeshFilter[] _meshFilters = _chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] _combine = new CombineInstance[_meshFilters.Length];
        int i = 0;
        while (i < _meshFilters.Length)
        {
            _combine[i].mesh = _meshFilters[i].sharedMesh;
            _combine[i].transform = _meshFilters[i].transform.localToWorldMatrix;
            i++;
        }
        //after creating the combine array with all the previously created quads we give it a meshFilter, a mesh renderer
        //and being the case that we don't need the quads to be rendered any more we can destroy them;
        MeshFilter _mf = (MeshFilter) _chunk.gameObject.AddComponent(typeof(MeshFilter));
        _mf.mesh = new Mesh();
        _mf.mesh.CombineMeshes(_combine);

        MeshRenderer _meshRenderer = _chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        _meshRenderer.material = _material;

        foreach (Transform quad in _chunk.transform)
            GameObject.Destroy(quad.gameObject);
    }

}
