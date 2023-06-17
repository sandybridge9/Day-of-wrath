using UnityEngine;

public class UnitMovementControllerBase : MonoBehaviour
{
    public float MovementSpeed;
    public LayerMask HitLayers;

    private Vector3 newPosition;
    
    void Start()
    {
        newPosition = transform.position;
    }

    void Update()
    {
        GetNewPositionByMouse();

        MoveToNewPosition();
    }

    private void GetNewPositionByMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            var castPointRay = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(castPointRay, out var hitRay, Mathf.Infinity, HitLayers))
            {
                newPosition = hitRay.point;
            }
        }
    }

    private void MoveToNewPosition()
    {
        if(transform.position == newPosition)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, newPosition, MovementSpeed * Time.deltaTime);
    }

    private void MoveUsingArrowKeys()
    {
        var movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.position += MovementSpeed * Time.deltaTime * movement;
    }
}
