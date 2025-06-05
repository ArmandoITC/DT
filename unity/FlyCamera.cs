using UnityEngine;

/// <summary>
///  FlyCamera: mueve un rig que contiene la cámara indicada.
///  • Coloca este componente en un GameObject vacío (el “rig”).
///  • Haz que la(s) cámara(s) que quieras controlar cuelguen como hijas del rig.
///  • Arrastra la cámara concreta al campo "Camera Transform" en el Inspector.
///    Si lo dejas vacío, tomará la primera cámara hija que encuentre.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FlyCamera : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform de la cámara que se va a mover")]
    [SerializeField] private Transform cameraTransform;   // arrástrala aquí

    [Header("Velocidades")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float fastSpeedMultiplier = 2f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float zoomSensitivity = 10f;
    [SerializeField] private float panSpeed = 0.5f;

    [Header("Colisiones")]
    [SerializeField] private LayerMask collisionMask = ~0;

    Rigidbody rb;

    Vector3 lastMousePosition;
    bool isRotating;

    float yaw, pitch, initialYaw, initialPitch;

    /* ---------- INICIALIZACIÓN ---------- */
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;   // el rig se mueve “a mano”, no por física

        // Si no arrastraron cámara, intenta coger la primera hija con Camera
        if (cameraTransform == null)
        {
            Camera camChild = GetComponentInChildren<Camera>();
            cameraTransform = camChild != null ? camChild.transform : null;
        }

        if (cameraTransform == null)
        {
            Debug.LogError("FlyCamera: No se ha asignado ninguna cámara. " +
                           "Arrastra la cámara hija en el Inspector.");
            enabled = false;
            return;
        }

        initialYaw = transform.eulerAngles.y;
        initialPitch = cameraTransform.localEulerAngles.x;
        yaw = initialYaw;
        pitch = initialPitch;
    }

    /* ---------- BUCLE PRINCIPAL ---------- */
    void Update()
    {
        if (!cameraTransform) return;   // seguridad extra

        HandleMouseRotation();
        HandleMovement();
        HandleZoom();
        HandlePanning();
        HandleClickSelection();
    }

    /* ---------- CONTROL DE ROTACIÓN ---------- */
    void HandleMouseRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (!isRotating) return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -85f, 85f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);      // yaw global
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);    // pitch local
    }

    /* ---------- MOVIMIENTO WASD + QE ---------- */
    void HandleMovement()
    {
        float speed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                      ? movementSpeed * fastSpeedMultiplier
                      : movementSpeed;

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.Q)) move.y -= 1;
        if (Input.GetKey(KeyCode.E)) move.y += 1;

        move = transform.TransformDirection(move.normalized);
        Vector3 target = rb.position + move * speed * Time.deltaTime;

        MoveWithCollision(move, target);
    }

    /* ---------- ZOOM CON RUEDA ---------- */
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f)) return;

        Vector3 zoomDir = cameraTransform.forward * scroll * zoomSensitivity;
        Vector3 targetPos = rb.position + zoomDir;

        MoveWithCollision(zoomDir, targetPos);
    }

    /* ---------- PAN CON BOTÓN CENTRAL ---------- */
    void HandlePanning()
    {
        if (Input.GetMouseButtonDown(2))
            lastMousePosition = Input.mousePosition;

        if (!Input.GetMouseButton(2)) return;

        Vector3 delta = Input.mousePosition - lastMousePosition;
        Vector3 move = (-transform.right * delta.x + -transform.up * delta.y)
                        * panSpeed * Time.deltaTime;

        Vector3 target = rb.position + move;
        MoveWithCollision(move, target);

        lastMousePosition = Input.mousePosition;
    }

    /* ---------- DETECCIÓN DE OBJETOS (ejemplo) ---------- */
    void HandleClickSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = cameraTransform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            ObjectIdentifier obj = hit.collider.GetComponent<ObjectIdentifier>();
            if (obj != null)
            {
                GlobalData.idSeleccionado = obj.ID;
                Debug.Log($"Seleccionaste el objeto con ID: {obj.ID}");
            }
        }
    }

    /* ---------- UTILIDAD: MOVER RESPETANDO COLISIÓN ---------- */
    void MoveWithCollision(Vector3 direction, Vector3 targetPos)
    {
        float distance = direction.magnitude;
        if (!Physics.Raycast(rb.position, direction.normalized, out RaycastHit hit, distance, collisionMask))
            rb.MovePosition(targetPos);
        else
            rb.MovePosition(hit.point - direction.normalized * 0.1f);
    }
}

