using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ScrollController : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Transform content;

    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float smoothTime = 8f;

    // تنظیمات نگه داشتن دکمه
    [SerializeField] private float firstRepeatDelay = 0.4f; // تاخیر اولیه
    [SerializeField] private float repeatRate = 0.08f;      // سرعت تکرار

    float speed;
    float targetValue=1;

    float holdTimer;
    float repeatTimer;
    float lastInput;

    void Start()
    {
        speed = baseSpeed / Mathf.Max(1, content.childCount);
        // targetValue = scrollbar.value;
    }

    void Update()
    {
        if (Gamepad.current == null) return;

        float input = GetVerticalInput();

        if (input != 0 && lastInput == 0)
        {
            ApplyScroll(input);
            holdTimer = 0;
            repeatTimer = 0;
        }
        else if (input != 0 && lastInput == input)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= firstRepeatDelay)
            {
                repeatTimer += Time.deltaTime;

                if (repeatTimer >= repeatRate)
                {
                    ApplyScroll(input);
                    repeatTimer = 0;
                }
            }
        }
        else if (input == 0)
        {
            holdTimer = 0;
            repeatTimer = 0;
        }

        lastInput = input;

        scrollbar.value = Mathf.Lerp(
            scrollbar.value,
            targetValue,
            Time.deltaTime * smoothTime
        );
    }

    float GetVerticalInput()
    {
        var pad = Gamepad.current;

        if (pad.dpad.up.isPressed) return 1;
        if (pad.dpad.down.isPressed) return -1;

        float stickY = pad.leftStick.y.ReadValue();

        if (stickY > 0.5f) return 1;
        if (stickY < -0.5f) return -1;

        return 0;
    }

    void ApplyScroll(float direction)
    {
        targetValue += direction * speed;
        targetValue = Mathf.Clamp01(targetValue);
    }
}
