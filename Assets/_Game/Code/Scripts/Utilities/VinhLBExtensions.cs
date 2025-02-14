using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public static class VinhLBExtensions
    {
        public static T ChangeAlpha<T>(this T t, float newAlpha) where T : Graphic
        {
            Color color = t.color;
            color.a = newAlpha;
            t.color = color;
            
            return t;
        }
        
        public static void Shuffle<T>(this IList<T> list) {
            int count = list.Count;
            int last = count - 1;
            for (int i = 0; i < last; i++) {
                int randomIndex = UnityEngine.Random.Range(i, count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
        
        public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null, bool useNormalizeViewPort = false)
        {
            if (camera == null)
            {
                camera = Camera.main;
            }
        
            Vector3 viewportPosition = camera.WorldToViewportPoint(worldPosition);
        
            if (useNormalizeViewPort)
            {
                Rect normalizedViewPort = camera.rect;
                viewportPosition.x = viewportPosition.x * normalizedViewPort.width + normalizedViewPort.x;
                viewportPosition.y = viewportPosition.y * normalizedViewPort.height + normalizedViewPort.y;
            }
        
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
        {
            Vector3 viewportPosition = new Vector3(
                screenPosition.x / Screen.width,
                screenPosition.y / Screen.height,
                0);
            
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
        {
            Vector3 centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            Vector2 scale = canvasRectTransform.sizeDelta;
            
            return Vector3.Scale(centerBasedViewPortPosition, scale);
        }
        
        public static bool IsOpposite(this Direction source, Direction target)
        {
            uint a = (uint)source | (uint)target;
            uint b = a & 0b_1010;

            return b == 0 || b == a;
        }

        public static Direction Opposite(this Direction direction)
        {
            int a = (int)Mathf.Log((uint)direction, 2);
            int b = (a + 2) % 4;
            Direction result = (Direction)Mathf.Pow(2, b);

            return result;
        }

        public static int GetSortingLayer(this Renderer renderer)
        {
            return SortingLayer.GetLayerValueFromID(renderer.sortingLayerID);
        }
        
        public static void TryUpdateShapeToAttachedSprite(this PolygonCollider2D collider)
        {
            collider.UpdateShapeToSprite(collider.GetComponent<SpriteRenderer>().sprite);
        }

        public static void UpdateShapeToSprite(this PolygonCollider2D collider, Sprite sprite)
        { 
            // Ensure both valid
            if (collider != null && sprite != null)
            {
                // Update count
                collider.pathCount = sprite.GetPhysicsShapeCount();
                
                // New paths variable
                List<Vector2> path = new List<Vector2>();

                // Loop path count
                for (int i = 0; i < collider.pathCount; i++)
                {
                    // Clear
                    path.Clear();
                    // Get shape
                    sprite.GetPhysicsShape(i, path);
                    // Set path
                    collider.SetPath(i, path.ToArray());
                }
            }
        }
    }
}