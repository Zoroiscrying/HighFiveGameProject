using System;
using System.Collections.Generic;
using System.Linq;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace ReadyGamerOne.Data
{
	public abstract class CsvMgr:IHasStringId
	{
		public class FieldInfo
		{
			public string fieldName;
			public string fieldType;
		}
		public class DataConfigInfo
		{
			public string className;
			public string parentName;
			public Dictionary<string,FieldInfo> fieldInfos=new Dictionary<string,FieldInfo>();
			public List<string> childrenNames=new List<string>();
		}

		public const string DefaultDataFileKey = "DefaultDataFileKey";

		#region Private

		private static Dictionary<string, DataConfigInfo> _dataConfigInfos;
		
		/// <summary>
		/// 数据字典 Type => FileKey => data
		/// </summary>
		private static Dictionary<string,
			Dictionary<string,Dictionary<string,CsvMgr>>> allDataDic=new Dictionary<string, Dictionary<string, Dictionary<string, CsvMgr>>>();

		/// <summary>
		/// 安全插入数据
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="data"></param>
		/// <param name="fileKey"></param>
//		private static void SafeInsertData(string typeName, CsvMgr data, string fileKey = null)
//		{
//			Assert.IsNotNull(data);
//			Assert.IsFalse(string.IsNullOrEmpty(typeName));
//			fileKey = string.IsNullOrEmpty(fileKey) ? DefaultDataFileKey:fileKey;
//			
//			
//			if (!allDataDic.ContainsKey(typeName))
//			{
//				allDataDic.Add(typeName, new Dictionary<string,Dictionary<string,CsvMgr>>());
//			}
//
//			var fileKeyDic = allDataDic[typeName];
//			if (!fileKeyDic.ContainsKey(fileKey))
//			{
//				fileKeyDic.Add(fileKey, new Dictionary<string, CsvMgr>());
//			}
//
//			var dataDic = fileKeyDic[fileKey];
//			if (dataDic.ContainsKey(data.ID))
//			{
//				Debug.LogError($"{typeName}:{fileKey}重复ID:{data.ID}");
//				return;
//			}
//
//			dataDic.Add(data.ID, data);
//		}

		/// <summary>
		/// 安全插入数据字典
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="dataDic"></param>
		/// <param name="fileKey"></param>
		private static void SafeInsertDataDic(string typeName, Dictionary<string, CsvMgr> dataDic,
			string fileKey = null)
		{
			Assert.IsNotNull(dataDic);
			Assert.IsFalse(string.IsNullOrEmpty(typeName));
			Assert.IsTrue(_dataConfigInfos.ContainsKey(typeName));
			
			fileKey = string.IsNullOrEmpty(fileKey) ? DefaultDataFileKey:fileKey;
			
			if (!allDataDic.ContainsKey(typeName))
			{
				allDataDic.Add(typeName, new Dictionary<string, Dictionary<string, CsvMgr>>());
			}

			var fileKeyDic = allDataDic[typeName];
			if (!fileKeyDic.ContainsKey(fileKey))
			{
				fileKeyDic.Add(fileKey, new Dictionary<string, CsvMgr>());
			}

			var targetDataDic = fileKeyDic[fileKey];
			foreach (var data in dataDic)
			{
				if (targetDataDic.ContainsKey(data.Key))
				{
					Debug.LogError($"{typeName}:{fileKey}重复ID：{data.Key}");
					continue;
				}

				targetDataDic.Add(data.Key, data.Value);
			}
		}

		/// <summary>
		/// 不考虑继承，安全查询一条数据，fileKey为Null默认查询所有字典,fileKey无效会报警告并查询所有字典
		/// </summary>
		/// <param name="dataId"></param>
		/// <param name="typeName"></param>
		/// <param name="fileKey"></param>
		/// <returns></returns>
		private static CsvMgr SearchDataInternal(string dataId, Type type, string fileKey = null)
		{
			Assert.IsFalse(string.IsNullOrEmpty(dataId));
			Assert.IsNotNull(type);
			Assert.IsTrue(_dataConfigInfos.ContainsKey(type.Name));
			
			var typeName = type.Name;

			//typeName库中有没有:
			if (!allDataDic.ContainsKey(typeName))
			{// 没有type就加载
				LoadConfigData(type, fileKey);
			}
			
			Assert.IsTrue(allDataDic.ContainsKey(typeName));

			var fileDics = allDataDic[typeName];
			var warning = false;
			var searchall = false;
			if (string.IsNullOrEmpty(fileKey))
			{
				searchall = true;
			}else if (!fileDics.ContainsKey(fileKey))
			{
				searchall = true;
				warning = true;
			}

			if (searchall)
			{// 寻找所有dataDic
				foreach (var fileKey_dataDic in fileDics)
				{
					var dataDic = fileKey_dataDic.Value;
					if (dataDic.ContainsKey(dataId))
					{
						if (warning)
						{
							Debug.LogWarning($"无效FileKey:{fileKey}，已查询所有字典代替");
						}

						return dataDic[dataId];						
					}
				}
			}
			else
			{
				//寻找指定FileKeyDic
				var dataDic = fileDics[fileKey];
				if (dataDic.ContainsKey(dataId))
				{
					return dataDic[dataId];
				}
			}

			return null;
		}		
		/// <summary>
		/// 不考虑继承，安全查询所有typename的数据，fileKey为Null默认查询所有字典,fileKey无效会报警告并查询所有字典
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="fileKey"></param>
		/// <returns></returns>
		private static Dictionary<string, CsvMgr> GetDatasInternal(Type dataType, string fileKey = null)
		{
			Assert.IsNotNull(dataType);
			Assert.IsTrue(_dataConfigInfos.ContainsKey(dataType.Name));

			if (!allDataDic.ContainsKey(dataType.Name))
			{
				LoadConfigData(dataType, fileKey);
			}

			Assert.IsTrue(allDataDic.ContainsKey(dataType.Name));
			
			var ans = new Dictionary<string,CsvMgr>();
			
			var fileDics = allDataDic[fileKey];
			var searchall = false;
			if (string.IsNullOrEmpty(fileKey))
			{
				searchall = true;
			}else if (!fileDics.ContainsKey(fileKey))
			{
				searchall = true;
				Debug.LogWarning($"无效FileKey:{fileKey}，已查询所有字典代替");
			}

			if (searchall)
			{
				foreach (var fileKey_dataDic in fileDics)
				{
					foreach (var id_data in fileKey_dataDic.Value)
					{
						if (id_data.Value != null)
						{
							ans.Add(id_data.Key, id_data.Value);
						}
					}
				}
			}
			else
			{
				foreach (var id_data in fileDics[fileKey])
				{
					if (id_data.Value != null)
					{
						ans.Add(id_data.Key, id_data.Value);
					}
				}
			}


			return ans;


		}
		/// <summary>
		/// 加载数据，fileKey不合法的时候默认取type.Name
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filePath"></param>
		private static void LoadConfigData(Type type, string fileKey = null)
		{
			Assert.IsNotNull(type);
			Assert.IsNotNull(_dataConfigInfos);
			
			//fileKey不合法的时候默认取type.Name
			fileKey = string.IsNullOrEmpty(fileKey) ? type.Name : fileKey;
			
			var getString = ResourceMgr.GetAsset<TextAsset>(fileKey,OriginBundleKey.File).text;

			var csr = new CsvReaderByString (getString);

			var dataRow = new Dictionary<string, CsvMgr> ();
				
			var fieldInfos = new System.Reflection.FieldInfo[csr.ColCount];
			for (var colNum=1; colNum<csr.ColCount+1; colNum++) {
				fieldInfos [colNum - 1] = type.GetField (csr [1, colNum]);
			}

			//每行都是一条数据，所以循环行数次
			for (var i=3; i<csr.RowCount+1; i++) {
				
				var dataObj = Activator.CreateInstance(type);
				
				//逐个判断当前行每一列域类型加入字典
				for (var j=0; j<fieldInfos.Length; j++) {
					string fieldValue = csr [i, j + 1];
					
					//试探可能的类型，将数据填充到数据项dataItem中
					var dataItem = new object ();
					switch (fieldInfos [j].FieldType.ToString ()) {
						case "System.Single":
							dataItem = string.IsNullOrEmpty(fieldValue) ? default(float) : float.Parse(fieldValue);
							break;
						case "System.Int32":
							dataItem = string.IsNullOrEmpty(fieldValue) ? default(int) : int.Parse (fieldValue);
							break;
						case "System.Int64":
							dataItem = string.IsNullOrEmpty(fieldValue) ? default(long) : long.Parse (fieldValue);
							break;
						case "System.String":
							dataItem = string.IsNullOrEmpty(fieldValue) ? default(string) : fieldValue;
							break;
						default:
							Debug.LogWarning("error data dataType:"+fieldInfos [j].FieldType);
							break;
					}
					
					fieldInfos [j].SetValue (dataObj, dataItem);
					
					//如果是第一列，这一列被视为主键
					if (j==0) {
						dataRow.Add (dataItem.ToString (), (CsvMgr)dataObj);
					}
				}
			}

			SafeInsertDataDic(
				type.Name, 
				dataRow, 
				fileKey==type.Name? null:fileKey);
		}

		/// <summary>
		/// 读取数据
		/// </summary>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		private static void LoadConfigData<T> (string fileKey = null) where T:CsvMgr
		{
			LoadConfigData(typeof(T), fileKey);
		}

		/// <summary>
		/// 读取数据结构信息
		/// </summary>
		private static void LoadRootConfigData()
		{
			//避免重复初始化
			if (_dataConfigInfos != null)
				return;
			
			_dataConfigInfos=new Dictionary<string, DataConfigInfo>();
			var getString = ResourceMgr.GetAsset<TextAsset>("DataConfig",OriginBundleKey.File).text;

			var csr = new CsvReaderByString (getString);
			for (var i = 1; i < csr.RowCount + 1; i += 2)
			{
				var config = new DataConfigInfo();
				for (var j = 1; j < csr.ColCount + 1; j++)
				{
					var name = csr[i+0, j];
					var type = csr[i+1, j];
					switch (type)
					{
						case "className":
							config.className = name;
							break;
						case "parentClassName":
							if (!string.IsNullOrEmpty(name))
							{
								config.parentName = name;
								_dataConfigInfos[config.parentName]
									.childrenNames.Add(config.className);
							}
							break;
						default:
							if (string.IsNullOrEmpty(name))
								break;
							config.fieldInfos.Add(
								name,
								new FieldInfo
								{
									fieldName = name,
									fieldType = type
								});
							break;
					}
				}

				if (string.IsNullOrEmpty(config.className))
				{
					Debug.LogError("ClassName is null ???");
				}

				_dataConfigInfos.Add(config.className, config);                
			}
		}		
		#endregion

		
		/// <summary>
		/// 考虑继承，安全查询数据，fileKey为Null默认查询所有字典,fileKey无效会报警告并查询所有字典
		/// </summary>
		/// <param name="dataId"></param>
		/// <param name="typeName"></param>
		/// <param name="fileKey"></param>
		/// <returns></returns>
		public static CsvMgr GetData(string dataId, Type dataType, string fileKey = null)
		{
			if(_dataConfigInfos==null)
				LoadRootConfigData();
			
			Assert.IsFalse(string.IsNullOrEmpty(dataId));
			Assert.IsNotNull(dataType);
			
			//如果数据结构表中没有，那么必然没有
			if (!_dataConfigInfos.ContainsKey(dataType.Name))
			{
				Debug.LogWarning($"未注册的数据类型：{dataType.Name}");
				return null;
			}
			
			var ans = SearchDataInternal(dataId, dataType, fileKey);
			if (ans != null)
				return ans;

			//查找子类型
			var nameSpace = dataType.Namespace;
			foreach (var typeName_info in _dataConfigInfos)
			{
				var info = typeName_info.Value;
				if (info.parentName == dataType.Name)
				{
					var typeName = nameSpace + "." + info.className;
					var childType = Type.GetType(typeName);
					if (childType == null)
					{
						Debug.LogError("错误类型:"+typeName);
						continue;
					}
					if (!_dataConfigInfos.ContainsKey(childType.Name))
					{
						Debug.LogError("未注册的类型："+typeName);
						continue;
					}
					
					ans = SearchDataInternal(dataId, childType, fileKey);
					if (ans == null)
					{
						Debug.LogError($"尝试子类型：{typeName}失败");
					}else
						return ans;
				}
			}

			return null;
		}

		/// <summary>
		/// 考虑继承，安全查询数据返回T，fileKey为Null默认查询所有字典,fileKey无效会报警告并查询所有字典
		/// </summary>
		/// <param name="dataId"></param>
		/// <param name="fileKey"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetData<T>(string dataId, string fileKey = null)
			where T:CsvMgr
		{
			return GetData(dataId, typeof(T), fileKey) as T;
		}
		
		/// <summary>
		/// 获取某个种类随机一个数据
		/// </summary>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetRandomData<T>(string filePath = null)
			where T:CsvMgr
		{
			var setT = typeof(T);
			if (filePath == null) {
				filePath =setT.Name;
			}

			if (!allDataDic.ContainsKey(filePath)) {
				LoadConfigData<T>(filePath);
			}
//			Assert.IsTrue(allDataDic.ContainsKey(dataType.Name));
			
			var objDic = allDataDic[filePath];
			var keyList = objDic.Keys.ToList();
			var randomIndex = Random.Range(0, keyList.Count - 1);
			return objDic[keyList[randomIndex]] as T;
		}


		/// <summary>
		/// 根据typeName获取所有类型数据，如果fileKey为null，就查询所有，fileKey无效会警告
		/// </summary>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		public static Dictionary<string,CsvMgr> GetDatas(Type dataType, string fileKey = null)
		{
			if(_dataConfigInfos==null)
				LoadRootConfigData();
			
			var ans = new Dictionary<string, CsvMgr>();
			
			if (!_dataConfigInfos.ContainsKey(dataType.Name))
				return ans;


			//加入当前类型
			var tempAns = GetDatasInternal(dataType, fileKey);
			if (tempAns != null)
			{
				foreach (var id_data in tempAns)
				{
					ans.Add(id_data.Key, id_data.Value);
				}
			}
			
			//加入子类型
			var nameSpace = dataType.Namespace;
			foreach (var typeName_info in _dataConfigInfos)
			{
				var info = typeName_info.Value;
				if (info.parentName == dataType.Name)
				{
					var childType = Type.GetType(nameSpace + info.className);
					if (childType == null)
					{
						Debug.LogError("错误类型:"+nameSpace + info.className);
						continue;
					}
					if (!_dataConfigInfos.ContainsKey(childType.Name))
					{
						Debug.LogError("未注册的类型："+nameSpace + info.className);
						continue;
					}
					
					//加入子类型
					tempAns = GetDatasInternal(childType, fileKey);
					if (tempAns != null)
					{
						foreach (var id_data in tempAns)
						{
							ans.Add(id_data.Key, id_data.Value);
						}
					}
					
				}
			}

			return ans;
		}

		/// <summary>
		/// 根据T获取所有类型数据，如果fileKey为null，就查询所有，fileKey无效会警告
		/// </summary>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<string,T> GetDatas<T> (string fileKey = null) 
			where T:CsvMgr
		{
			var tempAns = GetDatas(typeof(T), fileKey);
			
			var ans = new Dictionary<string, T>();
			foreach (var id_data in tempAns)
			{
				var data = id_data.Value as T;
				if (data != null)
					ans.Add(id_data.Key, data);
			}

			return ans;
		}
		
		public abstract string ID { get; }
	}
}
