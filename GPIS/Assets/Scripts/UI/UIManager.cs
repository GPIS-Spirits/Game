/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Player Stats UI")]
    public Slider hpSlider;

    public int maxHP = 100;
    public int currentHP;

     void Start()
     {
         currentHP = maxHP;

         if (hpSlider != null)
         {
             hpSlider.maxValue = maxHP;
             hpSlider.value = currentHP;
         }
     }

    public void /*TakeDamage????(int amount)
    {
        currentHP -= amount;

        //make sure not going negative 
        if (currentHP< 0)
            currentHP = 0;

        hpSlider.value = currentHP;
    }
}*/