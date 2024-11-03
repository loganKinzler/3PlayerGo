using System.Drawing;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    private Vector3 movement;
    private float speed = 2.5f;

    void Update()
    {
        movement = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"),
            -100*Input.GetAxisRaw("Mouse ScrollWheel")
        );

        transform.position += (movement - movement.z * Vector3.forward) * Time.deltaTime * speed;
        GetComponent<Camera>().orthographicSize += movement.z * Time.deltaTime * speed;

        KeepCameraInBounds();

    }

    private void KeepCameraInBounds() {
        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, 1,5);
        if (GetComponent<Camera>().orthographicSize == 5) {
            transform.position = new Vector3(0,0,-10);
            return;
        }

        // Get X position
        float minX = -(4.75f - GetComponent<Camera>().orthographicSize);
        float maxX = 4.75f - GetComponent<Camera>().orthographicSize * Screen.width / Screen.height / 2;

        float posX;
        if (minX > maxX)
            posX = 0;
        else
            posX = Mathf.Clamp(gameObject.transform.position.x, minX, maxX);

        // Get Y position
        float minY = -(4.75f - GetComponent<Camera>().orthographicSize / 2);
        float maxY = 4.75f - GetComponent<Camera>().orthographicSize * Screen.width / Screen.height / 2;

        float posY;
        if (minY > maxY)
            posY = 0;
        else
            posY = Mathf.Clamp(transform.position.y, minY, maxY);


        // Set position
        transform.position = new Vector3(posX, posY, -10);
    }
}
