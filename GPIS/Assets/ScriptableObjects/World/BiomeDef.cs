using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBiome", menuName = "World/Biome")]
public class BiomeDef : ScriptableObject
{
    [Header("Identity")]
    public string ID;
    public string biomeName;
    [TextArea]
    public string description;

    [Header("Visuals")]
    public Color mainColor;
    public Sprite tileSprite;
    public Sprite backgroundImage;

    //different biome different music
    [Header("Audio")]
    public AudioClip ambientSound;
    public AudioClip musicTrack;

    [Header("Gameplay")]
    public float movementCost = 1f;  // this is for movement buffs or debuffs (like snow could make you slower)_
    public float encounterRate = 1f;

    [Header("Tags")]
    public List<string> tags; // this is for different biome types like toxic or fire or something
}