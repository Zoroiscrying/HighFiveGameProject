using System;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.IO;
using System.Runtime.Serialization;
using Game.Script;
using Game.Control;
using Game.Serialization;

namespace Game
{

    ///////////////////////////////    全局类     ///////////////////////////////////
   
    /// <summary>
    /// 单例泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T>
    where T: class,new()
    {
        private static T instance = null;
        public  static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }
    }
	/// <summary>
	/// 安全单例
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class SafeSingleton<T>
	    where T : class, new()
    {
	    private static bool isInsciated=false;

	    public SafeSingleton()
	    {
		    if (isInsciated)
			    throw new Exception("这是安全单例，不可多次初始化");
		    isInsciated = true;
	    }
    }
    
    /// <summary>
    /// MonoBehavior单例泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T>:MonoBehaviour
        where T : MonoSingleton<T>, new()
    {
        private static T instance;
        public static T Instance
        {
            get { return instance; }
        }
        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
    
   
    /// <summary>
    /// 游戏计算
    /// </summary>
    public static partial class GameMath
    {
        internal static int FindChar(string str, char ch,int num)
        {
            var n = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (ch == str[i])
                {
                    n++;
                    if (n == num)
                        return i;
                }
            }
            return -1;
        }

    }
    
    ///////////////////////////////     事件机制     /////////////////////////////////

    /// <summary>
    /// 事件中心
    /// 监听分发事件
    /// </summary>CallBack
    public static class CEventCenter
    {
        
        /// <summary>
        /// 消息字典
        /// </summary>
        private static SerializableDictionary<string, List<Delegate>> listeners;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        static CEventCenter()
        {
            listeners = new SerializableDictionary<string, List<Delegate>>();
        }

        /// <summary>
        /// 添加监听事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public static void AddListener(string type, Action listener)
        {
            if (!listeners.ContainsKey(type))
                listeners.Add(type, new List<Delegate>());
            listeners[type].Add(listener);
        }
        public static void AddListener<T>(string type, Action<T> listener)
        {
            if (!listeners.ContainsKey(type))
                listeners.Add(type, new List<Delegate>());
            listeners[type].Add(listener);
        }
        public static void AddListener<T,U>(string type, Action<T,U> listener)
        {
            if (!listeners.ContainsKey(type))
                listeners.Add(type, new List<Delegate>());
            listeners[type].Add(listener);
        }
        /// <summary>
        /// 移出监听事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public static void RemoveListener(string type, Action listener)
        {
            if (listeners.ContainsKey(type))
                listeners[type].Remove(listener);
        }
        public static void RemoveListener<T>(string type, Action<T> listener)
        {
            if (listeners.ContainsKey(type))
                listeners[type].Remove(listener);
        }
        public static void RemoveListener<T,U>(string type, Action<T,U> listener)
        {
            if (listeners.ContainsKey(type))
                listeners[type].Remove(listener);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="evt"></param>
        public static void SendMessage(CBaseMessage evt)
        {
            BroadMessage<CBaseMessage>(evt.Type, evt);
        }
        public static void BroadMessage(string type)
        {
            if(listeners.ContainsKey(type))
            {
                var list = listeners[type];
                for (var i = 0; i < list.Count; i++)
                {
                    var f = list[i] as Action;
                    if (null != f)
                        f();
                }
            }
        }
        public static void BroadMessage<T>(string type, T arg1)
        {
            if (listeners.ContainsKey(type))
            {
                var list = listeners[type];
                for (var i = 0; i < list.Count; i++)
                {
                    var f = list[i] as Action<T>;
                    if (null != f)
                        f(arg1);
                }
            }
        }
        public static void BroadMessage<T,U>(string type,T arg1,U arg2)
        {
            if (listeners.ContainsKey(type))
            {
                var list = listeners[type];
                for (var i = 0; i < list.Count; i++)
                {
                    var f = list[i] as Action<T,U>;
                    if (null != f)
                        f(arg1,arg2);
                }
            }
        }
        public static void RemoveAll()
        {
            listeners.Clear();
        }
    }
    
    /// <summary>
    /// 消息基类
    /// </summary>
    public class CBaseMessage
    {

        protected string type; //消息类型
        protected Object sender;              //发送者
                                              //protected Hashtable otherMsg;         //其他信息
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public Object Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        public override string ToString()
        {
            return this.type + "[" + ((this.sender == null) ? "null" : this.sender.ToString()) + "]";
        }

        public CBaseMessage Clone()
        {
            return new CBaseMessage(this.type, this.sender);//, this.otherMsg);
        }

        public CBaseMessage(string _type = "", Object _sender = null)//, Hashtable dic =null)
        {
            this.type = _type;
            this.sender = _sender;
        }
    }
    
    
    //////////////////////////////     对象池       ///////////////////////////////////

    public interface IObjectPool<T>where T:Object
    {
	    T BorrowObject();
	    void ReturnObject(T obj);
	    void Release();
    }

    public class BaseObjectPool<T>:IObjectPool<T>where T:Object
    {

	    #region private

	    private Queue<T> objectQue;
	    private string type;
	    private string path;
	    #endregion
	    
	    #region 属性
	    
	    public string Type
	    {
		    get { return this.type; }
	    }

	    public int Count
	    {
		    get { return this.objectQue.Count; }
	    }
	    
	    #endregion

	    
	    #region 单例，构造，初始化
	    
	    private static BaseObjectPool<T> instance;

	    public static BaseObjectPool<T> Instance
	    {
		    get
		    {
			    Assert.IsTrue(instance != null);
			    return instance;
		    }
	    }

	    public static void InitPool(string type,string path)
	    {
		    instance = new BaseObjectPool<T>(type, path);
	    }
	    
	    private BaseObjectPool(string type, string path)
	    {
		    this.objectQue=new Queue<T>();
		    this.type = type;
		    this.path = path;
	    }
	    
	    #endregion

	    protected virtual T Init(T origin)
	    {
		    throw new NotImplementedException();
	    }

	    protected virtual T Hide(T obj)
	    { 
		    throw new NotImplementedException();
	    }

	    #region IObjectPool<T>
	    
	    public T BorrowObject()
	    {
		    if (this.objectQue.Count == 0)
		    {
			    var obj = Resources.Load<T>(path);
			    return this.Init(obj);
		    }
		    else
		    {
			    return this.Init(this.objectQue.Dequeue());
		    }
	    }

	    public void ReturnObject(T obj)
	    {
		    this.Hide(obj);
		    this.objectQue.Enqueue(obj);
	    }

	    public void Release()
	    {
		    this.objectQue.Clear();
	    }
	    
	    #endregion
    }

//    public interface IObjectPoolFactory<T>where T:Object
//    {
//	    BaseObjectPool<T> CreateObjectPool(string name, string path);
//    }
//
//    public class ObjectPoolFactory<T> : IObjectPoolFactory<T>where T:Object
//    {
//	    
//
//	    public BaseObjectPool<T> CreateObjectPool(string name, string path)
//	    {
//		    
//	    }
//    }
    
    public class PoolMgr: Singleton<PoolMgr>
    {
	    public PoolMgr()
	    {
		    //这里进行各种池的初始化
	    }
	    
	    
	    /// <summary>
	    /// 获取任意池
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    public BaseObjectPool<T> GetPool<T>() where T : Object
	    {
		    return BaseObjectPool<T>.Instance;
	    }
    }
}


