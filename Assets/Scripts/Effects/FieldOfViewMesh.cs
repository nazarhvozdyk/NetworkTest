using UnityEngine;

public class FieldOfViewMesh : MonoBehaviour
{
    // FULLY CHAT GPT SCRIPT
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;
    public int meshResolution = 30;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    private Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        DrawFOV();
    }

    private void DrawFOV()
    {
        Vector3[] vertices = new Vector3[meshResolution + 2];
        int[] triangles = new int[meshResolution * 3];

        vertices[0] = Vector3.zero;

        float angleStep = viewAngle / meshResolution;
        for (int i = 0; i <= meshResolution; i++)
        {
            float angle = -viewAngle / 2 + angleStep * i;
            Vector3 dir = DirFromAngle(angle);
            vertices[i + 1] = dir * viewRadius;
        }

        for (int i = 0; i < meshResolution; i++)
        {
            int start = i + 1;
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = start;
            triangles[i * 3 + 2] = start + 1;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private Vector3 DirFromAngle(float angleInDegrees)
    {
        float rad = Mathf.Deg2Rad * angleInDegrees;
        return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
    }
}
