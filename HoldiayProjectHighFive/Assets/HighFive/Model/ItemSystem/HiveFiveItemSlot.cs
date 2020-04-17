using System;
using Game.Scripts;
using HighFive.Data;
using HighFive.Global;
using ReadyGamerOne.Data;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Rougelike.Item;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HighFive.Model.ItemSystem
{
    public enum DragType
    {
        Gembox,
        Package
    }
    public class HiveFiveItemSlot : Slot<HiveFiveItemSlot,ItemData>
    {
//        private DragType _dragType = DragType.Package;
        private Dragger dragger;
        private RectTransform selfRect;
        private Func<GameObject, bool> gemboxIsTarget;
        private Func<GameObject, bool> slotIsTarget;
        private Action onDragStateChanged;
        
        protected override void UseItemData()
        {
            icon.sprite = ResourceMgr.GetAsset<Sprite>(
                ItemData.spriteName
            );

            dragger = transform.GetOrAddComponent<Dragger>();
            selfRect = transform.GetComponent<RectTransform>();
        }


        public void InitDragger(Func<GameObject, bool> gemboxIsTarget,Func<GameObject, bool> slotIsTarget,Action onDragStateChanged)
        {
            this.gemboxIsTarget = gemboxIsTarget;
            this.slotIsTarget = slotIsTarget;
            this.onDragStateChanged = onDragStateChanged;
        }


        /// <summary>
        /// 切换拖拽状态
        /// </summary>
        /// <param name="dragType"></param>
        /// <param name="targetObj"></param>
        public void SwitchDragType(DragType dragType,GameObject targetObj)
        {
            HighFiveItem item = null;
            switch (dragType)
            {
                case DragType.Gembox:
                    //Ui上
                    transform.SetParent(targetObj.transform);
                    transform.localPosition = Vector3.zero;
                    selfRect.pivot = new Vector2(0.5f, 0.5f);

                    //逻辑上：宝石放到宝石槽
                    item = GlobalVar.G_Player.GetItems()[ItemData.ID];
                    item.Enable();
                    

                    //Ui逻辑
                    dragger.IsTarget = gemboxIsTarget;
                    dragger.eventOnGetTarget =
                        gridObj =>
                        {
                            item.gemBoxIndex = -1;
                            SwitchDragType(DragType.Package, gridObj);
                            onDragStateChanged?.Invoke();
                        };
                    
                    break;
                case DragType.Package:
                    //UI显示
                    transform.SetParent(targetObj.transform);

                    //逻辑上应处理的：从宝石槽中取下宝石
                    item=GlobalVar.G_Player.GetItems()[ItemData.ID];
                    item.Disable();

                    //ui逻辑
                    dragger.IsTarget = slotIsTarget;
                    dragger.eventOnGetTarget =
                        gemBoxObj =>
                        {
                            item.gemBoxIndex = gemBoxObj.transform.GetSiblingIndex();
                            Debug.Log($"setindex_itemId:{item.GetHashCode()},itemIndex:{item.gemBoxIndex}");
                            SwitchDragType(DragType.Gembox, gemBoxObj);
                            onDragStateChanged?.Invoke();
                        };                        
                    
                    break;
            }
        }
    }
}
