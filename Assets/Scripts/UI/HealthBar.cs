using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMPro.TMP_Text _healthText;
    private float _fillAmount;

    public void SetHealth(float currHealth, float maxHealth)
    {
        _fillAmount = currHealth / maxHealth;

        _slider.value = _fillAmount;
        _healthText.text = $"{currHealth}/{maxHealth}";
    }
}