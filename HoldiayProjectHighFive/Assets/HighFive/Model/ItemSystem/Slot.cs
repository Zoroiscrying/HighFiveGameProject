using System;
using HighFive.Data;
using ReadyGamerOne.Data;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HighFive.Model.ItemSystem
{
    public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
    {
        public ItemData itemData;
        public Image icon;
        public Text countText;
        public event Action<Slot> onPointerEnter;
        public event Action<Slot> onPointerExit;
        public event Action<Slot> onPointerClick;

        private Action<string> onDelete;

        public int Count => int.Parse(countText.text);
        
        public void Refresh(string itemId, int count = 1,Action<string> ondelete=null)
        {
            this.onDelete = ondelete;
            
            itemData = CsvMgr.GetData<ItemData>(itemId);
            
            icon.sprite = ResourceMgr.GetAsset<Sprite>(
                itemData.spriteName
            );

            countText.text = count.ToString();
        }

        public void Add(int count)
        {
            countText.text = (Count + count).ToString();
        }

        public int Remove(int amount)
        {
            var count = Count;
            if (count <= amount)
            {
                onDelete?.Invoke(itemData.ID);
                Destroy(gameObject);
                return count;
            }
            
            countText.text = (Count - amount).ToString();

            return amount;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.Invoke(this);
        }
    }
}
