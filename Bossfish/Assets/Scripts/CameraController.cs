using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Look Settings")]
    public InputAction lookAction;
    public InputAction switchAction;
    public float mouseSensitivity;
    Vector2 rotation;
    public float moveSpeed;

    public delegate void CameraControllerDelegate();
    public event CameraControllerDelegate OnSwitchPosition;

    private void OnEnable()
    {
        lookAction.Enable();
        switchAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnDisable()
    {
        lookAction.Disable();
        switchAction.Disable();
    }

    private void Update()
    {
        //Перемикає позицію камери
        if (switchAction.triggered)
        {
            OnSwitchPosition?.Invoke();
        }

        Look();
    }
    void Look()
    {
        // Знаходить напрямок обертання
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>();
        mouseDelta *= mouseSensitivity;
        rotation += mouseDelta;

        //Обертає камеру
        rotation.y = Mathf.Clamp(rotation.y, -90, 90);
        transform.localRotation = Quaternion.Euler(-rotation.y, rotation.x, 0f);
    }
}
