using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace HighFive.Utility.AutoTextureProcessor
{
    public class SpriteSetToSpecificAttributes
    {
        [MenuItem("Assets/SetTextureToMultipleSprites")]
        static void Execute()
        {
            if (Selection.objects.Length>0)
            {
                foreach (var o in Selection.objects)
                {
                    var texture = (Texture2D) o;
                    string selectionPath = AssetDatabase.GetAssetPath(texture);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(selectionPath) as TextureImporter;
                    //设置参数
                    if (textureImporter != null)
                    {
                        textureImporter.isReadable = true;
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                        textureImporter.mipmapEnabled = false;
                        textureImporter.filterMode = FilterMode.Point;
                        textureImporter.spritePivot = new Vector2(.5f,.5f);
                        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                        
                        var textureSettings = new TextureImporterSettings();
                        textureImporter.ReadTextureSettings(textureSettings);
                        textureSettings.spriteMeshType = SpriteMeshType.Tight;
                        textureSettings.spriteExtrude = 0;
                        textureImporter.SetTextureSettings(textureSettings);

                        int SliceWidth = 90;
                        int SliceHeight = 96;
                        Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, Vector2.zero,
                            new Vector2(SliceWidth, SliceHeight), Vector2.zero);
                        var rectsList = new List<Rect>(rects);
                        rectsList = SortRects(rectsList, texture.width);
                        string fileNameNoExtention = Path.GetFileNameWithoutExtension(selectionPath);
                        var metas = new List<SpriteMetaData>();
                        int rectNum = 0;

                        foreach (var rect in rectsList)
                        {
                            Debug.Log("Generate A Rect Meta Data");
                            var meta = new SpriteMetaData();
                            meta.alignment = 9;
                            meta.pivot = new Vector2(.5f,.5f);
                            meta.rect = rect;
                            meta.name = fileNameNoExtention + "_" + rectNum++;
                            metas.Add(meta);
                        }

                        textureImporter.spritesheet = metas.ToArray();
                    }
                    //设置参数
                    AssetDatabase.ImportAsset(selectionPath, ImportAssetOptions.ForceUpdate);

                }
            }
        }

        static List<Rect> SortRects(List<Rect> rects, float textureWidht)
        {
            List<Rect> list = new List<Rect>();
            while (rects.Count>0)
            {
                Rect rect = rects[rects.Count - 1];
                Rect sweepRect = new Rect(0, rect.yMin, textureWidht, rect.height);
                List<Rect> list2 = RectSweep(rects, sweepRect);
                if (list2.Count<=0)
                {
                    list.AddRange(rects);
                    break;
                }
                list.AddRange(list2);
            }

            return list;
        }

        static List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
        {
            List<Rect> result;
            if (rects == null || rects.Count == 0)
            {
                result = new List<Rect>();
            }
            else
            {
                List<Rect> list = new List<Rect>();
                foreach (var current in rects)
                {
                    if (current.Overlaps(sweepRect))
                    {
                        list.Add(current);
                    }
                }

                foreach (var current2 in list)
                {
                    rects.Remove(current2);
                }
                
                list.Sort((a,b)=>a.x.CompareTo(b.x));
                result = list;
            }

            return result;
        }
        
    }
}
