using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using Random = System.Random;

public class Chunk
{
    public Material _material;
    public Block[,,] _chunkData;
    public GameObject _chunk;
    void BuildChunk()
    {
        _chunkData = new Block[World.chunkSize,World.chunkSize,World.chunkSize];
        /*Create Chunk Data*/
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x,y,z);
                    Vector3 position = _chunk.transform.position;
                    int worldX = (int) (x + position.x);
                    int worldY = (int) (y + position.y);
                    int worldZ = (int) (z + position.z);
                    
                    //Debug.Log(GenerationUtils.GenerateHeight(worldX, worldZ));
                    if (worldY <= GenerationUtils.GenerateHeight(worldX, worldZ))
                    {
                        _chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, _chunk.gameObject, this);
                    }
                    else
                    {
                        _chunkData[x,y,z] = new Block(Block.BlockType.AIR, pos, _chunk.gameObject, this);
                    }
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
