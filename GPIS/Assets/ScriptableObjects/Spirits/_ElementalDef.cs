using UnityEngine;

[CreateAssetMenu(menuName = "_SO/Elementals/Elemental")]
public class ElementalDef : BaseNPCDef
{
    [Header("Elemental Attributes")]
    public Element elementType = Element.Fire;
    public Quality quality = Quality.Common;

    [Header("Stats")]
    public float dmgMult = 1;
    public float dmgFlat = 0;
    public float defMult = 1;
    public float defFlat = 0;
}
