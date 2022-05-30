using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
    public struct HexCellFace
    {
        public List<Vector3> Verticies { get; private set; }
        public List<int> Triangles { get; private set; }
        public List<Vector2> UVs { get; private set; }

        public HexCellFace(List<Vector3> verticies, List<int> triangles, List<Vector2> uvs)
        {
            this.Verticies = verticies;
            this.Triangles = triangles;
            this.UVs = uvs;
        }
    }
}