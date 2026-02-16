using UnityEngine;
using UnityEngine.UI;

public class HelthBarContriller : MonoBehaviour
{
    [SerializeField] Image bar;
    [SerializeField] BFFcontroller bos;
    float startHPnumber;

    private void Start()
    {
        if (bos != null)
        startHPnumber = bos.helthPoints;
    }
    private void Update()
    {
        if (bos != null)
        bar.fillAmount = bos.helthPoints / startHPnumber;
    }
}
