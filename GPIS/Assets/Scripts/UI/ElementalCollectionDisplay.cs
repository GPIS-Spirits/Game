using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElementalCollectionDisplay : MonoBehaviour
{
    public RectTransform contentRoot;
    public GameObject listItemPrefab;

    ElementalCollection collection;
    readonly List<GameObject> pool = new();

    public void SetCollection(ElementalCollection c)
    {
        if (collection != null) 
        {
            foreach (var elem in collection.List)
                elem.OnRenamed -= HandleRename;
        }

            collection = c;

        if (collection != null)
        {
            foreach (var elem in collection.List)
                elem.OnRenamed += HandleRename;
        }
        else { Debug.Log("ECD has null collection"); }

        RefreshList();
    }

    void HandleRename(Elemental e)
    {
        RefreshList();
    }

    public void RefreshList()
    {
        foreach (var go in pool)
            go.SetActive(false);

        if (collection == null)
            return;

        var list = collection.List;

        for (int i = 0; i < list.Count; i++)
        {
            GameObject row = GetRow(i);

            // Get the UI component you built
            var item = row.GetComponent<ElementalCollectionDisplayItem>();

            item.SetElement(list[i]);
        }
    }


    GameObject GetRow(int index)
    {
        if (index >= pool.Count)
        {
            var row = Instantiate(listItemPrefab, contentRoot);
            pool.Add(row);
            return row;
        }

        var existing = pool[index];
        existing.SetActive(true);
        return existing;
    }
}
