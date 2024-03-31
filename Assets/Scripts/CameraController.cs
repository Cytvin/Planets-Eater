using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    private void Update()
    {
        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");
        float yMovement = Input.GetAxis("Mouse ScrollWheel");

        Vector3 movement = new Vector3(xMovement, zMovement, yMovement * 10);

        _camera.transform.Translate(movement);
    }
}