namespace Game.Modal
{
    /// <summary>
    /// 2D图片基类
    /// </summary>
    public abstract class BaseSpriteGO
    {
        protected GameObject go;
        
        protected BaseSpriteGO(string path,Vector3 pos,Transform parent=null)
        {
            var res = Resources.Load<GameObject>(path);
            if (res == null)
                Debug.LogError("图片路径错误");
            go = GameObject.Instantiate(res, pos, Quaternion.identity,parent);
            
            Init();
        }

        protected virtual void Update()
        {
            
        }

        public virtual void Init()
        {
	        MainLoop.Instance.AddUpdateFunc(Update);
        }

        public virtual void Release()
        {
            MainLoop.Instance.RemoveUpdateFunc(Update);
        }

        protected virtual void DestoryThis()
        {
	        if (this.go == null)
		        return;
            Release();
            GameObject.Destroy(this.go);
            
        }
    }
    
    
    /////////////////////////////////   道具   //////////////////////////////

    /// <summary>
    /// 抽象道具类
    /// </summary>
    public abstract class AbstractItem
    {
        public string name;
        public int id;
        public string sprite;

        public virtual void Init(string args)
        {
            var strs = args.Split('|');

            Assert.IsTrue(strs.Length>=3);
            this.id = Convert.ToInt32(strs[0].Trim());
            this.name = strs[1].Trim();
            this.sprite = strs[2].Trim();
        }
    }

    /// <summary>
    /// 抽象UI物品
    /// </summary>
    public abstract class AbstractUIItem : AbstractItem
    {
	    #region 
	    
	    private static SerializableDictionary<int,AbstractItem> typeDic=new SerializableDictionary<int, AbstractItem>();

	    public static Type IdToType(int id)
	    {
		    if (typeDic.ContainsKey(id))
			    return typeDic[id].GetType();
		    else
			    throw new Exception("没有这个类型");
	    }
	    
	    #endregion
	    
	   
	    
	    
        public int num;
        public int capcity;
        public string doc;
        public string story;
        public override void Init(string args)
        {
            var strs = args.Split('|');
            Assert.IsTrue(strs.Length==6);
            this.num = 0;
            this.capcity = Convert.ToInt32(strs[3].Trim());
            this.doc = strs[4].Trim();
            this.story = strs[5].Trim();
            base.Init(string.Join("|", strs, 0, 3));

            if (typeDic.ContainsKey(this.id))
	            typeDic.Add(this.id, this);
        }
    }
}


namespace Game.Control
{
    /// <summary>
    /// 人物基类
    /// </summary>
    public abstract class PersonControl
    {
	    [HideInInspector]
	    public GameObject obj;

	    protected PersonControl()
	    {
	    }

	    protected PersonControl(string path)
        {
            if (path == null)
                Debug.Log("预制体路径不对");
            else
                this.obj = Resources.Load<GameObject>(path);
        }
    }
}


namespace Game.View
{
    
    /// <summary>
    /// 窗口基类
    /// </summary>
    public abstract class AbstractWindow
    {
        public Transform m_TransFrom=null;      //位置
        private string m_sResName="";              //资源名
        private bool m_bIsVisible=false;           //是否可见

        /// <summary>
        /// 类对象初始化
        /// 添加对象使用必须的事件监听
        /// Create时调用
        /// </summary>
        protected virtual void InitWindow()
        {
	        MainLoop.Instance.AddUpdateFunc(Update);
        }

        /// <summary>
        /// 类对象释放
        /// Destroy时调用
        /// </summary>
        protected virtual void RealseWindow()
        {
	        MainLoop.Instance.RemoveUpdateFunc(Update);
        }

        /// <summary>
        /// 游戏事件注册
        /// </summary>
        protected virtual void OnAddListener()
        {
        }

