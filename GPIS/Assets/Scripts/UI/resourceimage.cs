using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class resourceimage : MonoBehaviour
{
    public int currentImage;

    public Image imageUI;

    public Sprite[] elements;

    // Start is called before the first frame update
    void Start()
    {
        UpdateImage();
    }

    // Update is called once per frame
    public void UpdateImage()
    {
        imageUI.sprite = elements[currentImage];
    }
}
