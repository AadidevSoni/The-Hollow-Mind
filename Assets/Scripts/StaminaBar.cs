using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public PlayerControlling player;
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = player.GetMaxStamina();
        slider.value = player.GetCurrentStamina();
    }

    void Update()
    {
        slider.value = player.GetCurrentStamina();
    }
}