        /// <summary>
        /// 游戏事件注销
        /// </summary>
        protected virtual void OnRemoveListener()
        {
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        /// <returns></returns>
        public bool IsVisable()
        {
            return m_bIsVisible;
        }

        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <returns></returns>
        protected void Create(string path)
        {
            m_sResName = path;
            
            if(m_TransFrom)
            {
                Debug.LogError("Window Create Error Exist");
                return ;
            }

            if (string.IsNullOrEmpty(m_sResName))
            {
                Debug.LogError("Windows Create Error ResName is empty");
                return ;
            }
            
            var go = Resources.Load<GameObject>(m_sResName);

            var canvas=Global.CGameObjects.Canvas.transform;
            
            if(null==canvas)
                Debug.Log("画布获取失败");
            
            var obj = Object.Instantiate(go,canvas);

            if (obj == null)
            {
                Debug.LogError("Window Create Error LoadRes WindowName = " + m_sResName);
                return ;
            }

            m_TransFrom = obj.transform;

            m_TransFrom.gameObject.SetActive(false);

            InitWindow();
        }

        /// <summary>
        /// 销毁窗体
        /// </summary>
        public virtual void DestroyThis(PointerEventData eventData=null)
        {
            if(m_TransFrom)
            {
                OnRemoveListener();
                RealseWindow();
                Object.Destroy(m_TransFrom.gameObject);
                m_TransFrom = null;
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if(m_TransFrom&&m_TransFrom.gameObject.activeSelf==false)
            {
                m_TransFrom.gameObject.SetActive(true);
                m_bIsVisible = true;
                OnAddListener();
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            if(m_TransFrom&&m_TransFrom.gameObject.activeSelf==true)
            {
                OnRemoveListener();
            }
            m_bIsVisible = false;
        }
    }
    
    /// /////////////////////    栈型UI设计    ///////////////////
    
    public interface IStackPanel
    {
//        string panelName { get; set; }
		string PanelName { get; }
	    void ToEable();
	    void ToDisable();
	    void ToPop();

    }

    /// <summary>
    /// 可用栈盛放的栈窗口
    /// </summary>
    public abstract class StackWindow : AbstractWindow,IStackPanel
    {

	    /// /////////////////     继承接口      ////////////////////////
	    public string PanelName
	    {
		    get { return this.m_TransFrom.name; }
	    }
        
	    public void ToEable()
	    {
		    this.Show();
	    }

	    public void ToDisable()
	    {
		    this.Hide();
	    }

	    public void ToPop()
	    {
		    this.DestroyThis();
	    }
    }

    
    /// <summary>
    /// 总UI管理者
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
	    public UIManager()
	    {
            
	    }
	    private Stack<IStackPanel> panelStack=new Stack<IStackPanel>();

	    public IStackPanel CurrentPanel
	    {
		    get { return panelStack.Peek(); }
	    }

	    public void PopPanel()
	    {
		    CurrentPanel.ToPop();
		    panelStack.Pop();
		    if(panelStack.Count>0)
			    CurrentPanel.ToEable();
	    }

	    public void PushPanel(IStackPanel panel)
	    {
		    if(panelStack.Count>0)
			    CurrentPanel.ToDisable();
		    panelStack.Push(panel);
		    panel.ToEable();
	    }
    }
}


namespace Game.StateMachine
{
	 /*
	  * Copyright (c) 2016 Made With Mosnter Love (Pty) Ltd
	  * 
	  * Permission is hereby granted, free of charge, to any person obtaining a copy
	  * of this software and associated documentation files (the "Software"), to 
	  * deal in the Software without restriction, including without limitation the 
	  * rights to use, copy, modify, merge, publish, distribute, sublicense, 
	  * and/or sell copies of the Software, and to permit persons to whom the 
	  * Software is furnished to do so, subject to the following conditions:
	  * 
	  * The above copyright notice and this permission notice shall be included 
	  * in all copies or substantial portions of the Software.
	  * 
	  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
	  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
	  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
	  * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
	  * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
	  * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
	  * OTHER DEALINGS IN THE SOFTWARE.
	  */
	public enum StateTransition
	{
		Safe,
		Overwrite
	}

	public interface IStateMachine
	{
		MonoBehaviour Component { get; }
		StateMapping CurrentStateMap { get; }
		bool IsInTransition { get; }
	}

	public class StateMachine<T> : IStateMachine where T : struct, IConvertible, IComparable
	{
		public event Action<T> Changed;

		private StateMachineRunner engine;
		private MonoBehaviour component;

		private StateMapping lastState;
		private StateMapping currentState;
		private StateMapping destinationState;

		private SerializableDictionary<object, StateMapping> stateLookup;

		private readonly string[] ignoredNames = new[] { "add", "remove", "get", "set" };

		private bool isInTransition = false;
		private IEnumerator currentTransition;
		private IEnumerator exitRoutine;
		private IEnumerator enterRoutine;
		private IEnumerator queuedChange;

		public StateMachine(StateMachineRunner engine, MonoBehaviour component)
		{
			this.engine = engine;
			this.component = component;

			//Define States
			var values = Enum.GetValues(typeof(T));
			if (values.Length < 1) { throw new ArgumentException("Enum provided to Initialize must have at least 1 visible definition"); }

			stateLookup = new SerializableDictionary<object, StateMapping>();
			for (int i = 0; i < values.Length; i++)
			{
				var mapping = new StateMapping((Enum) values.GetValue(i));
				stateLookup.Add(mapping.state, mapping);
			}

			//Reflect methods
			var methods = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public |
									  BindingFlags.NonPublic);

