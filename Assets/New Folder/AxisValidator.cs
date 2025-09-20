using UnityEngine;

public class AxisValidator : MonoBehaviour
{
    void Start()
    {
        Vector3 up = transform.up;
        Vector3 forward = transform.forward;

        if (up != Vector3.up || forward != Vector3.forward)
        {
            Debug.LogWarning($"[AxisValidator] El modelo '{gameObject.name}' tiene ejes incorrectos." +
                             $" UP: {up}, FORWARD: {forward}");
        }
        else
        {
            Debug.Log($"[AxisValidator] El modelo '{gameObject.name}' está correctamente alineado.");
        }
    }
}
