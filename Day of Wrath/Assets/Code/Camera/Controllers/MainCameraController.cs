using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public float MovementSpeed = 5.0f;
    public float mouseSensitivity = 1.0f;
    private Vector3 lastPosition;

    void Start()
    {
        
    }

    void Update()
    {
        Move();

        //Pan();
    }

    private void Move()
    {
        var movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.position += MovementSpeed * Time.deltaTime * movement;
    }
    
    //private void Pan()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        lastPosition = Input.mousePosition;
    //    }

    //    if (Input.GetMouseButton(0))
    //    {
    //        var delta = Input.mousePosition - lastPosition;

    //        transform.Translate(delta.x * mouseSensitivity, delta.y * mouseSensitivity, 0);

    //        lastPosition = Input.mousePosition;
    //    }

    //}
}
