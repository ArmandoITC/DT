using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WindWobble : MonoBehaviour
{
    [Header("Wind Strength Range")]
    public float windStrengthMin = 0.01f;
    public float windStrengthMax = 0.1f;

    [Header("Wind Speed Range")]
    public float windSpeedMin = 0.5f;
    public float windSpeedMax = 2.0f;

    [Header("Wind Direction Range")]
    public Vector3 windDirectionMin = new Vector3(-1, 0, -1);
    public Vector3 windDirectionMax = new Vector3(1, 0, 1);

    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;

    private Vector3 windDirection;
    private float windStrength;
    private float windSpeed;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        if (mf.sharedMesh == null || !mf.sharedMesh.isReadable)
        {
            Debug.LogWarning($"{name}: Mesh is not readable or null. WindWobble will be disabled.");
            enabled = false;
            return;
        }

        // Clonar el mesh para evitar modificar el original
        mesh = Instantiate(mf.sharedMesh);
        mf.mesh = mesh;

        originalVertices = mesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];

        // Dirección, fuerza y velocidad del viento aleatorias dentro de los rangos
        windDirection = new Vector3(
            Random.Range(windDirectionMin.x, windDirectionMax.x),
            Random.Range(windDirectionMin.y, windDirectionMax.y),
            Random.Range(windDirectionMin.z, windDirectionMax.z)
        ).normalized;

        windStrength = Random.Range(windStrengthMin, windStrengthMax);
        windSpeed = Random.Range(windSpeedMin, windSpeedMax);
    }

    void Update()
    {
        if (originalVertices == null || mesh == null)
            return;

        float t = Time.time * windSpeed;

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            float wave = Mathf.Sin(Vector3.Dot(vertex, windDirection) + t);
            displacedVertices[i] = vertex + windDirection * wave * windStrength;
        }

        mesh.vertices = displacedVertices;

        try
        {
            mesh.RecalculateNormals();
        }
        catch { }
    }
}

