using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    public float swayStep = 0.01f;
    public float swayMaxStepDistance = 0.06f;
    Vector3 swayPosition;

    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayRotation;

    [SerializeField]
    private float smooth;
    [SerializeField]
    private float rotationSmooth;

    void Update()
    {
        CalculateSway();
        CalculateRotation();
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayRotation), Time.deltaTime * rotationSmooth);
    }
    void CalculateRotation()
    {
        Vector2 invert = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * -rotationStep;
        invert.x = Mathf.Clamp(invert.x, -maxRotationStep, maxRotationStep);
        invert.y = Mathf.Clamp(invert.y, -maxRotationStep, maxRotationStep);
        swayRotation = new Vector3(invert.y, invert.x, invert.x);

    }
    void CalculateSway()
    {
        Vector2 invert = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * -swayStep;
        invert.x = Mathf.Clamp(invert.x, -swayMaxStepDistance, swayMaxStepDistance);
        invert.y = Mathf.Clamp(invert.y, -swayMaxStepDistance, swayMaxStepDistance);
        swayPosition = invert;
    }
}
