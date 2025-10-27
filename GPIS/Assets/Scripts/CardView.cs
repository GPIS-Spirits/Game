using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text attack;
    [SerializeField] private TMP_Text description;

    [SerializeField] private TMP_Text cost;

    [SerializeField] private SpriteRenderer imageSR;

    [SerializeField] private GameObject wrapp;
}
