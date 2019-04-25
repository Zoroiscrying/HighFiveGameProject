using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Common;
using UnityEngine;

namespace Game.Data
{
	public class DataFactory<DataType>:Singleton<DataFactory<DataType>>,IDataFactory
		where DataType:class,ITxtSerializable,new()
	{
		public ITxtSerializable CreateData(string initLine)
		{
			var data = new DataType();
			data.SignInit(initLine);
			return data;
		}
	}

}
