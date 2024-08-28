using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private Image _cardBackground;
    public Color BackgroundColor { get => _cardBackground.color; set => _cardBackground.color = value; }

    [SerializeField] private Image _cardIcon;
    public Sprite CardIcon { get => _cardIcon.sprite; set => _cardIcon.sprite = value; }

    [SerializeField] private TMP_Text _cardNameText;
    public string CardName { get => _cardNameText.text; set => _cardNameText.text = value; }

    [SerializeField] private GameObject _cardDescriptionContainer;
    [SerializeField] private GameObject _cardDescriptionComponentPrefab;

    public void Initialize(int cardID, string cardName, string cardDescription, ColorType buildingColor)
    {
        CardName = cardName;

        Sprite icon = Resources.Load<Sprite>($"Sprites/Card/{cardID}");

        if (icon == null) Debug.LogError($"Icon not found for card ID {cardID}");
        else CardIcon = icon;

        BackgroundColor = ColorUtil.GetColorFromBuildingColor(buildingColor);
        SetCardDescription(cardDescription);
    }

    private void SetCardDescription(string cardDescription)
    {
        _cardDescriptionContainer.SetActive(true);
        TMP_Text cardDescriptionText = Instantiate(_cardDescriptionComponentPrefab, _cardDescriptionContainer.transform).GetComponentInChildren<TMP_Text>();
        cardDescriptionText.text = cardDescription;
    }

    private void SetCardDescription(List<BuildingCardEffect> effects)
    {
        //TODO: To be implemented
    }
}

