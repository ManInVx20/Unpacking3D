using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace VinhLB
{
    public static class VinhLBUtility
    {
        public static Direction GetDragDirection(Vector3 dragVector)
        {
            float positiveX = Mathf.Abs(dragVector.x);
            float positiveY = Mathf.Abs(dragVector.y);
            Direction direction;
            if (positiveX > positiveY)
            {
                direction = (dragVector.x > 0) ? Direction.Right : Direction.Left;
            }
            else
            {
                direction = (dragVector.y > 0) ? Direction.Up : Direction.Down;
            }

            return direction;
        }

        public static float GetAngleByDirection(Direction direction)
        {
            float angle = 0f;
            if (direction == Direction.Up)
            {
                angle = 0f;
            }
            else if (direction == Direction.Right)
            {
                angle = -90f;
            }
            else if (direction == Direction.Down)
            {
                angle = -180f;
            }
            else if (direction == Direction.Left)
            {
                angle = 90f;
            }

            return angle;
        }

        public static (int, int) GetNextLocationByDirection(int x, int y, Direction direction)
        {
            int nextX = x;
            int nextY = y;
            if (direction == Direction.Up)
            {
                nextY += 1;
            }
            else if (direction == Direction.Right)
            {
                nextX += 1;
            }
            else if (direction == Direction.Down)
            {
                nextY -= 1;
            }
            else if (direction == Direction.Left)
            {
                nextX -= 1;
            }

            return (nextX, nextY);
        }

        public static Vector2 GetPointerDeltaPosition(int touchIndex = 0)
        {
            Vector2 deltaPosition = Vector2.zero;
            if (IsOnEditor())
            {
                if (Input.GetMouseButton(0))
                {
                    deltaPosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                }
            }
            else
            {
                if (Input.touchCount > 0 && touchIndex < Input.touchCount)
                {
                    deltaPosition = Input.GetTouch(touchIndex).deltaPosition;
                    deltaPosition.x /= Screen.width;
                    deltaPosition.y /= Screen.height;
                    deltaPosition *= 40f;
                }
            }

            return deltaPosition;
        }
        
        public static Vector3 GetPointerScreenPosition(int touchIndex = 0)
        {
            Vector3 pointerScreenPosition = Vector3.zero;
            if (IsOnEditor())
            {
                if (Input.GetMouseButton(0))
                {
                    pointerScreenPosition = Input.mousePosition;
                }
            }
            else
            {
                if (Input.touchCount > 0 && touchIndex < Input.touchCount)
                {
                    pointerScreenPosition = Input.GetTouch(touchIndex).position;
                }
            }

            return pointerScreenPosition;
        }
        
        public static Vector3 GetPointerWorldPosition(Camera camera = null, float zPosition = 0, int touchIndex = 0)
        {
            Vector3 pointerWorldPosition = Vector3.zero;
            camera ??= Camera.main;
            if (camera != null)
            {
                Vector3 pointerScreenPosition = GetPointerScreenPosition(touchIndex);
                pointerScreenPosition.z = zPosition;
                pointerWorldPosition = camera.ScreenToWorldPoint(pointerScreenPosition);
            }

            return pointerWorldPosition;
        }
        
        public static Vector3 ScreenToWorldPosition(Vector3 screenPosition, Camera camera = null)
        {
            Vector3 pointerWorldPosition = Vector3.zero;
            camera ??= Camera.main;
            if (camera != null)
            {
                pointerWorldPosition = camera.ScreenToWorldPoint(screenPosition);
            }

            return pointerWorldPosition;
        }
        
        public static bool CheckClickOnUI(int touchIndex = 0)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = GetPointerScreenPosition(touchIndex);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            foreach (var item in results)
            {
                if (item.gameObject.layer == VinhLBLayer.UI)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        // Returns 'true' if we touched or hovering on Unity UI element
        public static bool IsPointerOverUI(int touchIndex = 0)
        {
            return IsPointerOverUI(GetEventSystemRaycastResults(touchIndex));
        }
        
        private static bool IsPointerOverUI(List<RaycastResult> raycastResults)
        {
            for (int i = 0; i < raycastResults.Count; i++)
            {
                if (raycastResults[i].gameObject.layer == VinhLBLayer.UI)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        private static List<RaycastResult> GetEventSystemRaycastResults(int touchIndex)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = GetPointerScreenPosition(touchIndex);
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            
            return raycastResults;
        }

        
        public static bool IsPointerActive()
        {
            bool result;
            if (IsOnEditor())
            {
                result = Input.GetMouseButton(0);
            }
            else
            {
                result = Input.touchCount > 0;
            }

            return result;
        }

        public static bool IsPointerDown()
        {
            bool result;
            if (IsOnEditor())
            {
                result = Input.GetMouseButtonDown(0);
            }
            else
            {
                result = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
            }

            return result;
        }

        public static bool IsPointerUp()
        {
            bool result;
            if (IsOnEditor())
            {
                result = Input.GetMouseButtonUp(0);
            }
            else
            {
                result = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
            }

            return result;
        }

        public static bool IsAnyPointerDown(out List<TouchData> touchDataList)
        {
            touchDataList = new List<TouchData>();
            bool result = false;
            if (IsOnEditor())
            {
                result = Input.GetMouseButtonDown(0);
            }
            else
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Began)
                    {
                        touchDataList.Add(new TouchData()
                        {
                            Index = i,
                            Position = touch.position
                        });
                        result = true;
                    }
                }
            }

            return result;
        }
        
        public static bool TryCastRay(Camera camera, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity, 
            int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            Vector3 pointerScreenPosition = GetPointerScreenPosition();
            
            Vector3 pointerScreenPositionFar = pointerScreenPosition;
            pointerScreenPositionFar.z = camera.farClipPlane;
            Vector3 pointerScreenPositionNear = pointerScreenPosition;
            pointerScreenPositionNear.z = camera.nearClipPlane;
            
            Vector3 pointerWorldPositionFar = camera.ScreenToWorldPoint(pointerScreenPositionFar);
            Vector3 pointerWorldPositionNear = camera.ScreenToWorldPoint(pointerScreenPositionNear);
            
            return Physics.Raycast(pointerWorldPositionNear, pointerWorldPositionFar - pointerWorldPositionNear, 
                out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        }
        
        private static bool IsOnEditor()
        {
            // return Application.platform == RuntimePlatform.WindowsEditor ||
            //        Application.platform == RuntimePlatform.OSXEditor ||
            //        Application.platform == RuntimePlatform.LinuxEditor;
            return UnityEngine.Device.Application.isEditor;
        }
        
        public static string GetTextboxColor(Color color, string text)
        {
            string htmlString = $"#{ColorUtility.ToHtmlStringRGBA(color)}";
            
            return GetTextboxColor(htmlString, text);
        }

        public static string GetTextboxColor(string htmlString, string text)
        {
            return $"<color={htmlString}>{text}</color>";
        }
        
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        
        private static int[] GetRandomsInRange(int length, int min, int max)
        {
            int rangeSize = max - min;
            if (length <= 0 || length > rangeSize)
            {
                return null;
            }
            
            int[] results = new int[length];
            List<int> numberList = new List<int>(Enumerable.Range(min, rangeSize));
            for (int i = 0; i < results.Length; i++)
            {
                int randomIndex = Random.Range(0, numberList.Count);
                results[i] = numberList[randomIndex];
                numberList.RemoveAt(randomIndex);
            }

            return results;
        }
        
        public static void DrawRect(Rect rect)
        {
            Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
        }
        
        public static bool IsInLayerMask(GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
        
        public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
    }
    
    [Flags]
    public enum Direction
    {
        Up = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Left = 1 << 3
    }

    public enum BotMode
    {
        Easy = 0,
        Normal = 1,
        Hard = 2
    }
    
    public struct TouchData
    {
        public int Index;
        public Vector3 Position;
    }
}