using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
    [SerializeField] private Image panel;
    [SerializeField] private Transform player;
    [SerializeField] private Transform boss;
    [SerializeField] private Animator shipAnimator;
    [SerializeField] private AudioSource gameOverSourse;
    [SerializeField] private RectTransform[] hints;
    [SerializeField] private InputAction[] actions;
    [SerializeField] InputAction ExitAction;
    [SerializeField] InputAction ResetData;

    private Color panelColor;
    private RectTransform lastMassage;

    [Header ("Settings")]
    [SerializeField] Volume volume;
    [SerializeField] VolumeProfile profileRTC;
    [SerializeField] VolumeProfile profileBGS;

    private void OnEnable()
    {
        foreach (var action in actions)
            action.Enable();

        ExitAction.Enable();
        ResetData.Enable();
    }

    private void OnDisable()
    {
        foreach (var action in actions)
            action.Disable();
    }

    private void Start()
    {
        StartCoroutine(GameStarter());

        lastMassage = hints[hints.Length -1];
        lastMassage.position += Vector3.left * 300f;
    }

    private void Update()
    {
        if (ExitAction.IsPressed())
        {
            Application.Quit();
        }

        if (ResetData.IsPressed())
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Rese Data");
        }
    }

    private IEnumerator GameStarter()
    {
        //Застосовує налаштування
        if (PlayerPrefs.GetString("volumetricCloudsIsOn") == "true")
        {
            volume.profile = profileRTC;
        }
        else { volume.profile = profileBGS; }

        //Розвертає гравця у випадкову напрямку
        Vector3 newPlayerRot = Vector3.zero;
        newPlayerRot.y = Random.Range(0, 360);
        player.rotation = Quaternion.Euler(newPlayerRot);

        // Зникнення панелі
        panelColor = panel.color;
        float fadeDuration = 1.5f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            panelColor.a = alpha;
            panel.color = panelColor;
            yield return null;
        }

        panelColor.a = 0f;
        panel.color = panelColor;

        // Туторіал
        if (PlayerPrefs.GetString("isFirstPlay", "true") == "true")
        {
            for (int i = 0; i < hints.Length -1; i++)
            {
                RectTransform hint = hints[i];
                InputAction action = actions[i];

                Vector3 originalPos = hint.position;
                Vector3 hiddenPos = originalPos + Vector3.left * 300f;
                Vector3 hideOutPos = originalPos + Vector3.left * 300f;

                hint.position = hiddenPos;
                hint.gameObject.SetActive(true);

                // Поява
                yield return MoveHint(hint, hiddenPos, originalPos, 0.7f);

                // Чекаємо натискання
                while (!action.IsPressed())
                    yield return null;

                // Зникнення
                yield return MoveHint(hint, originalPos, hideOutPos, 0.7f);
                hint.gameObject.SetActive(false);
            }
            Debug.Log("Усі підсказки завершено!");
            PlayerPrefs.SetString("isFirstPlay", "false");
        }

        //Поява боса
        boss.gameObject.SetActive(true);
        Vector3 newBossPos = player.position + Random.onUnitSphere * 250;
        newBossPos.y = -10;
        boss.position = newBossPos;
        Debug.Log("Boss appeared");

        shipAnimator.SetTrigger("Open monitors");
    }

    private IEnumerator MoveHint(RectTransform hint, Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            hint.position = Vector3.Lerp(from, to, t / duration);
            yield return null;
        }
        hint.position = to;
    }

    public void GameOver()
    {
        panel.color = Color.black;
        Camera.main.transform.position = new Vector3(0, 0, -100);
        gameOverSourse.Play();
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("MainScene");
    }

    public void LastMaseg()
    {
        StartCoroutine(ShowLastMaseg());
    }

    IEnumerator ShowLastMaseg()
    {
        lastMassage.gameObject.SetActive(true);
        yield return MoveHint(lastMassage, lastMassage.position, lastMassage.position + Vector3.right * 300, 0.7f);
        yield return new WaitForSeconds(2);
        yield return MoveHint(lastMassage, lastMassage.position, lastMassage.position + Vector3.left * 300, 0.7f);
    }
}
