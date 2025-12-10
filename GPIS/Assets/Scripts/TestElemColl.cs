using UnityEngine;

public class TestElemColl : MonoBehaviour
{
    public ElementalCollectionDisplay listView;

    private ElementalCollection collection;

    private void Awake()
    {
        PlayerElementals pe = FindObjectOfType<PlayerElementals>();
        if (pe != null)
        {
            collection = pe.collection;
        }
        else
        {
            Debug.LogError("TestElemColl: No PlayerElementals found in scene.");
        }
        if (listView != null)
            listView.SetCollection(collection);
    }
}
