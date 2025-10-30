using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    public static ComboManager instance;

    public TMP_Text comboTxt;
    public Color[] lvlColors;
    public Image comboBar;

    public int comboCount = 0;
    public float comboTimeout = 3f; //how long it lasts before decay
    public float decay = 0.25f; //how fast decreases
    private float comboTimer;
    private bool isDecaying = false;
    public int comboincrease = 1;

    public event Action<int> OnComboChanged;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (comboCount > 0 && isDecaying)
        {
            comboCount -= Mathf.CeilToInt(decay * Time.deltaTime);
            comboCount = Mathf.Max(0, comboCount); //oops it went into negative
            UpdateUI();
            OnComboChanged?.Invoke(comboCount);
            if(comboCount == 0)
            {
                isDecaying = false;
            }
        }
        if (comboCount > 0 && !isDecaying)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                isDecaying = true;
            }
        }
        if (comboCount > 0)
        {
            comboBar.fillAmount = comboTimer / comboTimeout;
        }
        else
        {
            comboBar.fillAmount = 0f;
        }
     }

    public void AddCombo()
    {
        comboCount += comboincrease;
        comboTimer = comboTimeout;
        isDecaying = false;
        UpdateUI();
        OnComboChanged?.Invoke(comboCount);

    }


    public void Reset()
    {
        comboCount = 0;
        isDecaying = false;
        comboTimer = comboTimeout;
    }

    public int GetComboLevel()
    {
        if (comboCount >= 50) return 6;
        if (comboCount >= 40) return 5;
        if (comboCount >= 30) return 4;
        if (comboCount >= 20) return 3;
        if (comboCount >= 10) return 2;
        if (comboCount >= 1) return 1;
        return 0;  // i could probably math this

    }

    public void UpdateUI()
    {
        comboTxt.text = "COMBO: " + comboCount.ToString();
    }
}
