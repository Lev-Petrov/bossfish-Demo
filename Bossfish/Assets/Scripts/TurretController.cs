using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretController : MonoBehaviour
{
    [SerializeField] Transform tripod;
    [SerializeField] Transform gun;
    [SerializeField] CameraController cam;
    [SerializeField] Transform camTarget;
    [SerializeField] GameObject Sight;
    [SerializeField] bool playerAtTurret;
    [SerializeField] Vector2 rotSpeed;
    [Range(180, 0)][SerializeField] float tripotRotationLimit;
    [Range(0, 180)] [SerializeField] float gunUpRotationLimit;
    [Range(-180,0)] [SerializeField] float gunDownRotationLimit;
    Vector3 currentRot;

    [Header("Fire setings")]
    public InputAction fireAction;
    [SerializeField] AudioSource shotSorce;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform bolt;
    [SerializeField] float damage;
    [SerializeField] float timeBtwShots;
    [SerializeField] float cameraShakePower;
    [SerializeField] float boltMoveAmplitude;
    Vector3 boltStartPosition;

    [SerializeField] GameObject mazzleFlash;

    private void OnEnable()
    {
        fireAction.Enable();
        cam.OnSwitchPosition += PositionSwiched;
    }
    private void OnDisable()
    {
        fireAction.Disable();
        cam.OnSwitchPosition -= PositionSwiched;
    }

    private void Start()
    {
        boltStartPosition = bolt.localPosition;
    }
    private void PositionSwiched()
    {
        playerAtTurret = !playerAtTurret;
        Sight.SetActive(playerAtTurret);

        if (playerAtTurret)
        {
            StartCoroutine(Fire());
            Debug.Log("Player at turret");
        }
    }

    private void Update()
    {
        if (playerAtTurret)
        {
            //Переміщює камеру до турелі
            cam.transform.position = Vector3.Lerp(cam.transform.position, camTarget.position, cam.moveSpeed * Time.deltaTime);
            //Знаходить напрямок обертання
            Vector3 targetRot = cam.transform.localRotation.eulerAngles;
            targetRot.y += 180;
            targetRot.x += 90;

            //Обертає штатив
            currentRot.y = Mathf.LerpAngle(currentRot.y, targetRot.y, rotSpeed.y * Time.deltaTime);
            currentRot.y = Mathf.Clamp(currentRot.y, -tripotRotationLimit, tripotRotationLimit);
            tripod.localRotation = Quaternion.Euler(0, currentRot.y, 0);

            //Обертає дуло
            currentRot.x = Mathf.LerpAngle(currentRot.x, -targetRot.x, rotSpeed.x * Time.deltaTime);
            currentRot.x = Mathf.Clamp(currentRot.x, gunDownRotationLimit, gunUpRotationLimit);
            gun.localRotation = Quaternion.Euler(currentRot.x, 0, 0);

            //Повертає штатив на місце
            bolt.localPosition = Vector3.Lerp(bolt.localPosition, boltStartPosition, 8 * timeBtwShots * Time.deltaTime);
        }
    }

    IEnumerator Fire()
    {
        Debug.Log("Torret can shot");
        while (playerAtTurret)
        {
            if (fireAction.IsPressed())
            {
                shotSorce.Play();
                Ray ray = new Ray(firePoint.position, firePoint.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        if (hit.collider.CompareTag("Enemy"))
                        {
                            BFFcontroller BFF = hit.transform.GetComponentInParent<BFFcontroller>();
                            if (BFF != null)
                            {
                                BFF.helthPoints -= damage;
                            }
                            else
                            {
                                Debug.LogWarning("Enemy object does not have BFFcontroller attached!", hit.transform);
                            }
                        }

                    }
                }

                //Трусить камеру
                cam.transform.position += -cam.transform.forward * cameraShakePower;

                bolt.localPosition += boltMoveAmplitude * Vector3.down;

                mazzleFlash.SetActive(true);
                yield return new WaitForSeconds(timeBtwShots);
            }
            else { yield return null; }
        }
        Debug.Log("Torret can`t shot");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(firePoint.position, firePoint.forward * 100);
    }
}