			//Bind methods to states
			var separator = "_".ToCharArray();
			for (int i = 0; i < methods.Length; i++)
			{
				if (methods[i].GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length != 0)
				{
					continue;
				}

				var names = methods[i].Name.Split(separator);

				//Ignore functions without an underscore
				if (names.Length <= 1)
				{
					continue;
				}

				Enum key;
				try
				{
					key = (Enum) Enum.Parse(typeof(T), names[0]);
				}
				catch (ArgumentException)
				{
					//Not an method as listed in the state enum
					continue;
				}

				var targetState = stateLookup[key];

				switch (names[1])
				{
					case "Enter":
						if (methods[i].ReturnType == typeof(IEnumerator))
						{
							targetState.hasEnterRoutine = true;
							targetState.EnterRoutine = CreateDelegate<Func<IEnumerator>>(methods[i], component);
						}
						else
						{
							targetState.hasEnterRoutine = false;
							targetState.EnterCall = CreateDelegate<Action>(methods[i], component);
						}
						break;
					case "Exit":
						if (methods[i].ReturnType == typeof(IEnumerator))
						{
							targetState.hasExitRoutine = true;
							targetState.ExitRoutine = CreateDelegate<Func<IEnumerator>>(methods[i], component);
						}
						else
						{
							targetState.hasExitRoutine = false;
							targetState.ExitCall = CreateDelegate<Action>(methods[i], component);
						}
						break;
					case "Finally":
						targetState.Finally = CreateDelegate<Action>(methods[i], component);
						break;
					case "Update":
						targetState.Update = CreateDelegate<Action>(methods[i], component);
						break;
					case "LateUpdate":
						targetState.LateUpdate = CreateDelegate<Action>(methods[i], component);
						break;
					case "FixedUpdate":
						targetState.FixedUpdate = CreateDelegate<Action>(methods[i], component);
						break;
					case "OnCollisionEnter":
						targetState.OnCollisionEnter = CreateDelegate<Action<Collision>>(methods[i], component);
						break;
				}
			}

			//Create nil state mapping
			currentState = new StateMapping(null);
		}

		private V CreateDelegate<V>(MethodInfo method, Object target) where V : class
		{
			var ret = (Delegate.CreateDelegate(typeof(V), target, method) as V);

			if (ret == null)
			{
				throw new ArgumentException("Unabled to create delegate for method called " + method.Name);
			}
			return ret;

		}

		public void ChangeState(T newState)
		{
			ChangeState(newState, StateTransition.Safe);
		}

		public void ChangeState(T newState, StateTransition transition)
		{
			if (stateLookup == null)
			{
				throw new Exception("States have not been configured, please call initialized before trying to set state");
			}

			if (!stateLookup.ContainsKey(newState))
			{
				throw new Exception("No state with the name " + newState.ToString() + " can be found. Please make sure you are called the correct type the statemachine was initialized with");
			}

			var nextState = stateLookup[newState];

			if (currentState == nextState) return;

			//Cancel any queued changes.
			if (queuedChange != null)
			{
				engine.StopCoroutine(queuedChange);
				queuedChange = null;
			}

			switch (transition)
			{
				//case StateMachineTransition.Blend:
				//Do nothing - allows the state transitions to overlap each other. This is a dumb idea, as previous state might trigger new changes. 
				//A better way would be to start the two couroutines at the same time. IE don't wait for exit before starting start.
				//How does this work in terms of overwrite?
				//Is there a way to make this safe, I don't think so? 
				//break;
				case StateTransition.Safe:
					if (isInTransition)
					{
						if (exitRoutine != null) //We are already exiting current state on our way to our previous target state
						{
							//Overwrite with our new target
							destinationState = nextState;
							return;
						}

						if (enterRoutine != null) //We are already entering our previous target state. Need to wait for that to finish and call the exit routine.
						{
							//Damn, I need to test this hard
							queuedChange = WaitForPreviousTransition(nextState);
							engine.StartCoroutine(queuedChange);
							return;
						}
					}
					break;
				case StateTransition.Overwrite:
					if (currentTransition != null)
					{
						engine.StopCoroutine(currentTransition);
					}
					if (exitRoutine != null)
					{
						engine.StopCoroutine(exitRoutine);
					}
					if (enterRoutine != null)
					{
						engine.StopCoroutine(enterRoutine);
					}

					//Note: if we are currently in an EnterRoutine and Exit is also a routine, this will be skipped in ChangeToNewStateRoutine()
					break;
			}


			if ((currentState != null && currentState.hasExitRoutine) || nextState.hasEnterRoutine)
			{
				isInTransition = true;
				currentTransition = ChangeToNewStateRoutine(nextState, transition);
				engine.StartCoroutine(currentTransition);
			}
			else //Same frame transition, no coroutines are present
			{
				if (currentState != null)
				{
					currentState.ExitCall();
					currentState.Finally();
				}

				lastState = currentState;
				currentState = nextState;
				if (currentState != null)
				{
					currentState.EnterCall();
					if (Changed != null)
					{
						Changed((T) currentState.state);
					}
				}
				isInTransition = false;
			}
		}

