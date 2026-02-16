using UnityEngine;

public class RadarController : MonoBehaviour
{
    [SerializeField] Transform point;       // Мітка цілі на радарі
    [SerializeField] Transform player;      // Гравець
    [SerializeField] Transform target;      // Ціль
    [SerializeField] float radarRange = 50f; // Радіус радара

    private void Update()
    {
        if (target != null)
        {
            // Вектор від гравця до цілі на горизонтальній площині
            Vector3 direction = target.position - player.position;
            direction.y = 0;

            // Відстань перевищує діапазон радара — не показуємо мітку
            if (direction.magnitude > radarRange)
            {
                point.gameObject.SetActive(false);
                return;
            }

            point.gameObject.SetActive(true);

            // Нормалізована позиція цілі у межах радара
            Vector3 normalized = direction / radarRange;

            // Перетворення на 3D-вектор для повороту (XZ -> XY UI)
            Vector3 radarPos = new Vector3(normalized.x, 0, normalized.z);

            // Обчислюємо поворот гравця по осі Y
            Quaternion playerRotation = Quaternion.Euler(0, -player.eulerAngles.y, 0);

            // Обертаємо позицію мітки на радарі згідно з поворотом гравця
            Vector3 rotatedPos = playerRotation * radarPos;

            // Присвоюємо позицію мітці на радарі (XY в UI)
            point.localPosition = new Vector3(rotatedPos.x, rotatedPos.z, 0);
        }
        else { point.transform.gameObject.SetActive(false); }
        
    }
}
