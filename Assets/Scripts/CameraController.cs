using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    public float moveSpeed = 10000.0f; // 移动 
    public float rotationSpeed = 10.0f; // 旋转速度

    void Update()
    {
        // WASD 键控制移动
        float h = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(h, 0, v);

        // 鼠标右键控制旋转
        if (Input.GetMouseButton(1))
        {
            float hh = rotationSpeed * Input.GetAxis("Mouse X");;
            float vv = rotationSpeed * Input.GetAxis("Mouse Y");;

            transform.Rotate(Vector3.up, hh, Space.World);
            transform.Rotate(Vector3.right, -vv, Space.Self);
        }
    }
}
