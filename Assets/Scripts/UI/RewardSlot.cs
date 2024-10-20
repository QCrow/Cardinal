using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] private int _rewardID;
    [SerializeField] private TMPro.TMP_Text _rewardName;
    [SerializeField] private TMPro.TMP_Text _rewardText;

    public void Awake()
    {
        Button button = GetComponent<Button>();
        Debug.Assert(button != null, "Button component not found on RewardSlot object.");

        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    public void SetReward(int rewardID, string rewardName, string rewardText)
    {
        _rewardID = rewardID;
        _rewardName.text = rewardName;
        _rewardText.text = rewardText;
    }

    public void SetReward(Reward reward)
    {
        SetReward(reward.RewardID, reward.RewardName, reward.RewardText);
    }

    public void OnClick()
    {
        CardManager.Instance.AddCard(_rewardID);
        GameManager.Instance.ChangeGameState(GameState.Map);
    }
}