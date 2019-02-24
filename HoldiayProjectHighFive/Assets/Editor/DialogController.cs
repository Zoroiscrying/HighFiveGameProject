using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class DialogController : ScriptableObject
{
    public List<TreeNode> allNodes=new List<TreeNode>();
    private static DialogController instance;
    private static Dictionary<int, DialogController> DialogDic = new Dictionary<int, DialogController>();

    [MenuItem("Assets/Create/DialogControllor")]
    static void CreateDialog()
    {
	    //父类实例化
	    instance = CreateInstance<DialogController>();
	    Debug.Log(instance);
	    AssetDatabase.CreateAsset(instance, AssetDatabase.GetAssetPath(Selection.activeObject) + "/New DialogController.asset");
	    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Selection.activeObject) + "/New DialogController.asset");
	    DialogDic.Add(instance.GetInstanceID(),instance);
    }
    
    [MenuItem("Dialog/ShowAll")]
    private static void ShowAll()
    {
	    foreach (var t in DialogDic)
	    {
		    Debug.Log(t.Key.ToString()+t.Value);
	    }
    }
    //添加节点
    public TreeNode AddNode()
    {
    	var nodeRoot = ScriptableObject.CreateInstance<TreeNode>();
        //父类上添加子类
        AssetDatabase.AddObjectToAsset(nodeRoot, AssetDatabase.GetAssetPath(Selection.activeObject));
    	allNodes.Add(nodeRoot);
        return nodeRoot;
    }

    public void DeleteNode(TreeNode ele)
    {
	    allNodes.Remove(ele);
    	for(int i=0;i<allNodes.Count;i++)
        {
	        var node = allNodes[i];
    		if (node.children.Contains(ele))
    			node.children.Remove(ele);
    	}
	    //删除子物体
	    Object.DestroyImmediate (ele, true);
	    //用Import更新状态
	    AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath(Selection.activeObject));
    }

    private void OnDestroy()
    {
	    
    }
}