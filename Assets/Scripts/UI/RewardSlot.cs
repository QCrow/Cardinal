using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] private int _rewardID;
    [SerializeField] private TMPro.TMP_Text _rewardName;
    [SerializeField] private TMPro.TMP_Text _rewardText;
    [SerializeField] private TMPro.TMP_Text _rewardAttack;
    [SerializeField] private TMPro.TMP_Text _rewardCycle;
    [SerializeField] private GameObject _cycleContainer;

    public void Awake()
    {
        Button button = GetComponent<Button>();
        Debug.Assert(button != null, "Button component not found on RewardSlot object.");
        _cycleContainer.SetActive(false);

        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    public void SetReward(int rewardID, string rewardName, string rewardText, int rewardAttack, int rewardCycle)
    {
        _rewardID = rewardID;
        _rewardName.text = rewardName;
        _rewardText.text = rewardText;
        _rewardAttack.text = rewardAttack.ToString();
        if (rewardCycle > 0)
        {
            _cycleContainer.SetActive(true);
            _rewardCycle.text = rewardCycle.ToString();
        }
    }

    public void SetReward(CardReward reward)
    {
        SetReward(reward.RewardID, reward.RewardName, reward.RewardText, reward.RewardAttack, reward.RewardCycle);
    }

    public void OnClick()
    {
        CardSystem.Instance.DeckManager.AddCard(_rewardID);
        GameManager.Instance.ChangeGameState(GameState.Map); // TODO: TO BE CHANGED
        // Map.Instance.GoNext();
    }
}