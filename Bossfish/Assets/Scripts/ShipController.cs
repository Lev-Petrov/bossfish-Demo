
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour// ShipBuoyancy
{
    public bool playerAtTheHelm;

    [SerializeField] Transform camTarget;
    [SerializeField] CameraController cam;
    [SerializeField] AudioSource engineNoise;

    [Header("Move")]
    public InputAction moveAction;
    [SerializeField] Transform throttle;
    public float maxSpeedForward = 10;
    public float maxSpeedBack = 7;
    public float speedMultiplier = 2;

    float currentSpeed;

    [Header("Rotation")]
    [SerializeField] Transform wheel;
    public float maxRotSpeed = 4;
    public float rotSpeedMultiplier = 1;

    float currentRotSpeed;

    private void OnEnable()
    {
        moveAction.Enable();
        cam.OnSwitchPosition += PositionSwiched;
    }
    private void OnDisable()
    {
        moveAction.Disable();
        cam.OnSwitchPosition -= PositionSwiched;
    }

    private void Start()
    {
        cam.transform.localPosition = camTarget.localPosition;
    }

    void PositionSwiched()
    {
        playerAtTheHelm = !playerAtTheHelm;
        if (playerAtTheHelm)
        {
            Debug.Log("Player at the helm");
        }
    }

    private void Update()
    {
        if (playerAtTheHelm)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, camTarget.position, cam.moveSpeed * Time.deltaTime);

            Vector2 direction = moveAction.ReadValue<Vector2>();

            //Розганняє корабель
            if (direction.y != 0)
            {
                if (direction.y > 0)
                {
                    currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeedForward, speedMultiplier * Time.deltaTime);
                }
                else if (direction.y < 0)
                {
                    currentSpeed = Mathf.MoveTowards(currentSpeed, -maxSpeedBack, speedMultiplier * Time.deltaTime);
                }
            }
            
            //Нахиляє важіль
            float throttleAngel = (currentSpeed / maxSpeedForward) * 60;
            throttle.localRotation = Quaternion.Euler(throttleAngel, 0, 0);

            //видає звуки
            engineNoise.volume = Mathf.Abs(currentSpeed) / maxSpeedForward;

            //Розвертає корабель
            if (direction.x != 0)
            {   //Знаходить поточну швидкість обертання
                currentRotSpeed = Mathf.MoveTowards(currentRotSpeed, maxRotSpeed * direction.x, rotSpeedMultiplier * Time.deltaTime);
                //Розвертає штурвал
                Vector3 newWheelRot = new Vector3(-90 + (currentRotSpeed / maxRotSpeed) * 45, 90, -90);
                wheel.localRotation = Quaternion.Euler(newWheelRot);
            }
        }

        //Застосовує рух на кораблі
        transform.position += (transform.forward * currentSpeed) * Time.deltaTime;
        //Застосовує обертання на кораблі
        transform.Rotate(0, currentRotSpeed * currentSpeed * Time.deltaTime, 0);
    }
}
