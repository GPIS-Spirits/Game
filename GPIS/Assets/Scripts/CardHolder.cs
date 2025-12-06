using UnityEngine;
using UnityEngine.UI;

namespace CardHandler
{
    public static class CardHolder
    {
        public static CardInteraction[] cards;
        public static Vector3 Vertex; // manually set

        public static float padding = 4f;
        public static float e = 0.025f; 
        public static Vector3 baseVector = Vector3.right;

        public static void HoldCards()
        {
            CalculateCardPositionsAndRotations();
        }

        public static float CalculatePoint(float x)
        {
            float b = e * x;
            return -Mathf.Pow(b, 2);
        }

        static void CalculateCardPositionsAndRotations()
        {
            int numberOfCards = cards.Length;
            if (numberOfCards == 0) return;

            // center cards around vertex
            float halfCount = (numberOfCards - 1) / 2f;

            for (int i = 0; i < numberOfCards; i++)
            {
                float x = padding * (i - halfCount);
                float y = CalculatePoint(x);

                // position along parabola
                Vector3 pos = new Vector3(x, y, 0) + Vertex;
                RectTransform rt = cards[i].GetComponent<RectTransform>();
                if (rt != null)
                    rt.position = pos;

                // rotation tangent-based (Z only)
                float delta = 0.01f;
                float y1 = CalculatePoint(x - delta);
                float y2 = CalculatePoint(x + delta);
                float dy_dx = (y2 - y1) / (2f * delta);
                float angleZ = Mathf.Atan2(dy_dx, 1f) * Mathf.Rad2Deg;

                cards[i].transform.rotation = Quaternion.Euler(0f, 0f, angleZ);
            }
        }
    }
}
