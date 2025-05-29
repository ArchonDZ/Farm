using System.Collections.Generic;
using UnityEngine;

public static class PolygonCollider2DExtensions
{
    public static void UpdateColliderToSprite(this PolygonCollider2D collider, Sprite sprite)
    {
        if (collider == null || sprite == null)
            return;

        List<Vector2> path = new List<Vector2>();
        collider.pathCount = sprite.GetPhysicsShapeCount();

        for (int i = 0; i < collider.pathCount; i++)
        {
            path.Clear();
            sprite.GetPhysicsShape(i, path);
            collider.SetPath(i, path.ToArray());
        }
    }
}