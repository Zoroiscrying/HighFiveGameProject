using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Data
{
	public interface ITxtSerializable
	{
		
		string Sign { get; }
		void SignInit(string initLine);
		void LoadTxt(StreamReader sr);
	}
}

