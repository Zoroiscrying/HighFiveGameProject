using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEditor
{
	public class TestEditorWindow : EditorWindow
	{
		private DialogController dialog;
		
		//添加菜单
		[MenuItem("Dialog/Window")]
		private static void ShowWindow()
		{
			var window = EditorWindow.GetWindow<TestEditorWindow>();
		}

    	/// <summary>
    	/// 成员变量
    	/// </summary>
    
    	private TreeNode curruentNode = null;
    	private Vector2 curruentMousePosition;
    	private bool makingTransition = false;//不在连线
    	//画线
    	private void DrawNodeCurve(Rect rect, Rect end)
    	{
    		var beginPos=new Vector3(rect.x+rect.width/2,rect.y+rect.height/2,0);
    		var endPos=new Vector3(end.x+end.width/2,end.y+end.height/2,0);
    		var startTan = beginPos + Vector3.right * 50;
    		var endTan = endPos + Vector3.left * 50;
    		Handles.DrawBezier(beginPos, endPos, startTan, endTan, Color.green,null, 4);
    	}
    	
    	/// <summary>
    	/// 获取鼠标选中的节点
    	/// </summary>
    	/// <param name="index"></param>
    	/// <returns></returns>
    	private TreeNode GetMouseInNode(out int index)
    	{
    		index = 0;
    		TreeNode selectNode = null;
            var list = dialog.allNodes;
    		for (int i=0;i<list.Count;i++)
    		{
    			if (list[i].rect.Contains(curruentMousePosition))
    			{
    				index = i;
    				selectNode = list[i];
    				break;
    			}
    		}
    
    		return selectNode;
    	}
    
    	/// <summary>
    	/// 绘制本节点到子节点的连线
    	/// </summary>
    	private void DrawToChildCurve(TreeNode parent)
        {
	        if (parent == null)
		        return;
    		foreach (var node in parent.children)
    		{
	            if(node!=null)
    				DrawNodeCurve(parent.rect,node.rect);
    		}
    	}
    	//添加节点
    	private void AddNode()
    	{
    		var node =dialog.AddNode();
            node.rect = new Rect(curruentMousePosition.x, curruentMousePosition.y, 100, 100);
        }
    	//删除节点
    	private void DeleteNode()
    	{
    		int x = 0;
    		curruentNode = GetMouseInNode(out x);
    		if (curruentNode == null) return;
            curruentNode.RealseNode();
            curruentNode = null;
        }
    	private void DeleteNode(TreeNode _node)
    	{
    		if (_node == null) return;
    		//从所有列表中除掉当前节点的连线
    		//从所有列表中除掉当前节点的连线
            dialog.DeleteNode(_node);
        }
    	//开始连线
    	private void MakeTransition()
    	{
    		int x = 0;
    		curruentNode = GetMouseInNode(out x);
    		makingTransition = true;
    	}
    	//菜单
    	private void ShowMenu(int type)
    	{
    		var menu=new GenericMenu();
    		if (type == 0)
    		{
    			//菜单项
    			menu.AddItem(new GUIContent("AddNode"),false,AddNode );
    		}
    		else
    		{
    			//连线子节点
    			menu.AddItem(new GUIContent("Make Transition"),false,MakeTransition);
    			menu.AddSeparator("");
    			//删除节点
    			menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode);
    		}
    		menu.ShowAsContext();
    		Event.current.Use();
    	}
    	
    	private void OnGUI()
    	{
	        dialog=Selection.activeObject as DialogController;
	        if(dialog==null)
		        return;
    		var _event = Event.current;
    		curruentMousePosition = _event.mousePosition;
    		//遍历所有节点清空所有无关点
    		for(int i=0;i<dialog.allNodes.Count;i++)
            {
	            var node = dialog.allNodes[i];
    			if(!node.isUseful)
    				DeleteNode((node));
    		}
    
    		if (makingTransition) //如果是连接模式
    		{
    			if (_event.type == EventType.MouseDown)
    			{
    				int x = 0;
    				var tpNode = GetMouseInNode(out x);
                    if (tpNode != null&&!curruentNode.children.Contains(tpNode))
                    {
						curruentNode.children.Add(tpNode);
						DrawNodeCurve(curruentNode.rect,tpNode.rect);
    					curruentNode = null;
    					makingTransition = false;
                    }
    			}
    			else
    			{
    				var rect = curruentNode.rect;
    				var mr = new Rect(curruentMousePosition.x, curruentMousePosition.y, 5, 5);
    				DrawNodeCurve(rect,mr);
    			}
    		}
    		else//非连接模式
    		{
    			if (_event.button == 1) //鼠标右键
    			{
    				if (_event.type == EventType.MouseDown) //鼠标按下
    				{
    					int x = 0;
    					curruentNode = GetMouseInNode(out x);
    					if (curruentNode != null)
    						ShowMenu(1);
    					else
    					{
    						ShowMenu(0);
    					}
    				}
    			}
    		}
    		//没选中节点时，无法连线
    		if (!makingTransition&&curruentNode == null)
    			makingTransition = false;
    		
    		//开始绘制窗口
            var list = dialog.allNodes;
    		BeginWindows();
    		for (int i = 0; i < list.Count; i++)
    		{
	            list[i].rect = GUI.Window(i, list[i].rect, DrawNodeWindow,"第"+i.ToString()+"个窗口");
    			DrawToChildCurve(list[i]);
    		}
    		EndWindows();
    		
    		Repaint();
    	}
    	
    	void DrawNodeWindow(int id)
        {
	        var node = dialog.allNodes[id];
	        node.name = EditorGUILayout.TextArea(node.name, GUIStyle.none);
    		GUI.DragWindow();
    	}
    }


}
