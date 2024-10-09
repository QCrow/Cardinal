using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public void OnClick()
    {
        Board.Instance.RestoreFromSnapshot();
    }
}