// Accesses the "ElementalDef" of an Elemental to use for combat mechanics.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalCombat : MonoBehaviour
{
    [SerializeField] private ElementalDisplay elementalDisplay;
    public ElementalDef Def => elementalDisplay.elemental;

    void Awake()
    {
        if (!elementalDisplay) elementalDisplay = GetComponent<ElementalDisplay>();
    }
}
