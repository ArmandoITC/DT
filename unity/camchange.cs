using UnityEngine;

/*
 * Arrástralo a cualquier GameObject vacío.
 *  - targetCamera : la cámara cuyo “Priority” (depth) se cambiará.
 *  - delaySeconds : segundos que esperas antes de cambiarlo.
 *  - newPriority  : nuevo valor que verás en el campo Priority del Inspector.
 *
 * El campo Priority del Inspector es int, pero Camera.depth es float,
 * por eso newPriority es float.
 */
public class CameraPriorityTimer : MonoBehaviour
{
    [Header("Asignar en Inspector")]
    public Camera targetCamera;
    public float delaySeconds = 5f;
    public float newPriority = 10f;

    float timer;
    bool done;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
        {
            Debug.LogError("CameraPriorityTimer: Asigna una cámara o crea una MainCamera.");
            enabled = false;
        }
    }

    void Update()
    {
        if (done) return;

        timer += Time.deltaTime;
        if (timer >= delaySeconds)
        {
            targetCamera.depth = newPriority;   // <- aquí se cambia “Priority”
            done = true;
        }
    }
}
