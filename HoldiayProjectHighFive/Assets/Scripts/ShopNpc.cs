using HighFive.Const;
using ReadyGamerOne.Script;
using ReadyGamerOne.View;
using UnityEngine;

namespace Game.Scripts
{
	public class ShopNpc : CloseEnough
    {
    	private bool isShowing;
    
    	protected override void Start()
    	{
    		base.Start();
    		isShowing = false;
    	}
    	void Update()
    	{
    		if (isClose)
    		{
    			//Debug.Log("Close");
    			if (Input.GetKeyDown(KeyCode.E))
    			{
    				if (isShowing)
    				{
    					PanelMgr.PopPanel();
    					isShowing = false;
    				}
    				else
                    {
    					PanelMgr.PushPanel(PanelName.Shop);
    					isShowing = true;
    				}
    			}
    		}
    	}
    
    	protected override void OnTriggerExit2D(Collider2D col)
    	{
    		base.OnTriggerExit2D(col);
            if (isShowing)
            {
	            PanelMgr.PopPanel();
	            isShowing = false;
            }
    	}
    }

}

