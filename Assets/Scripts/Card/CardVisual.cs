using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public GameObject Card;
    private Vector3 rotationDelta;
    private Vector3 movementDelta;

    [SerializeField] private float _followSpeed = 30;
    [SerializeField] private float rotationAmount = 40;
    [SerializeField] private float rotationSpeed = 20;
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

}