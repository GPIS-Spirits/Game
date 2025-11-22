using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening; // 👈 Needed for DOMove / DORotate

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardView> cards = new();

    public IEnumerator AddCard(CardView cardView)
    {
        cards.Add(cardView);
        yield return UpdateCardPositions(0.15f);
    }

    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0) yield break;

        float cardSpacing = 1f / 10f;
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2f;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < cards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;

            // Evaluate spline in 3D (Unity splines are 3D by default)
            Vector3 pos3D = spline.EvaluatePosition(p);
            Vector3 tan3D = spline.EvaluateTangent(p);

            // Convert to 2D
            Vector2 pos = new Vector2(pos3D.x, pos3D.y);
            Vector2 tangent = new Vector2(tan3D.x, tan3D.y).normalized;

            // Compute 2D rotation angle (in degrees)
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

            // Tween to new position and rotation
            cards[i].transform.DOMove(
                new Vector3(pos.x, pos.y, 0) + transform.position + new Vector3(0, 0, -0.01f * i),
                duration
            );

            cards[i].transform.DORotate(
                new Vector3(0, 0, angle),
                duration
            );
        }

        yield return new WaitForSeconds(duration);
    }
}
