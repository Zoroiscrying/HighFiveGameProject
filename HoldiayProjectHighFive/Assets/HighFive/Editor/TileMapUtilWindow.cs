using System;
using System.Collections.Generic;
using System.IO;
using HighFive.Script;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace HighFive.Editor
{
    [Serializable]
    public class MapInfo
    {
        public bool enable = true;
        public Tilemap tileMap;
        public Color color=Color.black;
        public Vector3Int offset;
    }
    [Serializable]
    public class TileSwitchUnit
    {
        public Tile src;
        public Tile target;
    }
    public class TileMapUtilWindow:EditorWindow
    {
        [MenuItem("ReadyGamerOne/TileMap工具箱")]
        static void ShowWindow()
        {
            GetWindow<TileMapUtilWindow>();
        }

        #region 小地图生成

        private readonly Color defaultColor=new Color(0.804f,0.804f,0.804f,0.804f);
        [SerializeField] private string miniMapGenerateDirPath;
        [SerializeField] private string sceneName;
        [SerializeField]private Color emptyColor=new Color(0.65f, 1f, 0.99f);

        [SerializeField] private List<MapInfo> tileMapInfos=new List<MapInfo>();
        private ReorderableList tileMapInfoList;


        #region Funcs

        private void OnMiniMapGUI()
        {
            var curName = SceneManager.GetActiveScene().name;
            if (sceneName != curName)
            {
                ClearAll();
                sceneName = curName;
            }
            
            var filePath = $"Assets/{miniMapGenerateDirPath.GetAfterSubstring($"{Application.dataPath}/")}/Map_{sceneName}.asset";
            EditorGUILayout.LabelField("生成路径", filePath);
            emptyColor = EditorGUILayout.ColorField("空白区域颜色", emptyColor);
            tileMapInfoList?.DoLayoutList();
            if (GUILayout.Button("加载场景所有TileMap",GUILayout.Height(2*EditorGUIUtility.singleLineHeight)))
            {
                LoadTileMapsFromScene();
            }
            
            EditorGUILayout.BeginHorizontal(GUILayout.Height(2*EditorGUIUtility.singleLineHeight));

            if (GUILayout.Button("清空所有",GUILayout.Height(2*EditorGUIUtility.singleLineHeight)))
            {
                ClearAll();
            }

            if (GUILayout.Button("启用所有",GUILayout.Height(2*EditorGUIUtility.singleLineHeight)))
            {
                foreach (var info in tileMapInfos)
                {
                    info.enable = true;
                }
            }

            if (GUILayout.Button("禁用所有",GUILayout.Height(2*EditorGUIUtility.singleLineHeight)))
            {
                foreach (var info in tileMapInfos)
                {
                    info.enable = false;
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("生成地图",GUILayout.Height(2*EditorGUIUtility.singleLineHeight)))
            {
                GenerateMap();
            }

        }

        /// <summary>
        /// 清空TileMap列表
        /// </summary>
        private void ClearAll()
        {
            tileMapInfos.Clear();
        }
        
        /// <summary>
        /// 生成小地图
        /// </summary>
        private void GenerateMap()
        {
            Vector3Int min, max, size;
            if (!CheckTileMapsValid(out min,out max,out size))
                return;

            if (!Directory.Exists(miniMapGenerateDirPath))
                Directory.CreateDirectory(miniMapGenerateDirPath);
            
            
            
            var mapTexture = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);

            mapTexture.filterMode = FilterMode.Point;
            
            foreach (var info in tileMapInfos)
            {
                if(!info.enable)
                    continue;
                
                // 对于每一个TileMap都要把他们绘制到地图上
                var tileMapSize = info.tileMap.size;
                var tileMapMin = info.tileMap.cellBounds.min;
                for (var i = 0; i < tileMapSize.x; i++)
                {
                    for (var j = 0; j < tileMapSize.y; j++)
                    {
                        
                        var pos = new Vector3Int( tileMapMin.x + i,tileMapMin.y + j, 0);

                        var tile = info.tileMap.GetTile(pos);
                        if(tile!=null)
                            mapTexture.SetPixel(info.offset.x + i,info.offset.y + j, info.color);
                    }
                }
            }

            #region 自定义背景颜色

            
            for (var i = 0; i < size.x; i++)
            {
                for (var j = 0; j < size.y; j++)
                {
                    var color = mapTexture.GetPixel(i, j);
                    if (color.EqualsTo(defaultColor))
                        mapTexture.SetPixel(i, j, emptyColor);
                }
            }

            #endregion


            #region 持久化保存

            mapTexture.Apply();
            var filePath = $"Assets/{miniMapGenerateDirPath.GetAfterSubstring($"{Application.dataPath}/")}/Map_{sceneName}.asset";
            AssetDatabase.CreateAsset(mapTexture, filePath);
            AssetDatabase.Refresh();
            

            #endregion

            ProjectWindowUtil.ShowCreatedAsset(mapTexture);


            ResourceMgr.GenerateResourcesConst("HighFive", "Const", "Auto");

        }

        /// <summary>
        /// 检查条件是否可以生成地图
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool CheckTileMapsValid(out Vector3Int min,out Vector3Int max,out Vector3Int size)
        {
            min=Vector3Int.zero;
            max=Vector3Int.zero;
            size=Vector3Int.zero;

            var sceneUtil = FindObjectOfType<SceneUtil>();

            if (!sceneUtil)
            {
                Debug.LogError("当前场景中没有SceneUtil脚本");
                return false;
            }

            if (tileMapInfos.Count == 0)
            {
                Debug.LogError($"tilemap 列表为空");
                return false;
            }

            foreach (var info in tileMapInfos)
            {
                if(!info.enable)
                    continue;
                
                var tileMap = info.tileMap;
                
                if (tileMap == null)
                {
                    Debug.LogError($"列表中有tilemap为null");
                    return false;
                }


                if (System.Math.Abs(info.color.a) < 0.01f)
                {
                    Debug.LogError($"{tileMap.name}颜色Alpha通道为0");
                    return false;
                }
                
                if (Vector3Int.zero != Vector3Int.RoundToInt(tileMap.transform.position))
                {
                    Debug.LogError($"{tileMap.name}世界坐标不在原点");
                    return false;
                }

                var cellBounds = tileMap.cellBounds;
                
                min = Vector3Int.Min(min, cellBounds.min);
                max = Vector3Int.Max(max, cellBounds.max);

                sceneUtil.mapMax = max;
                sceneUtil.mapMin = min;

                EditorUtility.SetDirty(sceneUtil);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                
            }

            foreach (var info in tileMapInfos)
            {
                if(!info.enable)
                    continue;
                info.offset = info.tileMap.cellBounds.min - min;
            }

            size.x = max.x - min.x;
            size.y = max.y - min.y;

            return true;
        }
        
        /// <summary>
        /// 从当前场景中加载TileMaps
        /// </summary>
        private void LoadTileMapsFromScene()
        {
            for (var i = 0; i < tileMapInfos.Count; i++)
            {
                if (tileMapInfos[i].tileMap == null)
                {
                    tileMapInfos.RemoveAt(i);
                    i--;
                }
            }
            foreach (var tilemap in FindObjectsOfType<Tilemap>())
            {
                tileMapInfos.Add(new MapInfo
                {
                    tileMap = tilemap,
                    color = Color.black,
                });
            }
        }        

        #endregion
        

        #endregion

        #region Tile替换

        private List<Tilemap> tilemaps = new List<Tilemap>();
        private ReorderableList tilemapList;

        private List<TileSwitchUnit> tileSwitchInfos = new List<TileSwitchUnit>();
        private ReorderableList tileList;


        #region Funcs

        private void OnSwitchTileGUI()
        {
            tilemapList.DoLayoutList();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("加载所有TileMap"))
            {
                //清理空Tilemap
                for (var i = 0; i < tilemaps.Count; i++)
                {
                    if (tilemaps[i] == null)
                    {
                        tilemaps.RemoveAt(i);
                        i--;
                    }
                }
                //加入全部tileMap
                foreach (var tilemap in FindObjectsOfType<Tilemap>())
                {
                    if(tilemaps.Contains(tilemap))
                        continue;
                    tilemaps.Add(tilemap);
                }
            }

            if (GUILayout.Button("清空TileMap"))
            {
                tilemaps.Clear();
            }
            EditorGUILayout.EndHorizontal();

            tileList.DoLayoutList();
            if (GUILayout.Button("开始替换", GUILayout.Height(2 * EditorGUIUtility.singleLineHeight)))
            {
                bool changed = false;
                foreach (var info in tilemaps)
                {
                    if(null==info)
                        continue;

                    bool flag = false;
                    // 对于每一个TileMap都要把他们绘制到地图上
                    var tileMapSize = info.size;
                    var tileMapMin = info.cellBounds.min;
                    for (var i = 0; i < tileMapSize.x; i++)
                    {
                        for (var j = 0; j < tileMapSize.y; j++)
                        {
                        
                            var pos = new Vector3Int( tileMapMin.x + i,tileMapMin.y + j, 0);

                            var tile = info.GetTile(pos);
                            if (tile == null) continue;
                            foreach (var tilePair in tileSwitchInfos)
                            {
                                if (tilePair.src == tile)
                                {
                                    info.SetTile(pos, tilePair.target);
                                    flag = true;
                                }
                            }
                        }
                    }

                    if (flag)
                    {
                        EditorUtility.SetDirty(info);
                        changed = true;
                    }
                }
                if(changed)
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                else
                {
                    Debug.LogWarning("???，没变？？？");
                }
            }
        }

        #endregion

        #endregion


        private GUIStyle _style;
        
        private Vector2 postition;

        private string[] toolNames = {"tile替换","小地图生成"};
        private int toolIndex;
        private void OnEnable()
        {
            _style=new GUIStyle
            {
                fontSize = 15,
                fontStyle = FontStyle.BoldAndItalic,
                alignment = TextAnchor.MiddleCenter
            };

            #region MiniMap生成

            tileMapInfoList = new ReorderableList(tileMapInfos, typeof(MapInfo), true, true, true, true);
            tileMapInfoList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var pi = 0;
                var info = tileMapInfos[index];
                info.enable = EditorGUI.Toggle(rect.GetRectAtIndex(pi++), "是否启用", info.enable);
                info.tileMap = EditorGUI.ObjectField(rect.GetRectAtIndex(pi++),
                    "带生成的TileMap", info.tileMap, typeof(Tilemap), true) as Tilemap;
                info.color = EditorGUI.ColorField(rect.GetRectAtIndex(pi++), "显示颜色", info.color);
            };
            tileMapInfoList.elementHeight = 3 * EditorGUIUtility.singleLineHeight;
            tileMapInfoList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "待生成的TileMaps");
            miniMapGenerateDirPath = Application.dataPath + "/Resources/ClassUi/Maps";

            sceneName = SceneManager.GetActiveScene().name;

            #endregion

            #region 替换Tile

            tileList = new ReorderableList(tileSwitchInfos, typeof(TileSwitchUnit), true, true, true, true);
            tileList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;
            tileList.drawHeaderCallback = rect => EditorGUI.LabelField(rect,"替换的Tile");
            tileList.drawElementCallback = (rect, index, a, b) =>
            {
                var i = 0;
                tileSwitchInfos[index].src =
                    EditorGUI.ObjectField(rect.GetRectAtIndex(i++), "原Tile",
                        tileSwitchInfos[index].src, typeof(Tile), true) as Tile;
                tileSwitchInfos[index].target =
                    EditorGUI.ObjectField(rect.GetRectAtIndex(i++), "新Tile",
                        tileSwitchInfos[index].target, typeof(Tile), true) as Tile;
            };
            
            tilemapList = new ReorderableList(tilemaps, typeof(Tilemap), true, true, true, true);
            tilemapList.elementHeight = EditorGUIUtility.singleLineHeight;
            tilemapList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "操作的TileMaps");
            tilemapList.drawElementCallback = (rect, index, a, b) =>
            {
                tilemaps[index] =
                    EditorGUI.ObjectField(
                        rect, 
                        "tileMap", 
                        tilemaps[index],
                        typeof(Tilemap), 
                        true) as Tilemap;
            };            

            #endregion
            
        }


        private void OnGUI()
        {
            
            postition = EditorGUILayout.BeginScrollView(postition);
            
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("TileMap工具箱", _style);
            EditorGUILayout.Space();
            

            toolIndex = GUILayout.Toolbar(toolIndex, toolNames);
            switch (toolIndex)
            {
                case 0:// 替换Tile
                    OnSwitchTileGUI();
                    break;
                case 1:// MiniMap
                    OnMiniMapGUI();
                    break;
                
            }
            
            
            EditorGUILayout.EndScrollView();
            
        }

    }
}