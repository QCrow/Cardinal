using UnityEngine;
using Sirenix.OdinInspector;

public class ArrowPlacer : MonoBehaviour
{
    [SerializeField, Tooltip("The prefab to use for the arrows.")]
    private GameObject arrowPrefab;

    [SerializeField, Tooltip("Padding between the arrows and the matrix.")]
    private float padding = 10f;

    [SerializeField, Tooltip("The size of each matrix cell.")]
    private float cellSize = 233f;

    [SerializeField, Tooltip("Spacing between each cell.")]
    private float cellSpacing = 10f;

    [Button("Place Arrows Automatically")]
    private void PlaceArrows()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow prefab is not assigned!");
            return;
        }

        // Calculate total matrix size (3 cells + 2 spacings between cells)
        float matrixSize = (cellSize * 3) + (cellSpacing * 2);
        float halfMatrix = matrixSize / 2;

        // Define the positions for the arrows around the matrix
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-halfMatrix - padding, cellSize + cellSpacing / 2, 0), // Left-Top
            new Vector3(-halfMatrix - padding, 0, 0),                         // Left-Middle
            new Vector3(-halfMatrix - padding, -cellSize - cellSpacing / 2, 0), // Left-Bottom

            new Vector3(halfMatrix + padding, cellSize + cellSpacing / 2, 0), // Right-Top
            new Vector3(halfMatrix + padding, 0, 0),                         // Right-Middle
            new Vector3(halfMatrix + padding, -cellSize - cellSpacing / 2, 0), // Right-Bottom

            new Vector3(0, halfMatrix + padding, 0),                          // Top-Middle
            new Vector3(-cellSize - cellSpacing / 2, halfMatrix + padding, 0), // Top-Left
            new Vector3(cellSize + cellSpacing / 2, halfMatrix + padding, 0),  // Top-Right

            new Vector3(0, -halfMatrix - padding, 0),                          // Bottom-Middle
            new Vector3(-cellSize - cellSpacing / 2, -halfMatrix - padding, 0), // Bottom-Left
            new Vector3(cellSize + cellSpacing / 2, -halfMatrix - padding, 0)   // Bottom-Right
        };

        // Define corresponding rotations for the arrows (Z axis in degrees)
        float[] rotations = new float[]
        {
            0, 0, 0,       // Left side arrows
            180, 180, 180, // Right side arrows
            90, 90, 90,    // Top side arrows
            -90, -90, -90  // Bottom side arrows
        };

        // Define corresponding directions for each arrow
        Direction[] directions = new Direction[]
        {
            Direction.Left, Direction.Left, Direction.Left,
            Direction.Right, Direction.Right, Direction.Right,
            Direction.Up, Direction.Up, Direction.Up,
            Direction.Down, Direction.Down, Direction.Down
        };

        // Clear any existing arrows
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Place arrows with appropriate positions, rotations, and configure ArrowButton component
        for (int i = 0; i < 12; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform);
            arrow.transform.localPosition = positions[i];
            arrow.transform.localRotation = Quaternion.Euler(0, 0, rotations[i]);
            arrow.transform.localScale = Vector3.one;

            // Configure ArrowButton component
            ArrowButton arrowButton = arrow.GetComponent<ArrowButton>();
            if (arrowButton != null)
            {
                arrowButton._direction = directions[i];

                // Determine index (0 to 2) based on the arrow's row/column placement
                arrowButton._index = i % 3;

                // Set magnitude: -1 for Up/Left, 1 for Down/Right
                arrowButton._magnitude = (arrowButton._direction == Direction.Up ||
                                          arrowButton._direction == Direction.Left) ? -1 : 1;
            }
        }

        Debug.Log("Arrows placed and configured successfully!");
    }
}
