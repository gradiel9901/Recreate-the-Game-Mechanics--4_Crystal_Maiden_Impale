using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject impaleOBJ;
    public bool isBirdsEyeView;
    public LayerMask layerMask;
    public Vector3 birdsEyeOffset;
    public float speed = 10.0f;
    private float translation;
    private float straffe;
    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
    private Vector2 mouseLook;
    private Vector2 smoothV;
    public Transform cam;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = transform;
        cam = Camera.main.transform;
        if (!isBirdsEyeView) { Cursor.lockState = CursorLockMode.Locked; } else { cam.transform.SetParent(null); }
    }

    void Update()
    {
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        if (isBirdsEyeView)
        {
            cam.transform.position = playerTransform.position + birdsEyeOffset;
        }
        else
        {
            var movementDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            movementDirection = Vector2.Scale(movementDirection, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
            smoothV.x = Mathf.Lerp(smoothV.x, movementDirection.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, movementDirection.y, 1f / smoothing);
            mouseLook += smoothV;
            cam.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            transform.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.up);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Impale();
        }
    }

    void Impale()
    {
        if (isBirdsEyeView)
        {
            RaycastHit hit;
            Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                var spawnCalculation = transform.position + transform.forward;
                Quaternion rotation = Quaternion.LookRotation(hit.point - transform.position, Vector3.up);
                Instantiate(impaleOBJ, spawnCalculation, rotation);
            }
        }
        else
        {
            var spawnCalculation = transform.position + transform.forward;
            Instantiate(impaleOBJ, spawnCalculation, transform.rotation);
        }
    }
}
