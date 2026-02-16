
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BFFcontroller : MonoBehaviour
{
    public float helthPoints;
    [SerializeField] AudioSource MusicSource;
    public UnityEvent OnDeath;
    Coroutine deathCorutine;

    [Header("Move setings")]
    public Quaternion fixRotation = Quaternion.Euler(0, 0, 0);
    public Transform[] segments;
    public float elasticity;
    public bool canMove;
    public bool underWater;
    public float divingDepth;

    [HideInInspector] public Vector3 direction;
    [HideInInspector] public Transform head;

    [Header("Attack setings")]
    [SerializeField] AttackSO[] attacks;
    public Transform[] targets;
    public Transform player;
    public float speed;
    public float rotSpeed;
    public bool openJaw;

    bool finelPhaseStarted;

    public UnityEvent OnGameOver;

    private float[] segmentDistances;

    private void Start()
    {
        // Перевірка
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] == null)
            {
                Debug.LogError($"Segment[{i}] = null");
                enabled = false;
                return;
            }
        }

        // Перевертаю масив
        System.Array.Reverse(segments);
        head = segments[segments.Length - 1];

        // Зберігаю початкові відстані
        segmentDistances = new float[segments.Length - 1];
        for (int i = 0; i < segmentDistances.Length; i++)
        {
            segmentDistances[i] = Vector3.Distance(segments[i].position, segments[i + 1].position);
        }
    }


    private void Update()
    {
        Attack();
        if (canMove)
        {
            Move();
        }

        if (helthPoints <= 0 && deathCorutine == null)
        {
            deathCorutine = StartCoroutine(Death());
        }
    }

    //Керує атаками та перемикає фази
    private void Attack()
    { 
        if (attacks.Length != 0)
        {
            if (helthPoints > 80)
            {
                attacks[0].ExecuteAttack(this);
            }
            else if (helthPoints > 40)
            {
                Debug.Log("Phase 2");
                attacks[1].ExecuteAttack(this);
            }
            else if (helthPoints > 0)
            {
                if (!finelPhaseStarted)
                {
                    finelPhaseStarted = true;
                }
                Debug.Log("Phase 3");
                attacks[2].ExecuteAttack(this);
                elasticity = 16;
            }
        }
    }

    private void Move()
    {
        // Підводне вирівнювання
        if (underWater)
        {
            float targetHeight = WaterSistem.SampleWaterHeight(head.position) - divingDepth;
            direction.y = targetHeight - head.position.y;
            direction = direction.normalized;
        }

        // Ротація голови
        Quaternion targetRot = Quaternion.LookRotation(direction);
        head.rotation = Quaternion.Slerp(head.rotation, targetRot * fixRotation, rotSpeed * Time.deltaTime);

        // Рух вперед
        head.position += DirForwart(head) * speed * Time.deltaTime;

        // Сегменти притягуються один до одного з фіксованою відстанню
        for (int i = 0; i < segments.Length - 1; i++)
        {
            Vector3 dir = (segments[i + 1].position - segments[i].position).normalized;

            // Ротація сегменту
            Quaternion targetSegmentRot = Quaternion.LookRotation(dir) * fixRotation;
            segments[i].rotation = Quaternion.Slerp(segments[i].rotation, targetSegmentRot, rotSpeed * elasticity * Time.deltaTime);

            Vector3 targetSegmentPos = segments[i + 1].position - dir * segmentDistances[i];
            segments[i].position = Vector3.Lerp(segments[i].position, targetSegmentPos, speed * elasticity * Time.deltaTime);

        }
    }


    //Знаходить напрямок уперед враховуючи недоліки 3D моделі
    public Vector3 DirForwart(Transform transform)
    {
        return transform.up;
    }

    IEnumerator Death()
    {
        divingDepth = 10;
        for (float i = 0; i < 5; i += Time.deltaTime)
        {
            MusicSource.volume -= 0.1f * Time.deltaTime;
            yield return null;
        }
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
