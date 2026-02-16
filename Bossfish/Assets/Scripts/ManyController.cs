using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManyController : MonoBehaviour
{
    [SerializeField] Image panel;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float flashSpeed = 2f; // Швидкість миготіння
    public InputAction startGameAction;

    bool panelIsFlashing;

    Color panelColor;
    Color textColor;

    private void OnEnable()
    {
        startGameAction.Enable();
    }
    private void OnDisable()
    {
        startGameAction.Disable();
    }

    private void Start()
    {
        panelColor = panel.color;
        panelColor.a = 1;

        textColor = text.color;
    }

    private void Update()
    {
        panel.color = panelColor;
        // Зникнення панелі
        if (!panelIsFlashing)
        {
            panelColor.a = Mathf.Lerp(panelColor.a, 0, Time.deltaTime);
        }

        // Миготіння тексту
        float alpha = Mathf.PingPong(Time.time * flashSpeed, 0.5f); // коливається між 0 і 1
        text.color = new Color(textColor.r, textColor.g, textColor.b, alpha);

        if (startGameAction.IsPressed() && alpha < 0.5f)
        {
            StartCoroutine(SwichScene());
        }
    }
    IEnumerator SwichScene() 
    {
        panelIsFlashing = true;
        for (float i = 0; i < 2; i += Time.deltaTime) 
        {
            panelColor.a = Mathf.Lerp(panelColor.a, 1, 5 * Time.deltaTime);
            yield return null;
        }

        SceneManager.LoadScene("MainScene");
    }
}