		private IEnumerator ChangeToNewStateRoutine(StateMapping newState, StateTransition transition)
		{
			destinationState = newState; //Chache this so that we can overwrite it and hijack a transition

			if (currentState != null)
			{
				if (currentState.hasExitRoutine)
				{
					exitRoutine = currentState.ExitRoutine();

					if (exitRoutine != null && transition != StateTransition.Overwrite) //Don't wait for exit if we are overwriting
					{
						yield return engine.StartCoroutine(exitRoutine);
					}

					exitRoutine = null;
				}
				else
				{
					currentState.ExitCall();
				}

				currentState.Finally();
			}

			lastState = currentState;
			currentState = destinationState;

			if (currentState != null)
			{
				if (currentState.hasEnterRoutine)
				{
					enterRoutine = currentState.EnterRoutine();

					if (enterRoutine != null)
					{
						yield return engine.StartCoroutine(enterRoutine);
					}

					enterRoutine = null;
				}
				else
				{
					currentState.EnterCall();
				}

				//Broadcast change only after enter transition has begun. 
				if (Changed != null)
				{
					Changed((T) currentState.state);
				}
			}

			isInTransition = false;
		}

		IEnumerator WaitForPreviousTransition(StateMapping nextState)
		{
			while (isInTransition)
			{
				yield return null;
			}

			ChangeState((T) nextState.state);
		}

		public T LastState
		{
			get
			{
				if (lastState == null) return default(T);

				return (T) lastState.state;
			}
		}

		public T State
		{
			get { return (T) currentState.state; }
		}

		public bool IsInTransition
		{
			get { return isInTransition; }
		}

		public StateMapping CurrentStateMap
		{
			get { return currentState; }
		}

		public MonoBehaviour Component
		{
			get { return component; }
		}

		//Static Methods

		/// <summary>
		/// Inspects a MonoBehaviour for state methods as definied by the supplied Enum, and returns a stateMachine instance used to trasition states.
		/// </summary>
		/// <param name="component">The component with defined state methods</param>
		/// <returns>A valid stateMachine instance to manage MonoBehaviour state transitions</returns>
		public static StateMachine<T> Initialize(MonoBehaviour component)
		{
			var engine = component.GetComponent<StateMachineRunner>();
			if (engine == null) engine = component.gameObject.AddComponent<StateMachineRunner>();

			return engine.Initialize<T>(component);
		}

		/// <summary>
		/// Inspects a MonoBehaviour for state methods as definied by the supplied Enum, and returns a stateMachine instance used to trasition states. 
		/// </summary>
		/// <param name="component">The component with defined state methods</param>
		/// <param name="startState">The default starting state</param>
		/// <returns>A valid stateMachine instance to manage MonoBehaviour state transitions</returns>
		public static StateMachine<T> Initialize(MonoBehaviour component, T startState)
		{
			var engine = component.GetComponent<StateMachineRunner>();
			if (engine == null) engine = component.gameObject.AddComponent<StateMachineRunner>();

			return engine.Initialize<T>(component, startState);
		}

	}
	
	public class StateMachineRunner : MonoBehaviour
	{
		private List<IStateMachine> stateMachineList = new List<IStateMachine>();

		/// <summary>
		/// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. 
		/// </summary>
		/// <typeparam name="T">An Enum listing different state transitions</typeparam>
		/// <param name="component">The component whose state will be managed</param>
		/// <returns></returns>
		public StateMachine<T> Initialize<T>(MonoBehaviour component) where T : struct, IConvertible, IComparable
		{
			var fsm = new StateMachine<T>(this, component);

			stateMachineList.Add(fsm);

			return fsm;
		}

		/// <summary>
		/// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. Will automatically transition the startState
		/// </summary>
		/// <typeparam name="T">An Enum listing different state transitions</typeparam>
		/// <param name="component">The component whose state will be managed</param>
		/// <param name="startState">The default start state</param>
		/// <returns></returns>
		public StateMachine<T> Initialize<T>(MonoBehaviour component, T startState) where T : struct, IConvertible, IComparable
		{
			var fsm = Initialize<T>(component);

			fsm.ChangeState(startState);

			return fsm;
		}

		void FixedUpdate()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if(!fsm.IsInTransition && fsm.Component.enabled) fsm.CurrentStateMap.FixedUpdate();
			}
		}

		void Update()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if (!fsm.IsInTransition && fsm.Component.enabled)
				{
					fsm.CurrentStateMap.Update();
				}
			}
		}

		void LateUpdate()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if (!fsm.IsInTransition && fsm.Component.enabled)
				{
					fsm.CurrentStateMap.LateUpdate();
				}
			}
		}

		//void OnCollisionEnter(Collision collision)
		//{
		//	if(currentState != null && !IsInTransition)
		//	{
		//		currentState.OnCollisionEnter(collision);
		//	}
		//}

		public static void DoNothing()
		{
		}

		public static void DoNothingCollider(Collider other)
		{
		}

		public static void DoNothingCollision(Collision other)
		{
		}

		public static IEnumerator DoNothingCoroutine()
		{
			yield break;
		}
	}

	public class StateMapping
	{
		public object state;

		public bool hasEnterRoutine;
		public Action EnterCall = StateMachineRunner.DoNothing;
		public Func<IEnumerator> EnterRoutine = StateMachineRunner.DoNothingCoroutine;

		public bool hasExitRoutine;
		public Action ExitCall = StateMachineRunner.DoNothing;
		public Func<IEnumerator> ExitRoutine = StateMachineRunner.DoNothingCoroutine;

		public Action Finally = StateMachineRunner.DoNothing;
		public Action Update = StateMachineRunner.DoNothing;
		public Action LateUpdate = StateMachineRunner.DoNothing;
		public Action FixedUpdate = StateMachineRunner.DoNothing;
		public Action<Collision> OnCollisionEnter = StateMachineRunner.DoNothingCollision;

		public StateMapping(object state)
		{
			this.state = state;
		}

	}

}


namespace Game.Script
{
	public class AudioMgr : MonoSingleton<AudioMgr> 
	{
		private SerializableDictionary<string,AudioClip> audioclips=new SerializableDictionary<string, AudioClip>();
		private AudioSource audioBg;
		private AudioSource audioEffect;

