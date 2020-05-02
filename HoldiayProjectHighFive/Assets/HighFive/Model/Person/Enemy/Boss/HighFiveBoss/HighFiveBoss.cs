using ReadyGamerOne.Rougelike.Person;
using System;
using HighFive.Data;
using UnityEngine;

namespace HighFive.Model.Person
{
	public interface IHighFiveBoss : 
		IHighFiveEnemy
	{
	}

	public abstract class HighFiveBoss<T>:
		HighFiveEnemy<T>,
		IHighFiveBoss
		where T : HighFiveBoss<T>,new()
	{
		public override Type DataType => typeof(BossData);
	}
}
