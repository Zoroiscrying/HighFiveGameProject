using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ReadyGamerOne.Data
{
	public abstract class CsvMgr
	{
		//PetConfigData
		static Dictionary<string,Dictionary<string,CsvMgr>> dataDic = new Dictionary<string, Dictionary<string, CsvMgr>> ();

		public static T GetData<T> (string key, string filePath = null) where T:CsvMgr
		{
			var setT = typeof(T);
			if (filePath == null) {
				filePath = setT.Name;
			}

			if (!dataDic.ContainsKey(filePath)) {
				ReadConfigData<T>(filePath);
			}
			var objDic = dataDic[filePath];
			Debug.Log("test  (" + key + ")" + objDic.Count);
			if (!objDic.ContainsKey (key)) {
				throw new Exception ("no this config");
			}
			return (T)(objDic [key]);
		}

		public static List<T> GetDatas<T> (string filePath = null) where T:CsvMgr
		{
			var returnList = new List<T> ();
			var setT = typeof(T);
			if (filePath == null) {
				filePath = setT.Name;
			}

			if (!dataDic.ContainsKey(filePath))
			{
				ReadConfigData<T> (filePath);
			}
			var objDic = dataDic[filePath];
			foreach (var kvp in objDic) {
				returnList.Add ((T)(kvp.Value));
			}
			return returnList;
		}

		static void ReadConfigData<T> (string filePath = null) where T:CsvMgr
		{
			T obj = Activator.CreateInstance<T> ();
			if (filePath == null)
			{
				filePath = typeof(T).Name;
			}

			string getString = Resources.Load<TextAsset>(filePath).text;

			var csr = new CsvReaderByString (getString);

			var objDic = new Dictionary<string, CsvMgr> ();
				
			FieldInfo[] fis = new FieldInfo[csr.ColCount];
			for (int colNum=1; colNum<csr.ColCount+1; colNum++) {
				fis [colNum - 1] = typeof(T).GetField (csr [1, colNum]);
			}

			for (int rowNum=3; rowNum<csr.RowCount+1; rowNum++) {
				T configObj = Activator.CreateInstance<T> ();
				for (int i=0; i<fis.Length; i++) {
					string fieldValue = csr [rowNum, i + 1];
					object setValue = new object ();
					switch (fis [i].FieldType.ToString ()) {
						case "System.Int32":
							setValue = int.Parse (fieldValue);
							break;
						case "System.Int64":
							setValue = long.Parse (fieldValue);
							break;
						case "System.String":
							setValue = fieldValue;
							break;
						default:
							Debug.Log ("error data type");
							break;
					}
					fis [i].SetValue (configObj, setValue);
					if (fis [i].Name == "key" || fis [i].Name == "id") {   
						//只检测key和id的值，然后添加到objDic 中
						objDic.Add (setValue.ToString (), configObj);
					}
				}
			}
			dataDic.Add(filePath, objDic);    //可以作为参数
		}
	}
}
