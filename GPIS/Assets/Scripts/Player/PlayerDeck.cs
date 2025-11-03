using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    [Header("Player Deck")]
    public List<PlayableCardDef> startingDeck = new(); // List of Elemental SOs in the Player's deck.

    private readonly List<PlayableCardDef> library = new(); // Draw Pile ("Library")
    private readonly List<PlayableCardDef> discard = new(); // Discard Pile

    void Awake()
    {
        BuildAndShuffle();
    }

    public void BuildAndShuffle()
    {
        library.Clear();
        discard.Clear();

        library.AddRange(startingDeck); // Add the entire starting deck to the library.
        Shuffle(library);               // Shuffle the library before drawing.
    }

    public void Discard(PlayableCardDef def)
    {
        if (def != null) discard.Add(def);
    }

    public int TryDraw(int count, List<PlayableCardDef> outCards)
    {
        int drawn = 0;
        outCards.Clear();

        while (drawn < count)
        {
            if (library.Count == 0)
            {
                if (discard.Count == 0) break; // There are no cards left to draw, so break:

                library.AddRange(discard);
                discard.Clear();
                Shuffle(library);
            }

            int id = library.Count - 1;
            var def = library[id];
            library.RemoveAt(id);
            outCards.Add(def);
            drawn++;
        }
        return drawn;
    }

    private void Shuffle(List<PlayableCardDef> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
