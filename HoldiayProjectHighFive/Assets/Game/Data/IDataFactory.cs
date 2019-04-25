using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Data
{
	public interface IDataFactory
	{
		ITxtSerializable CreateData(string initLine);
	}
}

