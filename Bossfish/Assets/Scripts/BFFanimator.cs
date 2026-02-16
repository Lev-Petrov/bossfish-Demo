using UnityEngine;
using System.Collections.Generic;
public class BFFanimator : MonoBehaviour
{
    [SerializeField] BFFcontroller controller;
    [SerializeField] Transform jaw;

    [System.Serializable]
    public class FinSettings
    {
        public Transform fin;
        public float minY = -20f;
        public float maxY = 40f;
        public float speed = 1f;
        public float phaseOffset = 0f; // Додатковий зсув часу для асинхронного руху
    }

    [SerializeField] List<FinSettings> fins = new List<FinSettings>();

    void Update()
    {
        //Махання плавників
        foreach (FinSettings finSettings in fins)
        {
            if (finSettings.fin == null) continue;

            // Плавне значення від 0 до 1, з фазовим зсувом
            float t = (Mathf.Sin(Time.time * finSettings.speed + finSettings.phaseOffset) + 1f) / 2f;

            // Інтерполяція кута
            float yRotation = Mathf.Lerp(finSettings.minY, finSettings.maxY, t);

            // Отримуємо поточний оберт і оновлюємо лише Y
            Vector3 currentEuler = finSettings.fin.localEulerAngles;
            finSettings.fin.localRotation = Quaternion.Euler(currentEuler.x, yRotation, currentEuler.z);
        }

        //Відкриває щелепу
        float targetJawRotX = 0;

        if (controller.openJaw)
        {
            targetJawRotX = 30;
        }

        Vector3 newJawRot = jaw.localRotation.eulerAngles;
        newJawRot.x = Mathf.Lerp(newJawRot.x, targetJawRotX, 3 * Time.deltaTime);
        jaw.localRotation = Quaternion.Euler(newJawRot);
    }

    public void Death()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.y = 0;
        transform.position = targetPosition;
    }
}
