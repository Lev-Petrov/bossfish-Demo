using System.Collections;
using UnityEngine;

public class MazzleFlashController : MonoBehaviour
{
    [SerializeField] float flashingTime = 0.1f;
    [SerializeField] float fadingTime = 0.2f;

    [SerializeField] Vector3 startSize = Vector3.zero;
    [SerializeField] float maxSize = 1f;

    private void OnEnable()
    {
        StartCoroutine(Flashing());
    }

    IEnumerator Flashing()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flashingTime;
            transform.localScale = Vector3.Lerp(startSize, Vector3.one * maxSize, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadingTime;
            transform.localScale = Vector3.Lerp(Vector3.one * maxSize, Vector3.zero, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
