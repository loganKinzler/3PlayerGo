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

        gameObject.transform.position += (movement - movement.z * Vector3.forward) * Time.deltaTime * speed;
        gameObject.GetComponent<Camera>().orthographicSize += movement.z * Time.deltaTime * speed;

        KeepCameraInBounds();

    }

    private void KeepCameraInBounds() {
        gameObject.GetComponent<Camera>().orthographicSize = Mathf.Clamp(gameObject.GetComponent<Camera>().orthographicSize, 1,5);
        if (gameObject.GetComponent<Camera>().orthographicSize == 5) {
            gameObject.transform.position = new Vector3(0,0,-10);
            return;
        }

        gameObject.transform.position = new Vector3(
            Mathf.Clamp(gameObject.transform.position.x,
                -(4.75f-gameObject.GetComponent<Camera>().orthographicSize),
                4.75f-gameObject.GetComponent<Camera>().orthographicSize * Screen.width / Screen.height / 2
            ),
            Mathf.Clamp(gameObject.transform.position.y,
                -(4.75f-gameObject.GetComponent<Camera>().orthographicSize / 2),
                4.75f-gameObject.GetComponent<Camera>().orthographicSize * Screen.width / Screen.height / 2
            ),
            -10
        );

    }
}
