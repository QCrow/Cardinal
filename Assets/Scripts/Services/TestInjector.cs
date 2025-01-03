using UnityEngine;

public class TestInjector : MonoBehaviour
{
    [SerializeField] private EnemyScriptable _enemyScriptable;
    private void Start()
    {
        BattleManager.Instance.StartBattleAgainstEnemy(_enemyScriptable);
    }
}