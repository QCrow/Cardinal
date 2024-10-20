
using UnityEngine;
public class MapNode
{
    public int Row;
    public int Col;
    public MapNodeType NodeType;

    public bool IsVisited;

    public MapNode(MapNodeType nodeType)
    {
        NodeType = nodeType;
    }

    public void OnEnter()
    {
        IsVisited = true;
        switch (NodeType)
        {
            case MapNodeType.NormalBattle:
                GameManager.Instance.ChangeGameState(GameState.Battle);
                // TODO: Move the enemy generation logic to BattleManager
                BattleManager.Instance.StartBattleAgainstEnemy("NormalEnemy");
                break;
            case MapNodeType.EliteBattle:
                GameManager.Instance.ChangeGameState(GameState.Battle);
                // TODO: Move the enemy generation logic to BattleManager
                BattleManager.Instance.StartBattleAgainstEnemy("EliteEnemy");
                break;
            case MapNodeType.Shop:
                GameManager.Instance.ChangeGameState(GameState.Shop);
                ShopManager.Instance.InitializeShop();
                break;
            case MapNodeType.RestSite:
                Debug.Log("Rest site entered");
                break;
            case MapNodeType.Treasure:
                Debug.Log("Treasure node entered");
                break;
            case MapNodeType.Mystery:
                Debug.Log("Mystery node entered");
                break;
        }
    }
}