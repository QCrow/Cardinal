using UnityEngine;
using Sirenix.OdinInspector;

public class ChildrenRenamer : MonoBehaviour
{
    [SerializeField, Tooltip("The base name used for the children.")]
    private string nameVar = "namevar";

    [Button("Rename Children")]
    private void RenameChildren()
    {
        int childCount = transform.childCount;

        if (childCount != 9)  // 3x3 grid (0-2 for rows and cols)
        {
            Debug.LogWarning("There should be exactly 9 children (3x3 grid) to rename.");
            return;
        }

        int index = 0;
        for (int row = 0; row <= 2; row++)
        {
            for (int col = 0; col <= 2; col++)
            {
                Transform child = transform.GetChild(index);
                child.name = $"{nameVar} {row} {col}";
                index++;
            }
        }

        Debug.Log("Children renamed successfully!");
    }
}
