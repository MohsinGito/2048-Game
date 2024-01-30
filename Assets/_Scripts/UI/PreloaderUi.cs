using UnityEngine;
using UnityEngine.UI;

public class PreloaderUi : MonoBehaviour
{

    public Image bufferIcon;
    public float rotationSpeed = 50f;
    private bool isRotating = false;

    private void OnEnable()
    {
        isRotating = true;
    }

    private void Update()
    {
        if (isRotating)
        {
            bufferIcon.transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        isRotating = false;
    }

}