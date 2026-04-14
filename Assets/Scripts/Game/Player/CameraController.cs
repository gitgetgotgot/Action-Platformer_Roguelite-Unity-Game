using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    public GameObject playerTarget;
    public float speed = 1f;
    private Vector3 offset;
    void Start() {
        offset = new Vector3(0, 0, -10);
    }

    void LateUpdate() {
        Vector2 smoothedPos = Vector2.Lerp(transform.position, playerTarget.transform.position, speed * Time.deltaTime);
        transform.position = (Vector3)smoothedPos + offset;
    }
}
