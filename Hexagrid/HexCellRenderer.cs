using System;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.UsefulThings.UIBinding;
using PM.UsefulThings.UIBinding.Base;

namespace PM.UsefulThings
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class HexCellRenderer : BaseBindingBehaviourTarget
    {
        [SerializeField] private StringProperty pos = new StringProperty();
        
        public float InnerSize;
        public float OuterSize;
        public float InnerHeight;
        public float OuterHeight;
        public float BevelSize;
        public float BevelHeight;
        public bool IsHorizontalAlignment;
        [SerializeField]
        private Mesh mesh;
        [SerializeField]
        private MeshFilter meshFilter;
        [SerializeField]
        private MeshRenderer meshRenderer;

        public Material Material;

        private List<HexCellFace> faces = new List<HexCellFace>();

        private void Awake()
        {
            mesh = new Mesh
            {
                name = "Hex"
            };

            meshFilter.mesh = mesh;
            meshRenderer.material = Material;
        }

        public void SetPosition(Vector3Int position)
        {
            this.pos.value = $"{position.x}, {position.y}, {position.z}";
        }

        private void OnEnable()
        {
            DrawMesh();
        }

        // private void OnValidate()
        // {
        //     DrawMesh();
        // }

        public void DrawMesh()
        {
            DrawFaces();
            CombineFaces();
        }

        private void DrawFaces()
        {
            faces = new List<HexCellFace>();
            var scaledInnerSize = InnerSize / Mathf.Sqrt(3) * 2;
            var scaledOuterSize = OuterSize / Mathf.Sqrt(3) * 2;

            var effectiveOuterSize = (BevelSize != 0 && BevelHeight != 0) ? scaledOuterSize / 2 - BevelSize : scaledOuterSize / 2;
            var effectiveOuterHeight = (BevelSize != 0 && BevelHeight != 0) ? OuterHeight - BevelHeight : OuterHeight;
                
            for (int point = 0; point < 6; point++)
            {
                faces.Add(CreateFace(scaledInnerSize / 2, effectiveOuterSize, InnerHeight, OuterHeight, point));
                if (BevelSize != 0 && BevelHeight != 0)
                    faces.Add(CreateFace(effectiveOuterSize, scaledOuterSize / 2, OuterHeight, effectiveOuterHeight, point));
                
                faces.Add(CreateFace(scaledInnerSize / 2, scaledOuterSize / 2, 0, 0, point, true));
                
                faces.Add(CreateFace(scaledOuterSize / 2, scaledOuterSize / 2, 0, effectiveOuterHeight, point, true));
                faces.Add(CreateFace(scaledInnerSize / 2, scaledInnerSize / 2, 0, InnerHeight, point));
            }
        }

        private HexCellFace CreateFace(float innerRad, float outerRad, float innerHeight, float outerHeight, int point,
            bool Reverse = false)
        {
            var A = GetPoint(innerRad, innerHeight, point);
            var B = GetPoint(innerRad, innerHeight, (point + 1) % 6);
            var C = GetPoint(outerRad, outerHeight, (point + 1) % 6);
            var D = GetPoint(outerRad, outerHeight, point);

            var verticies = new List<Vector3>() { A, B, C, D};
            var triangles = new List<int>() {0, 1, 2, 2, 3, 0};
            var uvs = new List<Vector2>() {Vector2.zero, Vector2.up, Vector2.one, Vector2.right};

            if (Reverse)
            {
                verticies.Reverse();
            }

            return new HexCellFace(verticies, triangles, uvs);
        }

        private Vector3 GetPoint(float size, float height, int index)
        {
            float angle = ((IsHorizontalAlignment ? 0 : 30) + index * 60) * Mathf.Deg2Rad;
            return new Vector3(size * Mathf.Cos(angle), height, size * Mathf.Sin(angle));
        }

        private void CombineFaces()
        {
            var verticies = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();

            for (int i = 0; i < faces.Count; i++)
            {
                verticies.AddRange(faces[i].Verticies);
                uvs.AddRange(faces[i].UVs);

                int offset = i * 4;
                foreach (var triangle in faces[i].Triangles)
                {
                    triangles.Add(triangle + offset);
                }
            }

            mesh.Clear();
            mesh.vertices = verticies.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
        }

        public void SetColor(Color color)
        {
            mesh.colors = new Color[mesh.vertices.Length];
            for (int i = 0; i < mesh.colors.Length; i++)
            {
                mesh.colors[i] = color;
            }
        }
    }
}