using UnityEngine;
using UnityEngine.UI;

public class HealthBarObj : MonoBehaviour
{
    public EntityBaseClass target;
    private Slider healthSlider;
    [SerializeField] private Image bg;
    [SerializeField] private Image fill;

    private float curFadeOutDelay;
    public float maxFadeOutDelay;
    public bool shouldFade = false;

    public void SetUp(Color fillColor)
    {
        healthSlider = GetComponent<Slider>();
        healthSlider.value = 1;
        fill.color = fillColor;
        curFadeOutDelay = maxFadeOutDelay;
        shouldFade = false;

        Color bgAlpha = bg.color;
        bgAlpha.a = 0f;
        bg.color = bgAlpha;
        bg.enabled = false;

        Color fillAlpha = fill.color;
        fillAlpha.a = 0f;
        fill.color = fillAlpha;
        fill.enabled = false;       
    }

    public void UpdateHealth(float curHealth)
    {
        Color bgAlpha = bg.color;
        bgAlpha.a = 1f;
        bg.color = bgAlpha;
        bg.enabled = true;


        Color fillAlpha = fill.color;
        fillAlpha.a = 1f;
        fill.color = fillAlpha;
        fill.enabled = true;

        float adjustedHealth = curHealth / target.GetMaxHealth();
        healthSlider.value = adjustedHealth;
        curFadeOutDelay = maxFadeOutDelay;
        shouldFade = true;
    }

    public void HandleUIVisibility()
    {
        if (shouldFade == false) return;
        if (bg.color.a <= 0 && fill.color.a <= 0) return;

        curFadeOutDelay -= Time.deltaTime;

        if(curFadeOutDelay <= 0)
        {
            if (bg.color.a > .001f && fill.color.a > .001f)
            {
                Color tempBGAlpha = bg.color;
                tempBGAlpha.a = Mathf.Lerp(tempBGAlpha.a, 0f,
                    Time.deltaTime * 4.5f);
                bg.color = tempBGAlpha;

                Color tempFillAlpha = fill.color;
                tempFillAlpha.a = Mathf.Lerp(tempFillAlpha.a, 0f,
                    Time.deltaTime * 4.5f);
                fill.color = tempFillAlpha;
            }
            else
            {
                shouldFade = false;
                curFadeOutDelay = maxFadeOutDelay;
            }
        }
    }
}
