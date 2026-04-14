using UnityEngine;

public class MaceTrap : MonoBehaviour
{
    public float rotate_speed = 60.0f;
    public bool CW_rotation = true;
    public float DMG = 20.0f;
    public void SetRotation(float speed, bool CW)
    {
        rotate_speed = speed;
        CW_rotation = CW;
    }
    void Update()
    {
        if (CW_rotation)
        {
            transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * -rotate_speed, Space.Self);
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * rotate_speed, Space.Self);
        }
    }
}