		public float BgSound
		{
			get { return audioBg.volume; }
			set { audioBg.volume = value; }
		}

		public float EffectVolume
		{
			get { return audioEffect.volume; }
			set { audioEffect.volume = value; }
		}
	
		protected override void Awake()
		{
			base.Awake();
			//实现从文件夹读取所有文件
			DirectoryInfo source = new DirectoryInfo(Application.dataPath+"//Resources/Audio");
			foreach (FileInfo diSourceSubDir in source.GetFiles())
			{
				if(diSourceSubDir.Name.EndsWith(".meta"))
					continue;
				var strs = diSourceSubDir.Name.Split('.');
				var res = Resources.Load<AudioClip>("Audio/" + strs[0]);
				if(res==null)
					print("资源加载失败");
				audioclips.Add(strs[0], res);
			}
		
		
			//加载组件
			this.audioBg=this.gameObject.AddComponent<AudioSource>();
			this.audioBg.loop = true;
			this.audioBg.playOnAwake = true;
		
			this.audioEffect = this.gameObject.AddComponent<AudioSource>();
			this.audioBg.loop = false;
			this.audioBg.playOnAwake = false;



		}

		void Start()
		{
			PlayAuBg("Bg");
			this.audioBg.volume = 0f;
		}
	
	

		public void PlayAuBg(string name,float delayTime=0)
		{
			if(!audioclips.ContainsKey(name))
				throw new Exception("没有这个音频文件");
			this.audioBg.clip = audioclips[name];
			this.audioBg.PlayDelayed(delayTime);
		}

		public void PlayAuEffect(string name,float delayTime=0)
		{
			if(!audioclips.ContainsKey(name))
				throw new Exception("没有这个音频文件");
			this.audioEffect.clip = audioclips[name];
			this.audioEffect.PlayDelayed(delayTime);
		}

		public void PlayAuEffect(string name, Vector3 pos)
		{
			if(!audioclips.ContainsKey(name))
				throw new Exception("没有这个音频文件");
			AudioSource.PlayClipAtPoint(this.audioclips[name], pos);
		}
	}
	public class MainLoop : MonoSingleton<MainLoop>
	{

		#region 协程（延时调用，间隔调用，一段时间内每帧调用）

		/// <summary>
		/// 开始关闭协程
		/// </summary>
		/// <param name="Coroutine"></param>
		/// <returns></returns>
		public Coroutine StartCoroutines(IEnumerator Coroutine)
		{
			return StartCoroutine(Coroutine);
		}

		public void StopCoroutines(Coroutine Coroutine)
		{
			StopCoroutine(Coroutine);
		}


		/// <summary>
		/// 运行直到为真
		/// </summary>
		/// <param name="method"></param>
		/// <param name="endCall"></param>
		/// <returns></returns>
		public Coroutine ExecuteUntilTrue(Func<bool> method,Action endCall=null)
		{
			return  StartCoroutine(_ExecuteUntilTrue(method,endCall));
		}


		
		/// <summary>
		/// 延时调用
		/// </summary>
		/// <param name="method"></param>          c
		/// <param name="seconds"></param>
		public Coroutine ExecuteLater(Action method, float seconds)
		{
			return StartCoroutine(_ExecuteLater(method, seconds));
		}

		public Coroutine ExecuteLater<T>(Action<T> method, float seconds, T args)
		{
			return StartCoroutine(_ExecuteLater_T<T>(method, seconds, args));
		}

		/// <summary>
		/// 间隔调用
		/// </summary>
		/// <param name="method"></param>
		/// <param name="times"></param>
		/// <param name="duringTime"></param>
		public Coroutine ExecuteEverySeconds(Action method, float times, float duringTime)
		{
			return StartCoroutine(_ExecuteSeconds(method, times, duringTime));
		}

		public Coroutine ExecuteEverySeconds<T>(Action<T> mathdom, float times, float duringTime, T args)
		{
			return StartCoroutine(_ExecuteSeconds_T(mathdom, times, duringTime, args));
		}

		public Coroutine ExecuteEverySeconds(Action method, float times, float duringTime, Action endCall)
		{
			return StartCoroutine(_ExecuteSeconds_Action(method, times, duringTime, endCall));
		}
		
		public Coroutine ExecuteEverySeconds<T>(Action<T> method, float times, float duringTime,T args, Action<T> endCall)
		{
			return StartCoroutine(_ExecuteSeconds_Action_T(method, times, duringTime,args,endCall));
		}

		/// <summary>
		/// 一段时间内每帧调用
		/// </summary>
		/// <param name="method"></param>
		/// <param name="seconds"></param>
		public Coroutine UpdateForSeconds(Action method, float seconds, Action endCall)
		{
			return StartCoroutine(_UpdateForSeconds_Action(method, seconds, endCall));
		}

		public Coroutine UpdateForSeconds<T>(Action<T> method, float seconds, T arg, Action<T> endCall)
		{
			return StartCoroutine(_UpdateForSeconds_Action_T(method, seconds, arg, endCall));
		}

		public Coroutine UpdateForSeconds(Action method, float seconds, float start = 0f)
		{
			return StartCoroutine(_UpdateForSeconds(method, seconds, start));
		}

		public Coroutine UpdateForSeconds<T>(Action<T> method, float seconds, T args, float start = 0f)
		{
			return StartCoroutine(_UpdateForSeconds_T(method, seconds, args, start));
		}

		#region 内部调用

