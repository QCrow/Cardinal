using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private Image _cardBackground;
    public Color BackgroundColor { get => _cardBackground.color; set => _cardBackground.color = value; }

    [SerializeField] private Image _cardIcon;
    public Sprite CardIcon { get => _cardIcon.sprite; set => _cardIcon.sprite = value; }

    [SerializeField] private TMP_Text _cardNameText;
    public string CardName { get => _cardNameText.text; set => _cardNameText.text = value; }

    // [SerializeField] private GameObject _cardDescriptionContainer;
    // [SerializeField] private GameObject _cardDescriptionComponentPrefab;

    public Card Card;
    private Vector3 rotationDelta;
    private Vector3 movementDelta;

    [SerializeField] private float _followSpeed = 30;
    [SerializeField] private float rotationAmount = 40;
    [SerializeField] private float rotationSpeed = 20;

    [SerializeField] private float _descriptionExpandWidth = 360;
    private Vector2 _originalCardSize;
    private Vector2 _originalVisualSize;
    [SerializeField] private GameObject _descriptionContainer;

    private void Update()
    {
        SmoothFollow();

        FollowRotation();
    }
    private void SmoothFollow()
    {
        transform.position = Vector3.Lerp(transform.position, Card.transform.position, _followSpeed * Time.deltaTime);
    }

    private void FollowRotation()
    {
        if (Card.CurrentState is not CardDraggedState) return;
        // Calculate movement difference between the card and its visual
        Vector3 movement = transform.position - Card.transform.position;

        // Smoothly interpolate the movement difference (Lerp for smooth transition)
        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
        Vector3 movementRotation = (Card.GetComponent<Card>().CurrentState is CardDraggedState ? movementDelta : movement) * rotationAmount;

        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);

        // Apply the calculated rotation to the transform, keeping Z rotation unaffected
        transform.eulerAngles = new Vector3(
            Mathf.Clamp(-rotationDelta.y, -60, 60), // Clamp X-axis rotation between -60 and 60
            Mathf.Clamp(rotationDelta.x, -60, 60), // Clamp Y-axis rotation between -60 and 60
            transform.eulerAngles.z // Leave Z rotation unchanged
        );
    }

    public void Initialize(int cardID, string cardName)
    {
        CardName = cardName;

        Sprite icon = Resources.Load<Sprite>($"Sprites/Card/{cardID}");

        if (icon == null) Debug.LogError($"Icon not found for card ID {cardID}");
        else CardIcon = icon;

        // BackgroundColor = ColorUtil.GetColorFromBuildingColor(buildingColor);
        BackgroundColor = new Color(0.949f, 0.506f, 0.306f); // TODO: This will eventually be changed to match either the trait or other argument
        // SetCardDescription("Placeholder");
    }

    // private void SetCardDescription(string cardDescription)
    // {
    //     _cardDescriptionContainer.SetActive(true);
    //     TMP_Text cardDescriptionText = Instantiate(_cardDescriptionComponentPrefab, _cardDescriptionContainer.transform).GetComponentInChildren<TMP_Text>();
    //     cardDescriptionText.text = cardDescription;
    // }

    public void OnHoverEnter()
    {
        RectTransform cardRectTransform = Card.transform.parent.GetComponent<RectTransform>();
        RectTransform visualRectTransform = transform.GetChild(0).GetComponent<RectTransform>();

        // Store the original sizes
        _originalCardSize = cardRectTransform.sizeDelta;
        _originalVisualSize = visualRectTransform.sizeDelta;

        Vector2 targetSize = new Vector2(_descriptionExpandWidth, cardRectTransform.sizeDelta.y);

        cardRectTransform.DOSizeDelta(targetSize, 0.2f);

        targetSize = new Vector2(_descriptionExpandWidth, visualRectTransform.sizeDelta.y);
        visualRectTransform.DOSizeDelta(targetSize, 0.2f).OnStart(() =>
        {
            _descriptionContainer.SetActive(true);
        });
    }



    public void OnHoverExit()
    {
        RectTransform visualRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        visualRectTransform.DOSizeDelta(_originalVisualSize, 0.2f)
            .OnComplete(() =>
            {
                _descriptionContainer.SetActive(false);
            });

        RectTransform cardRectTransform = Card.transform.parent.GetComponent<RectTransform>();
        cardRectTransform.DOSizeDelta(_originalCardSize, 0.2f);

    }

    // ! Version with shake and overshoot
    // public void OnHoverEnter()
    // {
    //     // Quick overshoot on scaling for a punchy feel
    //     transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.1f)
    //         .SetEase(Ease.OutBack)
    //         .OnComplete(() =>
    //             transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f)
    //         );

    //     // Adding a slight shake effect for position and rotation
    //     transform.DOShakePosition(0.1f, new Vector3(10f, 10f, 0), 10, 90, false, true)
    //         .SetEase(Ease.InOutSine);

    //     transform.DOShakeRotation(0.1f, new Vector3(0, 0, 15f), 10, 90)
    //         .SetEase(Ease.InOutSine);
    // }
}