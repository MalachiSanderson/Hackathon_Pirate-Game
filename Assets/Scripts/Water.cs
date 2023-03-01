using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class Water : MonoBehaviour
{
    [Header("Tile Count")]
    public int tileWidth = 100;
    public int tileLength = 100;

    [Header("Tile Size")]
    public float tileSize = 1f;
    public float maxHeight = 2f;

    [Header("Wave")]
    public Color darkColor = Color.blue;
    public Color lightColor = Color.cyan;
    public List<Wave> waves = new List<Wave>();

    [Header("Debug")] public bool showGizmos = false;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        mesh.Clear();

        // Vertices
        vertices = new Vector3[(tileWidth + 1) * (tileLength + 1)];
        for (int i = 0, z = 0; z < tileLength + 1; z++)
        {
            for (int x = 0; x < tileWidth + 1; x++)
            {
                vertices[i] = new Vector3(x * tileSize, transform.position.y, z * tileSize);
                i++;
            }
        }

        // Triangles
        triangles = new int[vertices.Length * 6];
        for (int i = 0, z = 0; z < tileLength; z++)
        {
            for (int x = 0; x < tileWidth; x++)
            {
                triangles[(i * 6) + 0] = i + 0;
                triangles[(i * 6) + 1] = i + tileWidth + 1;
                triangles[(i * 6) + 2] = i + 1;
                triangles[(i * 6) + 3] = i + 1;
                triangles[(i * 6) + 4] = i + tileWidth + 1;
                triangles[(i * 6) + 5] = i + tileWidth + 2;
                i++;
            }

            i++;
        }

        // Colors
        colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.Lerp(Color.cyan, Color.blue, Random.value);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    private void Animate()
    {
        for (int i = 0, z = 0; z < tileLength + 1; z++)
        {
            for (int x = 0; x < tileWidth + 1; x++)
            {
                float tileX = x * tileSize;
                float tileZ = z * tileSize;

                float y = 0;
                foreach (var wave in waves)
                {
                    float time = Time.time * wave.speed;

                    if (wave.waveType == WaveType.Random)
                        y += Random.value * wave.height;
                    else if (wave.waveType == WaveType.Perlin)
                        y += Mathf.PerlinNoise(time + tileX * wave.scale.x, time + tileZ * wave.scale.y) * wave.height;
                    else if (wave.waveType == WaveType.Sine)
                        y += Mathf.Sin(time + (tileX + tileZ) * wave.scale.x) * wave.height;
                    else if (wave.waveType == WaveType.Cosine)
                        y += Mathf.Cos(time + (tileX + tileZ) * wave.scale.x) * wave.height;
                }

                y /= waves.Count;

                y = Math.Min(y, maxHeight);

                vertices[i].y = y;
                colors[i] = Color.Lerp(darkColor, lightColor, y / maxHeight);
                i++;
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
    }

    private void Update()
    {
        Animate();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Regenerate"))
        {
            GenerateMesh();
        }
    }

    private void OnDrawGizmos()
    {
        var size = new Vector3(tileWidth * tileSize, maxHeight, tileLength * tileSize);
        Gizmos.DrawWireCube(transform.position + size / 2, size);

        if (vertices == null || !showGizmos) return;

        foreach (var vertex in vertices)
            Gizmos.DrawSphere(transform.position + vertex, 0.1f);
    }

    [System.Serializable]
    public struct Wave
    {
        public Vector2 scale;
        public float height;
        public float speed;
        public WaveType waveType;
    }

    [System.Serializable]
    public enum WaveType
    {
        Sine,
        Cosine,
        Perlin,
        Random,
    }
}