		private IEnumerator _ExecuteLater(Action mathdem, float time)
		{
			yield return new WaitForSeconds(time);
			mathdem();
		}

		private IEnumerator _ExecuteLater_T<T>(Action<T> mathdom, float time, T args)
		{
			yield return new WaitForSeconds(time);
			mathdom(args);
		}

		private IEnumerator _ExecuteSeconds(Action mathdom, float times, float duringTime)
		{
			for (int i = 0; i < times; i++)
			{
				for (var timer = 0f; timer < duringTime; timer += Time.deltaTime)
					yield return 0;
				mathdom();
			}
		}

		private IEnumerator _ExecuteSeconds_T<T>(Action<T> mathdom, float times, float duringTime, T args)
		{
			for (int i = 0; i < times; i++)
			{
				for (var timer = 0f; timer < duringTime; timer += Time.deltaTime)
					yield return 0;
				mathdom(args);
			}
		}

		private IEnumerator _ExecuteSeconds_Action(Action method, float times, float dur, Action endCall)
		{
			for (int i = 0; i < times; i++)
			{
				for (var timer = 0f; timer < dur; timer += Time.deltaTime)
					yield return 0;
				method();
			}

			endCall();
		}
		private IEnumerator _ExecuteSeconds_Action_T<T>(Action<T> method, float times, float dur, T args,Action<T> endCall)
		{
			for (int i = 0; i < times; i++)
			{
				for (var timer = 0f; timer < dur; timer += Time.deltaTime)
					yield return 0;
				method(args);
			}

			endCall(args);
		}

		private IEnumerator _UpdateForSeconds(Action mathdom, float seconds, float start)
		{
			for (var d = 0f; d < start; d += Time.deltaTime)
				yield return 0;
			for (var timer = 0f; timer < seconds; timer += Time.deltaTime)
			{
				yield return 0;
				mathdom();
			}
		}

		private IEnumerator _UpdateForSeconds_T<T>(Action<T> mathdom, float seconds, T args, float start)
		{
			for (var d = 0f; d < start; d += Time.deltaTime)
				yield return 0;
			for (var timer = 0f; timer < seconds; timer += Time.deltaTime)
			{
				yield return 0;
				mathdom(args);
			}

		}

		private IEnumerator _UpdateForSeconds_Action(Action method, float time, Action endcall)
		{
			for (var timer = 0f; timer < time; timer += Time.deltaTime)
			{
				yield return 0;
				method();
			}

			yield return 0;
			endcall();
		}


		private IEnumerator _UpdateForSeconds_Action_T<T>(Action<T> method, float seconds, T arg, Action<T> endCall)
		{
			for (var timer = 0f; timer < seconds; timer += Time.deltaTime)
			{
				yield return 0;
				method(arg);
			}

			yield return 0;
			endCall(arg);
		}


		private IEnumerator _ExecuteUntilTrue(Func<bool> method,Action endCall)
		{
			while (true)
			{
				if (method())
				{
					endCall();
					break;
				}
				yield return 0;
			}
		}
		#endregion


		#endregion

		private event Action updateEvent;
		private event Action guiEvent;
		private event Action startEvent;

		void Start()
		{
			if (startEvent != null)
				startEvent();
		}

		void Update()
		{
			if (updateEvent != null)
				updateEvent();
		}

		void OnGUI()
		{
			if (guiEvent != null)
				guiEvent();
		}

		public void AddStartFunc(Action func)
		{
			startEvent += func;
		}

		public void RemoveStartFunc(Action func)
		{
			startEvent -= func;
		}


		public void AddUpdateFunc(Action func)
		{
			updateEvent += func;
		}

		public void RemoveUpdateFunc(Action func)
		{
			updateEvent -= func;

		}

		public void AddGUIFunc(Action func)
		{
			guiEvent += func;
		}

		public void RemoveGUIFunc(Action func)
		{
			guiEvent -= func;
		}



	}

	public class TriggerEvent : MonoBehaviour
	{

		public event Action<Collider2D> onTriggerEnterEvent;//如果TriggerEnter会调用这个event
		public event Action<Collider2D> onTriggerStayEvent;//如果TriggerStay会调用这个event
		public event Action<Collider2D> onTriggerExitEvent;//同理
	
		private void OnTriggerEnter2D( Collider2D col )
		{
			if( onTriggerEnterEvent != null )
				onTriggerEnterEvent( col );
		}


		private void OnTriggerStay2D( Collider2D col )
		{
			if( onTriggerStayEvent != null )
				onTriggerStayEvent( col );
		}


		private void OnTriggerExit2D( Collider2D col )
		{
			if( onTriggerExitEvent != null )
				onTriggerExitEvent( col );
		}


	}
	
	public class UIInputer : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerUpHandler,IPointerDownHandler,IPointerClickHandler
	{
		public event Action<PointerEventData> FunOnPointerEnter;
		public event Action<PointerEventData> FunOnPointerExit;
		public event Action<PointerEventData> FunOnPointerUp;
		public event Action<PointerEventData> FunOnPointerDown;
		public event Action<PointerEventData> FunOnPointerClick;


