using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SortOrder : MonoBehaviour
{
    public int order = -100;

    void Awake()
    {
        var r = GetComponent<Renderer>();

        r.material.renderQueue = 1000;

        r.sortingOrder = order;
    }
}
