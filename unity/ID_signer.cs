using UnityEngine;

public class ObjectIdentifier : MonoBehaviour
{
    public int ID { get; private set; }

    public void AssignID(int id)
    {
        ID = id;
        // Confirmar en consola por objeto
        Debug.Log($"Asignado ID {ID} a {gameObject.name}");
    }
}
