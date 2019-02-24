using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreeNode :DialogController
{
	[SerializeField] public string words;
	public List<TreeNode> children = new List<TreeNode>();
	public bool isUseful = true;
	public Rect rect = new Rect(0, 0, 100, 100);
	public void RealseNode()
	{
		isUseful=false;
	}private void OnEnable()
	{
		name = "New DialogNode.asset";
	}
}
