using System.Collections;
using UnityEngine;

public class SpriteTransition : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Coroutine coroutine;

    public void Play(Sprite sprite, Vector3 startPosition, Vector3 startDirection, Vector3 endPosition, Vector3 endDirection, float duration)
    {
        if (coroutine != null) return;

        gameObject.SetActive(true);
        coroutine = StartCoroutine(PlayCoroutine(sprite, startPosition, startDirection, endPosition, endDirection, duration));
    }

    private IEnumerator PlayCoroutine(Sprite sprite, Vector3 startPosition, Vector3 startDirection, Vector3 endPosition, Vector3 endDirection, float duration)
    {
        spriteRenderer.sprite = sprite;

        Vector3 p1 = startPosition + startDirection;
        Vector3 p2 = endPosition + endDirection;

        float spentTime = 0;
        while (spentTime <= duration)
        {
            float time = Mathf.Clamp01(spentTime / duration);
            float oneMinusT = 1.0f - time;

            transform.position = Mathf.Pow(oneMinusT, 3) * startPosition +
                3.0f * Mathf.Pow(oneMinusT, 2) * time * p1 +
                3.0f * oneMinusT * Mathf.Pow(time, 2) * p2 +
                Mathf.Pow(time, 3) * endPosition;

            spentTime += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        coroutine = null;
    }
}
