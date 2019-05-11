using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking.NetworkSystem;
using Object = UnityEngine.Object;

namespace Game.MemorySystem
{
	public static  class MemoryMgr 
	{
		//private static List<AssetBundle> assetBundleList=new List<AssetBundle>();
		private static Dictionary<string, Object> sourceObjectDic;

		private static Dictionary<string, Object> SourceObjects
		{
			get
			{
				if(sourceObjectDic==null)
					sourceObjectDic=new Dictionary<string, Object>();
				return sourceObjectDic;
			}
			set { sourceObjectDic = value; }
		}

		public static T GetSourceFromResources<T>(string path)where T : Object
		{
			if (SourceObjects.ContainsKey(path))//加载过这个资源
			{
				if(SourceObjects[path]==null)
					throw new Exception("资源已经释放，但引用亦然保留");
				var ans= SourceObjects[path] as T;
				if (ans == null)
					Debug.LogWarning("资源引用存在，但类型转化失败，小心bug");
				return ans;
			}
			else//初次加载资源
			{
				var source = Resources.Load<T>(path);
				if (source == null)
				{
					Debug.LogError("资源加载错误，错误路径："+path);
					return null;
				}
	
				SourceObjects.Add(path, source);
				return source;
			}

		}

		public static void ReleaseSourceFromResources<T>(string path, ref T sourceObj) where T : Object
		{
			if (!SourceObjects.ContainsKey(path))
			{
				Debug.LogWarning("资源字典中并不包含这个路径，注意路径是否错误：" + path);
				return;
			}

			var ans = SourceObjects[path] as T;
			if (ans == null)
			{
				Debug.LogWarning("该资源路径下Object转化为null，注意路径是否错误：" + path);
				
			}

			SourceObjects.Remove(path);
			Resources.UnloadAsset(ans);
			sourceObj = null;
		}

		public static void ReleaseSourceFromResources<T>(ref T sourceObj) where T : Object
		{
			if (!SourceObjects.ContainsValue(sourceObj))
			{
				Debug.LogWarning("资源字典中没有这个值，你真的没搞错？");
				return;
			}

			var list = new List<string>();
			foreach(var data in SourceObjects)
				if (data.Value == sourceObj)
				{
					Resources.UnloadAsset(data.Value);
					list.Add(data.Key);
				}
			foreach (var s in list)
			{
				SourceObjects.Remove(s);
			}
			list = null;
			
			
			sourceObj = null;
		}

		public static void ReleaseUnusedAssets()
		{
			SourceObjects = null;
			Resources.UnloadUnusedAssets();
		}


		public static GameObject Instantiate(string path,Transform parent=null)
		{
			var source = GetSourceFromResources<GameObject>(path);
			Assert.IsTrue(source);
			return Object.Instantiate(source,parent);
		}

		public static T Instantiate<T>(string path)where T:Object
		{
			return Object.Instantiate(GetSourceFromResources<T>(path));
		}

		public static GameObject Instantiate(string path, Vector3 pos, Quaternion quaternion, Transform parent = null)
		{
			var source = GetSourceFromResources<GameObject>(path);
			Assert.IsTrue(source);
			return Object.Instantiate(source,pos,quaternion,parent);
		}
    }
}

