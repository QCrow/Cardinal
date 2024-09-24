using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private float _fillAmount;
    [SerializeField] private TMPro.TMP_Text _healthText;

    public void SetHealth(float health, float maxHealth)
    {
        _fillAmount = health / maxHealth;
        _slider.value = _fillAmount;
        _healthText.text = $"{health}/{maxHealth}";
    }
}