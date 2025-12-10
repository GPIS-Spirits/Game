using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    [Header("Assign Icons for Each Element")]
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite earthSprite;
    public Sprite airSprite;
    public Sprite lightSprite;
    public Sprite darkSprite;

    [Header("Existing Resource UI Objects")]
    public ResourceUI[] resourceSlots;

    private Dictionary<Element, Sprite> sprites;

    private void Awake()
    {
        sprites = new Dictionary<Element, Sprite>
        {
            { Element.Fire, fireSprite },
            { Element.Water, waterSprite },
            { Element.Earth, earthSprite },
            { Element.Air, airSprite },
            { Element.Light, lightSprite },
            { Element.Dark, darkSprite }
        };
    }

    private void Start()
    {
        ApplyResources();
    }

    // ----------------------------------------------------------
    // Applies everything to the Prefabs
    // ----------------------------------------------------------
    public void ApplyResources()
    {
        foreach (var slot in resourceSlots)
        {
            if (slot == null) continue;

            Element elem = slot.element;

            if (sprites.TryGetValue(elem, out Sprite icon))
            {
                slot.Setup(elem, icon);
            }
            else
            {
                Debug.LogWarning($"No sprite assigned for Element: {elem}");
            }
        }
    }
}
