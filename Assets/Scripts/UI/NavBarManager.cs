using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NavBarManager : MonoBehaviour
{
    // Singleton instance
    public static NavBarManager Instance { get; private set; }

    [Header("Map References")]
    [SerializeField] private GameObject mapCanvas; // Map Canvas reference
    private RectTransform mapRect; // RectTransform of the map canvas

    private bool mapOpen = false; // Tracks whether the map is open

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        // Initialize map references
        if (mapCanvas != null)
        {
            mapRect = mapCanvas.GetComponent<RectTransform>();
        }
    }

    // Moves the map canvas off-screen initially
    public void MoveMapOffScreen()
    {
        if (mapRect != null)
        {
            mapRect.anchoredPosition = new Vector2(Screen.width, 0); // Start off-screen to the right
        }
    }

    public void MoveMapOnScreen()
    {
        if (mapRect != null)
        {
            mapRect.anchoredPosition = new Vector2(0, 0); // Start off-screen to the right
        }
    }

    // Opens the map by sliding it into view
    public void OpenMap()
    {
        if (mapRect == null) return;

        // Tween the map into position
        mapRect.DOAnchorPosX(0, 0.5f)
               .SetEase(Ease.OutQuad);
        mapOpen = true;
    }

    // Closes the map by sliding it off-screen
    public void CloseMap()
    {
        if (mapRect == null) return;

        // Tween the map out of view
        mapRect.DOAnchorPosX(Screen.width, 0.5f)
               .SetEase(Ease.InQuad);
        mapOpen = false;
    }

    // Toggles the map's open/close state
    public void ToggleMap()
    {
        // Prevent toggling if the game is already in the Map state
        if (GameManager.Instance.CurrentGameState == GameState.Map) return;

        if (!mapOpen)
        {
            OpenMap();
        }
        else
        {
            CloseMap();
        }
    }
}
