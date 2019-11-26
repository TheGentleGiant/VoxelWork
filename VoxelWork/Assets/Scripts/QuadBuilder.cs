using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadBuilder : MonoBehaviour
{
    [SerializeField] private Material _mat;
    enum SideOfCube
    {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    };

    public enum BlockType
    {
        GRASS,
        DIRT,
        STONE
    };

    public BlockType bType;

    private Vector2[,] blocksUvs =
    {
        /*grass top*/
        {
            new Vector2(0.125f, 0.375f), new Vector2(0.1875f, 0.375f), new Vector2(0.125f, 0.4375f),
            new Vector2(0.1875f, 0.4375f)
        },
        /*grass side*/
        {
            new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f), new Vector2(0.1875f, 1.0f),
            new Vector2(0.25f, 1.0f)
        },
        /*dirt*/
        {
            new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.125f, 1.0f),
            new Vector2(0.1875f, 1.0f)
        },
        /*stone*/
        {new Vector2(0, 0.875f), new Vector2(0.0625f, 0.875f), new Vector2(0f, 0.9375f), new Vector2(0.0625f, 0.9375f)}

    };
    
    void CreateQuad(SideOfCube side)
    {
        Mesh _mesh = new Mesh();
        _mesh.name = "CustomMesh";

        Vector3[] verts = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] tris = new int[6];

        //UVs
        Vector2 uv00 = new Vector2(0f, 0f);
        Vector2 uv01 = new Vector2(1f, 0f);
        Vector2 uv02 = new Vector2(0f, 1f);
        Vector2 uv03 = new Vector2(1f, 1f);

        if (bType ==  BlockType.GRASS && side == SideOfCube.TOP)
        {
            uv00 = blocksUvs[0, 0];
            uv01 = blocksUvs[0, 1];
            uv02 = blocksUvs[0, 2];
            uv03 = blocksUvs[0, 3];
        }
        else if(bType == BlockType.GRASS && side == SideOfCube.BOTTOM)
        {
            uv00 = blocksUvs[(int)(BlockType.DIRT+1), 0];
            uv01 = blocksUvs[(int)(BlockType.DIRT+1), 1];
            uv02 = blocksUvs[(int)(BlockType.DIRT+1), 2];
            uv03 = blocksUvs[(int)(BlockType.DIRT+1), 3];
        }
        else
        {
            uv00 = blocksUvs[(int)(bType+1), 0];
            uv01 = blocksUvs[(int)(bType+1), 1];
            uv02 = blocksUvs[(int)(bType+1), 2];
            uv03 = blocksUvs[(int)(bType+1), 3];
        }
        
        //verts of a cube;
        Vector3 v00 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 v01 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 v02 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 v03 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 v04 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 v05 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 v06 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 v07 = new Vector3(-0.5f, 0.5f, -0.5f);

        switch (side)
        {
            case SideOfCube.BOTTOM:
                verts = new Vector3[] {v00, v01, v02, v03};
                normals = new Vector3[] {Vector3.down, Vector3.down, Vector3.down, Vector3.down};
                uvs = new Vector2[] {uv03, uv02, uv00, uv01};
                tris = new int[] {3, 1, 0, 3, 2, 1};
                break;
            case SideOfCube.TOP:
                verts = new Vector3[] {v07, v06, v05, v04};
                normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
                uvs = new Vector2[] {uv03, uv02, uv00, uv01};
                tris = new int[] {3, 1, 0, 3, 2, 1};
                break;
            case SideOfCube.LEFT:
                verts = new Vector3[] {v07, v04, v00, v03};
                normals = new Vector3[] {Vector3.left, Vector3.left, Vector3.left, Vector3.left};
                uvs = new Vector2[] {uv03, uv02, uv00, uv01};
                tris = new int[] {3, 1, 0, 3, 2, 1};
                break;
            case SideOfCube.RIGHT:
                verts = new Vector3[] {v05, v06, v02, v01};
                normals = new Vector3[] {Vector3.right, Vector3.right, Vector3.right, Vector3.right};
                uvs = new Vector2[] {uv03, uv02, uv00, uv01};
                tris = new int[] {3, 1, 0, 3, 2, 1};
                break;
            case SideOfCube.FRONT:
                verts = new Vector3[] {v04, v05, v01, v00};
                normals = new Vector3[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward};
                uvs = new Vector2[] {uv03, uv02, uv00, uv01};
                tris = new int[] {3, 1, 0, 3, 2, 1};
                break;
            case SideOfCube.BACK:
                verts = new Vector3[] {v06, v07, v03, v02};
                normals = new Vector3[] {Vector3.back, Vector3.back, Vector3.back, Vector3.back};
                uvs = new Vector2[] {uv03, uv02, uv00, uv01};
                tris = new int[] {3, 1, 0, 3, 2, 1};
                break;

        }

        /* normals = new Vector3[]
         {
             Vector3.forward, 
             Vector3.forward, 
             Vector3.forward, 
             Vector3.forward
         };
         
         verts= new Vector3[] {v04, v05, v01, v00};
         uvs = new Vector2[] {uv03, uv02, uv00, uv01 };
         tris = new int[]{3,1,0,3,2,1};*/

        _mesh.vertices = verts;
        _mesh.normals = normals;
        _mesh.uv = uvs;
        _mesh.triangles = tris;

        _mesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.parent = this.gameObject.transform;
        MeshFilter _meshFilter = (MeshFilter) quad.AddComponent(typeof(MeshFilter));
        MeshRenderer _rend = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        _meshFilter.mesh = _mesh;
        _rend.material = _mat;
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
        _meshRenderer.material = _mat;

        foreach (Transform quad in this.transform)
            Destroy(quad.gameObject);
    }

    // Start is called before the first frame update
    void CreateCube()
    {
        CreateQuad(SideOfCube.FRONT);
        CreateQuad(SideOfCube.BACK);
        CreateQuad(SideOfCube.TOP);
        CreateQuad(SideOfCube.BOTTOM);
        CreateQuad(SideOfCube.LEFT);
        CreateQuad(SideOfCube.RIGHT);
        CombineQuads();
    }

    private void Start()
    {
        CreateCube();
    }
}

