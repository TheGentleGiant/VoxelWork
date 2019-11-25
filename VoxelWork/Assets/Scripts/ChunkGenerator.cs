using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public Material _material;
    public Block[,,] _chunkData;
    IEnumerator BuildChunk(int sizeX, int sizeY, int sizeZ)
    {
        _chunkData = new Block[sizeX,sizeY,sizeZ];
        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector3 pos = new Vector3(x,y,z);
                    _chunkData[x,y,z] = new Block(Block.BlockType.DIRT, pos, this.gameObject, _material);
                }
            }
        }
        
        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    _chunkData[x,y,z].Draw();
                    yield return null;
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

    private void Start()
    {
        StartCoroutine(BuildChunk(10, 10, 10));
    }

    //batching all the triangles together makes it easier for unity to handle the drawing, also have less draw calls
    //doing this by combining all the quads together
    void CombineQuads()
    {
        MeshFilter[] _meshFilters = GetComponentsInChildren<MeshFilter>();
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
        MeshFilter _mf = (MeshFilter) this.gameObject.AddComponent(typeof(MeshFilter));
        _mf.mesh = new Mesh();
        _mf.mesh.CombineMeshes(_combine);

        MeshRenderer _meshRenderer = this.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        _meshRenderer.material = _material;

        foreach (Transform quad in this.transform)
            Destroy(quad.gameObject);
    }

}
