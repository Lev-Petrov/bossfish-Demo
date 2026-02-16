using UnityEngine;

public class ShipBuoyancy : MonoBehaviour
{
    public Vector3 rotationOffset;

    [Header ("Buoyancy settings")]
    [SerializeField] Transform[] floatingPoints = new Transform[3];
    Vector3[] virtualPoints = new Vector3[3];

    [SerializeField] float floheightAboveWater;
    [SerializeField] float buoyancy = 1f;

    void Update()
    {
        //Тримає точки плавучості на воді
        for (int i = 0; i < 3; i++)
        {
            virtualPoints[i] = floatingPoints[i].position;
            virtualPoints[i].y = WaterSistem.SampleWaterHeight(virtualPoints[i]);
        }

        //Тримає корабель на воді
        Vector3 shipTargetPos = transform.position;
        shipTargetPos.y = WaterSistem.SampleWaterHeight(shipTargetPos);
        transform.position = Vector3.Lerp(transform.position, shipTargetPos + Vector3.up / 2, buoyancy * Time.deltaTime);

        //Нахиляє корабель оріентуючись на вистоу точок
        Vector3 P0 = virtualPoints[1];
        Vector3 P1 = virtualPoints[2];
        Vector3 P2 = virtualPoints[0];

        Vector3 right = (P1 - P0).normalized;
        Vector3 dirToP2 = (P2 - P0).normalized;
        Vector3 forward = Vector3.Cross(right, dirToP2).normalized;
        Vector3 up = Vector3.Cross(forward, right);

        Quaternion rotation = Quaternion.LookRotation(forward, up);
        Quaternion correction = Quaternion.Euler(rotationOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation * correction, buoyancy * Time.deltaTime);
    }
}
