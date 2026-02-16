using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Ram")]
public class Ram : AttackSO
{
    [SerializeField] float attackSpeed;
    [SerializeField] float attackTime;
    [SerializeField] float idleTime;

    Vector3 direction;
    bool onAttack;

    float i;
    public override void ExecuteAttack(BFFcontroller controller)
    {
        controller.speed = attackSpeed;

        if (onAttack)
        {
            if (i < attackTime)
            {
                i += Time.deltaTime;
                direction = (controller.player.position - controller.head.position).normalized;
            }
            else
            {
                i = 0;
                onAttack = false;
                Debug.Log("Attack has ended");
            }
        }
        else
        {
            if (i < idleTime)
            {
                i += Time.deltaTime;
            }
            else
            {
                i = 0;
                onAttack = true;
                Debug.Log("Attack has started");
            }
        }

        controller.direction = direction;

        for (int i = 0; i< controller.targets.Length; i++)
        {
            if (Vector3.Distance(controller.head.position, controller.targets[i].position) < 3.5f)
            {
                controller.OnGameOver?.Invoke();
                Debug.Log("Game over");
            }
        }

        if (Vector3.Distance(controller.head.position, controller.player.position) < 50 && onAttack)
        {
            controller.openJaw = true;
        }
        else controller.openJaw = false;

    }
}