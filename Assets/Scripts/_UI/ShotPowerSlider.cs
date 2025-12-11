using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShotPowerSlider : MonoBehaviour
{
    public Slider slider;
    public MonoBehaviour receiverComponent;  // будь-який компонент (лаунчер)
    public TextMeshProUGUI valueText;                   
    private IShotPowerReceiver receiver;

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        slider.minValue = 0f;
        slider.maxValue = 1f;

        if (receiverComponent != null)
        {
            receiver = receiverComponent as IShotPowerReceiver;

            if (receiver == null)
            {
                Debug.LogError(
                    $"ShotPowerSlider: {receiverComponent.name} не імплементує IShotPowerReceiver"
                );
            }
        }

        slider.onValueChanged.AddListener(OnSliderChanged);

        // 👈 одразу оновлюємо текст при старті
        UpdateText(slider.value);
    }

    void OnSliderChanged(float value)
    {
        if (receiver != null)
        {
            receiver.SetShotPower(value);
        }

        UpdateText(value);
    }

    void UpdateText(float value)
    {
        if (valueText != null)
        {
            int scaled = Mathf.RoundToInt(value * 100f); 
            valueText.text = scaled.ToString();
        }
    }
}