		public void OnPointerEnter(PointerEventData eventData)
		{
			if (FunOnPointerEnter == null) return;
			FunOnPointerEnter(eventData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (FunOnPointerExit == null) return;
			FunOnPointerExit(eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (FunOnPointerUp == null) return;
			FunOnPointerUp(eventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (FunOnPointerDown == null) return;
			FunOnPointerDown(eventData);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (FunOnPointerClick == null) return;
			FunOnPointerClick(eventData);
		}
	}

}


namespace Game.Serialization
{
	/// <summary>
	/// 可序列化字典
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[System.Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable,
		ISerializationCallbackReceiver
	{

		#region 构造函数
		
		public SerializableDictionary()
			: base()
		{
		}

		public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
			: base(dictionary)
		{
		}

		public SerializableDictionary(IEqualityComparer<TKey> comparer)
			: base(comparer)
		{
		}

		public SerializableDictionary(int capacity)
			: base(capacity)
		{
		}

		public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
			: base(capacity, comparer)
		{
		}

		protected SerializableDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion

		#region  IXmlSerializable
		
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>  
		/// 从对象的 XML 表示形式生成该对象  
		/// </summary>  
		/// <param name="reader"></param>  
		public void ReadXml(System.Xml.XmlReader reader)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
			bool wasEmpty = reader.IsEmptyElement;
			reader.Read();
			if (wasEmpty)
				return;
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");
				reader.ReadStartElement("key");
				TKey key = (TKey) keySerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement("value");
				TValue value = (TValue) valueSerializer.Deserialize(reader);
				reader.ReadEndElement();
				this.Add(key, value);
				reader.ReadEndElement();
				reader.MoveToContent();
			}

			reader.ReadEndElement();
		}

		/// <summary>  
		/// 将对象转换为其 XML 表示形式  
		/// </summary>  
		/// <param name="writer"></param>  
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
			foreach (TKey key in this.Keys)
			{
				writer.WriteStartElement("item");
				writer.WriteStartElement("key");
				keySerializer.Serialize(writer, key);
				writer.WriteEndElement();
				writer.WriteStartElement("value");
				TValue value = this[key];
				valueSerializer.Serialize(writer, value);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

		#endregion

		#region ISerializationCallbackReceiver
		
		[SerializeField] private List<TKey> _keys = new List<TKey>();
		[SerializeField] private List<TValue> _values = new List<TValue>();
		public void OnBeforeSerialize()
		{
			_keys.Clear();
			_values.Clear();
			_keys.Capacity = this.Count;
			_values.Capacity = this.Count;
			foreach (var kvp in this)
			{
				_keys.Add(kvp.Key);
				_values.Add(kvp.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			this.Clear();
			int count = Mathf.Min(_keys.Count, _values.Count);
			for (int i = 0; i < count; ++i)
			{
				this.Add(_keys[i], _values[i]);

			}
		}
		
		#endregion
	}

	/// <summary>
	/// Xml序列化管理类
	/// </summary>
	public static class XmlManager 
	{
		public static T LoadData<T>(string fullPath)where T:class
		{
			StreamReader r  = File.OpenText(fullPath);//_FileLocation是unity3D当前project的路径名，_FileName是xml的文件名。定义为成员变量了
			//当然，你也可以在前面先判断下要读取的xml文件是否存在
			String _data=r.ReadLine();
//			Debug.Log(_data);
			var myData = DeserializeObject<T>(_data);//myData是上面自定义的xml存取过程中要使用的数据结构UserData
			r.Close();
			return myData;
		}

		public static void SaveData<T>(T data,string fullPath)where T:class
		{
			StreamWriter writer;
			FileInfo t = new FileInfo(fullPath);
			t.Delete();
			writer = t.CreateText();
			String _data = SerializeObject(data); //序列化这组数据
			writer.WriteLine(_data); //写入xml
//			writer.WriteLine(_data); //写入xml
			writer.Close();
		}

		#region Xml底层
		private static String UTF8ByteArrayToString(byte []characters)
		{
			UTF8Encoding encoding = new UTF8Encoding();
			String constructedString = encoding.GetString(characters);
			return (constructedString);
		}
 
		private static byte[] StringToUTF8ByteArray(String pXmlString)
		{
			UTF8Encoding encoding = new UTF8Encoding();
			byte []byteArray = encoding.GetBytes(pXmlString);
			return byteArray;
		}
 
		// Here we serialize our UserData object of myData
		private static String SerializeObject<T>(T pObject)where T:class
		{
			String XmlizedString = "";
			MemoryStream memoryStream = new MemoryStream();
			XmlSerializer xs = new XmlSerializer(typeof(T));
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
			xs.Serialize(xmlTextWriter, pObject);
			memoryStream = (MemoryStream)xmlTextWriter.BaseStream; // (MemoryStream)
			XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
			return XmlizedString;
		}
 
		// Here we deserialize it back into its original form
		private static T DeserializeObject<T>(String pXmlizedString)where T:class
		{
			XmlSerializer xs = new XmlSerializer(typeof(T));
			MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
			return xs.Deserialize(memoryStream) as T;
		}
		
		#endregion
	}
}



//private static string ExportedFilePath
//{
//get
//{
//	string relativePath = "/Scripts/behaviac/exported";
//
//	return Application.dataPath + relativePath;
//}
//}
//
//private bool InitBehavic()
//{
//Debug.Log("InitBehavic");
//behaviac.Workspace.Instance.FilePath = ExportedFilePath;
//behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;
//return true;
//}
//
//private bool InitPlayer()
//{
//Debug.Log("InitPlayer");
//bool bRet = this.btload("FirstBT");
//
//	if (bRet)
//{
//	this.btsetcurrent("FirstBT");
//}
//
//return bRet;
//}
//
//void Awake()
//{
//InitBehavic();
//InitPlayer();
//}
//
//behaviac.EBTStatus _status = behaviac.EBTStatus.BT_RUNNING;
//	
//void Update()
//{
//if (_status == behaviac.EBTStatus.BT_RUNNING)
//{
//behaviac.Debug.LogWarning("Update");
//
//_status = this.btexec();
//}
//}
