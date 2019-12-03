using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
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
        STONE,
        DIAMOND,
        BEDROCK,
        REDSTONE,
        AIR,
    };
    
    private BlockType bType;
    private GameObject _parent;
    private Vector3 _position;
    private Material _material;
    private Chunk _chunkOwner;
    public bool bIsSolid;
    

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
        {new Vector2(0, 0.875f), new Vector2(0.0625f, 0.875f), new Vector2(0f, 0.9375f), new Vector2(0.0625f, 0.9375f)},
        /*diamond*/
        {new Vector2(0.125f, 0.75f),new Vector2(0.1875f, 0.75f),new Vector2(0.125f, 0.8125f),new Vector2(0.1875f, 0.8125f),},
        /*BEDROCK*/
        {new Vector2(0.0625f, 0.875f),new Vector2(0.125f, 0.875f),new Vector2(0.0625f, 0.9375f),new Vector2(0.125f, 0.9375f),},
        /*REDSTONE*/
        {new Vector2(0.1875f, 0.75f),new Vector2(0.25f, 0.75f),new Vector2(0.1875f, 0.8125f),new Vector2(0.25f, 0.8125f),}

    };

    public Block(BlockType block, Vector3 position, GameObject parent, Chunk chunk)
    {
        bType = block; 
        _position = position;
        _parent = parent;
        _chunkOwner = chunk;
        if (bType == BlockType.AIR)
        {
            bIsSolid = false;
        }
        else
        {
            bIsSolid = true;
        }
        
    }
    
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

        GameObject quad = new GameObject("Quad");
        quad.transform.position = _position;
        quad.transform.parent = _parent.transform;
        MeshFilter _meshFilter = (MeshFilter) quad.AddComponent(typeof(MeshFilter));
        /*After combining quads into a singular mesh we don't need a renderer for each single quad, one is added to the whole mesh already*/
        //MeshRenderer _rend = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //_rend.material = _material;
        _meshFilter.mesh = _mesh;
    }

        public void Draw()
        {
            if (bType == BlockType.AIR)
            {
                return;
            }
            if (!HasSolidNeighbor((int)_position.x, (int)_position.y, (int)_position.z +1)){
                CreateQuad(SideOfCube.FRONT);
            }
            if (!HasSolidNeighbor((int)_position.x, (int)_position.y, (int)_position.z -1)){
                CreateQuad(SideOfCube.BACK);
            }
            if (!HasSolidNeighbor((int)_position.x, (int)_position.y +1, (int)_position.z)){
                CreateQuad(SideOfCube.TOP);
            }
            if (!HasSolidNeighbor((int)_position.x, (int)_position.y- 1, (int)_position.z)){
                CreateQuad(SideOfCube.BOTTOM);
            }
            if (!HasSolidNeighbor((int)_position.x - 1, (int)_position.y, (int)_position.z)){
                CreateQuad(SideOfCube.LEFT);
            }
            if (!HasSolidNeighbor((int)_position.x +1 , (int)_position.y, (int)_position.z)){
                CreateQuad(SideOfCube.RIGHT);
            }
        }


        int ConvertBlockIndexToLocal(int i)
        {
            if (i == -1)
            {
                i = World.chunkSize - 1;
            }else if (i == World.chunkSize)
            {
                i = 0;
            }

            return i;
        }
        bool HasSolidNeighbor(int neighborX, int neighborY, int neighborZ)
        {
            Block[,,] chunks;
            chunks = _chunkOwner._chunkData;
            if (neighborX < 0|| neighborX >= World.chunkSize || neighborY < 0|| neighborY >= World.chunkSize|| neighborZ< 0|| neighborZ >= World.chunkSize )
            {
                Vector3 neighborChunkPosition = this._parent.transform.position +
                                                new Vector3((neighborX - (int) _position.x) * World.chunkSize,
                                                    (neighborY - (int) _position.y) * World.chunkSize,
                                                    (neighborZ - (int) _position.z) * World.chunkSize);

                string neighborName = World.BuildChunkName(neighborChunkPosition);

                neighborX = ConvertBlockIndexToLocal(neighborX);
                neighborY = ConvertBlockIndexToLocal(neighborY);
                neighborZ = ConvertBlockIndexToLocal(neighborZ);

                Chunk _neightborChunk;
                if (World.chunks.TryGetValue(neighborName, out _neightborChunk))
                {
                    chunks = _neightborChunk._chunkData;
                }
                else
                    return false;
            }
            else
            {
                chunks = _chunkOwner._chunkData;
            }
            try
            {
                return chunks[neighborX, neighborY, neighborZ].bIsSolid;
            }
            catch (System.IndexOutOfRangeException) {}

            return false;
        }
        
        
}
