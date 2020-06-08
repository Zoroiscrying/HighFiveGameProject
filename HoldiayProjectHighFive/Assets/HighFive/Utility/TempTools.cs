using ReadyGamerOne.Global;
using UnityEngine.Tilemaps;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HighFive.Utility
{
    public class TempTools:IEditorTools
    {
#pragma warning disable 414
        static string Title = "测试";
        private static Tilemap _tilemap;
        private static Object testObject;
        private static Vector2Int pos;
        private static RectTransform rect;
        private static SpriteRenderer _spriteRenderer;
        private static Camera _camera;
#pragma warning restore 414

#if UNITY_EDITOR
        static void OnToolsGUI(string rootNs, string viewNs, string constNs, string dataNs, string autoDir,
                                     string scriptDir)
        {
            _camera=EditorGUILayout.ObjectField("测试Camera",_camera,typeof(Camera),true) as Camera;
            if (GUILayout.Button("显示Camera信息") && _camera)
            {
                Debug.Log($"orthographicSize:{_camera.orthographicSize}");
                Debug.Log($"aspect:{_camera.aspect}");
                var size = 2 * new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);
                Debug.Log($"size:{size}");
            }
            
            
            _spriteRenderer=EditorGUILayout.ObjectField("测试SpriteRenderer",_spriteRenderer,typeof(SpriteRenderer),true) as SpriteRenderer;
            if (GUILayout.Button("显示SpriteRenderer信息") && _spriteRenderer)
            {
                var bounds = _spriteRenderer.bounds;
                Debug.Log($"Min:{bounds.min}, Max:{bounds.max}");
                Debug.Log($"size:{bounds.size}");
                Debug.Log($"center:{bounds.center}");
            }
            
            
            testObject = EditorGUILayout.ObjectField("测试Object", testObject, typeof(Object),true);
            if (GUILayout.Button("Show Type"))
            {
                if (testObject != null)
                {
                    Debug.Log(testObject.GetType());
                }
            }
            
            rect=EditorGUILayout.ObjectField(
                "RectTransform",rect,typeof(RectTransform),true)as RectTransform;
            if (GUILayout.Button("ShowRectInfo"))
            {
                Debug.Log($"rect.size:{rect.rect.size}");
                Debug.Log($"sizeDelta:{rect.sizeDelta}");
                Debug.Log($"offsetMin:{rect.offsetMin}");
                Debug.Log($"offsetMax:{rect.offsetMax}");
                Debug.Log($"anchoredPosition:{rect.anchoredPosition}");
                Debug.Log($"anchorMax:{rect.anchorMax}");
                Debug.Log($"anchorMin:{rect.anchorMin}");
            }
            
            _tilemap = EditorGUILayout.ObjectField("显示地图",
                _tilemap, typeof(Tilemap), true) as Tilemap;
            if (GUILayout.Button("开始生成", GUILayout.Height(3 * EditorGUIUtility.singleLineHeight)))
            {
                if (_tilemap == null )
                {
                    Debug.LogError("tilemap  is null");
                    return;
                }

                var size = _tilemap.size;
                var cellBounds = _tilemap.cellBounds;
                var left = cellBounds.min.x;
                var bottom = cellBounds.min.y;
                var startPos = new Vector3Int(left, bottom, 0);
                
                var t = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);
                Debug.Log($"Size:{size}");
                Debug.Log($"min:{_tilemap.cellBounds.min}");
                Debug.Log($"Start:{startPos}");
                for (var i = 0; i < size.x; i++)
                {
                    for (var j = 0; j < size.y; j++)
                    {
                        var pos = startPos + new Vector3Int(i, j, 0);
                        var ans = _tilemap.GetTile(pos);
                        var color = ans == null ? Color.black : Color.white;
                        t.SetPixel(i, j, color);
                    }
                }
                

                t.Apply();
                AssetDatabase.CreateAsset(t, "Assets/Texture/test.asset");
                AssetDatabase.Refresh();
            }
        }
#endif
        
    }
}