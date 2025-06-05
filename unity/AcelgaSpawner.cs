using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Prefab del objeto a instanciar
    public Vector3[] spawnPositions; // Posiciones donde se crearán los objetos
    private int idCounter = 1; // ← ¡Comenzamos desde 1!

    void Start()
    {
        spawnPositions = new Vector3[]
        {
            // tubo 1_1
            new Vector3(1.33f, 0.41f, 0.30f),
            new Vector3(1.33f, 0.41f, 0.49f),
            new Vector3(1.33f, 0.41f, 0.68f),
            new Vector3(1.33f, 0.41f, 0.87f),
            new Vector3(1.33f, 0.41f, 1.06f),
            new Vector3(1.33f, 0.41f, 1.25f),
            new Vector3(1.33f, 0.41f, 1.44f),
            new Vector3(1.33f, 0.41f, 1.63f),
            new Vector3(1.33f, 0.41f, 1.82f),
            new Vector3(1.33f, 0.41f, 2.01f),
            new Vector3(1.33f, 0.41f, 2.20f),
            new Vector3(1.33f, 0.41f, 2.39f),
            new Vector3(1.33f, 0.41f, 2.58f),
            new Vector3(1.33f, 0.41f, 2.77f),

            new Vector3(1.171f, 0.41f, 0.30f),
            new Vector3(1.171f, 0.41f, 0.49f),
            new Vector3(1.171f, 0.41f, 0.68f),
            new Vector3(1.171f, 0.41f, 0.87f),
            new Vector3(1.171f, 0.41f, 1.06f),
            new Vector3(1.171f, 0.41f, 1.25f),
            new Vector3(1.171f, 0.41f, 1.44f),
            new Vector3(1.171f, 0.41f, 1.63f),
            new Vector3(1.171f, 0.41f, 1.82f),
            new Vector3(1.171f, 0.41f, 2.01f),
            new Vector3(1.171f, 0.41f, 2.20f),
            new Vector3(1.171f, 0.41f, 2.39f),
            new Vector3(1.171f, 0.41f, 2.58f),
            new Vector3(1.171f, 0.41f, 2.77f),



            new Vector3(1.33f, 1.01f, 0.30f),
            new Vector3(1.33f, 1.01f, 0.49f),
            new Vector3(1.33f, 1.01f, 0.68f),
            new Vector3(1.33f, 1.01f, 0.87f),
            new Vector3(1.33f, 1.01f, 1.06f),
            new Vector3(1.33f, 1.01f, 1.25f),
            new Vector3(1.33f, 1.01f, 1.44f),
            new Vector3(1.33f, 1.01f, 1.63f),
            new Vector3(1.33f, 1.01f, 1.82f),
            new Vector3(1.33f, 1.01f, 2.01f),
            new Vector3(1.33f, 1.01f, 2.20f),
            new Vector3(1.33f, 1.01f, 2.39f),
            new Vector3(1.33f, 1.01f, 2.58f),
            new Vector3(1.33f, 1.01f, 2.77f),

            new Vector3(1.171f, 1.01f, 0.30f),
            new Vector3(1.171f, 1.01f, 0.49f),
            new Vector3(1.171f, 1.01f, 0.68f),
            new Vector3(1.171f, 1.01f, 0.87f),
            new Vector3(1.171f, 1.01f, 1.06f),
            new Vector3(1.171f, 1.01f, 1.25f),
            new Vector3(1.171f, 1.01f, 1.44f),
            new Vector3(1.171f, 1.01f, 1.63f),
            new Vector3(1.171f, 1.01f, 1.82f),
            new Vector3(1.171f, 1.01f, 2.01f),
            new Vector3(1.171f, 1.01f, 2.20f),
            new Vector3(1.171f, 1.01f, 2.39f),
            new Vector3(1.171f, 1.01f, 2.58f),
            new Vector3(1.171f, 1.01f, 2.77f),




            new Vector3(1.33f, 1.612f, 0.30f),
            new Vector3(1.33f, 1.612f, 0.49f),
            new Vector3(1.33f, 1.612f, 0.68f),
            new Vector3(1.33f, 1.612f, 0.87f),
            new Vector3(1.33f, 1.612f, 1.06f),
            new Vector3(1.33f, 1.612f, 1.25f),
            new Vector3(1.33f, 1.612f, 1.44f),
            new Vector3(1.33f, 1.612f, 1.63f),
            new Vector3(1.33f, 1.612f, 1.82f),
            new Vector3(1.33f, 1.612f, 2.01f),
            new Vector3(1.33f, 1.612f, 2.20f),
            new Vector3(1.33f, 1.612f, 2.39f),
            new Vector3(1.33f, 1.612f, 2.58f),
            new Vector3(1.33f, 1.612f, 2.77f),

            new Vector3(1.171f, 1.612f, 0.30f),
            new Vector3(1.171f, 1.612f, 0.49f),
            new Vector3(1.171f, 1.612f, 0.68f),
            new Vector3(1.171f, 1.612f, 0.87f),
            new Vector3(1.171f, 1.612f, 1.06f),
            new Vector3(1.171f, 1.612f, 1.25f),
            new Vector3(1.171f, 1.612f, 1.44f),
            new Vector3(1.171f, 1.612f, 1.63f),
            new Vector3(1.171f, 1.612f, 1.82f),
            new Vector3(1.171f, 1.612f, 2.01f),
            new Vector3(1.171f, 1.612f, 2.20f),
            new Vector3(1.171f, 1.612f, 2.39f),
            new Vector3(1.171f, 1.612f, 2.58f),
            new Vector3(1.171f, 1.612f, 2.77f),




            // tubo 1_1
            new Vector3(-1.035f, 0.41f, 0.30f),
            new Vector3(-1.035f, 0.41f, 0.49f),
            new Vector3(-1.035f, 0.41f, 0.68f),
            new Vector3(-1.035f, 0.41f, 0.87f),
            new Vector3(-1.035f, 0.41f, 1.06f),
            new Vector3(-1.035f, 0.41f, 1.25f),
            new Vector3(-1.035f, 0.41f, 1.44f),
            new Vector3(-1.035f, 0.41f, 1.63f),
            new Vector3(-1.035f, 0.41f, 1.82f),
            new Vector3(-1.035f, 0.41f, 2.01f),
            new Vector3(-1.035f, 0.41f, 2.20f),
            new Vector3(-1.035f, 0.41f, 2.39f),
            new Vector3(-1.035f, 0.41f, 2.58f),
            new Vector3(-1.035f, 0.41f, 2.77f),

            new Vector3(-1.202f, 0.41f, 0.30f),
            new Vector3(-1.202f, 0.41f, 0.49f),
            new Vector3(-1.202f, 0.41f, 0.68f),
            new Vector3(-1.202f, 0.41f, 0.87f),
            new Vector3(-1.202f, 0.41f, 1.06f),
            new Vector3(-1.202f, 0.41f, 1.25f),
            new Vector3(-1.202f, 0.41f, 1.44f),
            new Vector3(-1.202f, 0.41f, 1.63f),
            new Vector3(-1.202f, 0.41f, 1.82f),
            new Vector3(-1.202f, 0.41f, 2.01f),
            new Vector3(-1.202f, 0.41f, 2.20f),
            new Vector3(-1.202f, 0.41f, 2.39f),
            new Vector3(-1.202f, 0.41f, 2.58f),
            new Vector3(-1.202f, 0.41f, 2.77f),



            new Vector3(-1.035f, 1.01f, 0.30f),
            new Vector3(-1.035f, 1.01f, 0.49f),
            new Vector3(-1.035f, 1.01f, 0.68f),
            new Vector3(-1.035f, 1.01f, 0.87f),
            new Vector3(-1.035f, 1.01f, 1.06f),
            new Vector3(-1.035f, 1.01f, 1.25f),
            new Vector3(-1.035f, 1.01f, 1.44f),
            new Vector3(-1.035f, 1.01f, 1.63f),
            new Vector3(-1.035f, 1.01f, 1.82f),
            new Vector3(-1.035f, 1.01f, 2.01f),
            new Vector3(-1.035f, 1.01f, 2.20f),
            new Vector3(-1.035f, 1.01f, 2.39f),
            new Vector3(-1.035f, 1.01f, 2.58f),
            new Vector3(-1.035f, 1.01f, 2.77f),

            new Vector3(-1.202f, 1.01f, 0.30f),
            new Vector3(-1.202f, 1.01f, 0.49f),
            new Vector3(-1.202f, 1.01f, 0.68f),
            new Vector3(-1.202f, 1.01f, 0.87f),
            new Vector3(-1.202f, 1.01f, 1.06f),
            new Vector3(-1.202f, 1.01f, 1.25f),
            new Vector3(-1.202f, 1.01f, 1.44f),
            new Vector3(-1.202f, 1.01f, 1.63f),
            new Vector3(-1.202f, 1.01f, 1.82f),
            new Vector3(-1.202f, 1.01f, 2.01f),
            new Vector3(-1.202f, 1.01f, 2.20f),
            new Vector3(-1.202f, 1.01f, 2.39f),
            new Vector3(-1.202f, 1.01f, 2.58f),
            new Vector3(-1.202f, 1.01f, 2.77f),



            new Vector3(-1.035f, 1.612f, 0.30f),
            new Vector3(-1.035f, 1.612f, 0.49f),
            new Vector3(-1.035f, 1.612f, 0.68f),
            new Vector3(-1.035f, 1.612f, 0.87f),
            new Vector3(-1.035f, 1.612f, 1.06f),
            new Vector3(-1.035f, 1.612f, 1.25f),
            new Vector3(-1.035f, 1.612f, 1.44f),
            new Vector3(-1.035f, 1.612f, 1.63f),
            new Vector3(-1.035f, 1.612f, 1.82f),
            new Vector3(-1.035f, 1.612f, 2.01f),
            new Vector3(-1.035f, 1.612f, 2.20f),
            new Vector3(-1.035f, 1.612f, 2.39f),
            new Vector3(-1.035f, 1.612f, 2.58f),
            new Vector3(-1.035f, 1.612f, 2.77f),

            new Vector3(-1.202f, 1.612f, 0.30f),
            new Vector3(-1.202f, 1.612f, 0.49f),
            new Vector3(-1.202f, 1.612f, 0.68f),
            new Vector3(-1.202f, 1.612f, 0.87f),
            new Vector3(-1.202f, 1.612f, 1.06f),
            new Vector3(-1.202f, 1.612f, 1.25f),
            new Vector3(-1.202f, 1.612f, 1.44f),
            new Vector3(-1.202f, 1.612f, 1.63f),
            new Vector3(-1.202f, 1.612f, 1.82f),
            new Vector3(-1.202f, 1.612f, 2.01f),
            new Vector3(-1.202f, 1.612f, 2.20f),
            new Vector3(-1.202f, 1.612f, 2.39f),
            new Vector3(-1.202f, 1.612f, 2.58f),
            new Vector3(-1.202f, 1.612f, 2.77f),

        };

        SpawnObjects();
    }

    void SpawnObjects()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("No se ha asignado un objeto para instanciar.");
            return;
        }

        foreach (Vector3 position in spawnPositions)
        {
            GameObject newObject = Instantiate(objectToSpawn, position, Quaternion.identity);

            // Asegurar que no haya un ObjectIdentifier previo
            ObjectIdentifier existing = newObject.GetComponent<ObjectIdentifier>();
            if (existing != null)
            {
                DestroyImmediate(existing); // Eliminamos la copia heredada del prefab si existe
            }

            // Agregar componente limpio y asignar ID único
            ObjectIdentifier identifier = newObject.AddComponent<ObjectIdentifier>();
            identifier.AssignID(idCounter);

            Debug.Log($"Objeto creado en {position} con ID: {identifier.ID}");
            idCounter++;
        }
    }
}
