using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMPro.TMP_Text _currentHealthText;
    [SerializeField] private TMPro.TMP_Text _maxHealthText;
    private float _fillAmount;

    public void SetHealth(float currHealth, float maxHealth)
    {
        _fillAmount = currHealth / maxHealth;

        _slider.value = _fillAmount;
        _currentHealthText.text = $"{currHealth}";
        _maxHealthText.text = $"{maxHealth}";
    }
}