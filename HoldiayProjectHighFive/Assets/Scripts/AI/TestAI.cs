using System.Collections;
using System.Collections.Generic;
using Game.Control;
using Game.StateMachine;
using UnityEngine;
using Game.Global;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine.Networking;

public class TestAI : Actor {

	public enum EnemyState
	{
		Run,
		Idle,
		Chase,
		Dash,
		Jump
	}

	public float horizentalTryDis;
	public float awakeDis;
	public float shootDis;
	public float fuckDis;
	public float dashDis;

	private StateMachine<EnemyState> stateMachine;
	private float disWithPlayer;

	private Player player;
	private TestPerson self;
	public override void Awake()
	{
		base.Awake();
		player = AbstractPerson.GetInstance<Player>(CGameObjects.Player);
		self = AbstractPerson.GetInstance<TestPerson>(gameObject);
		this.stateMachine = StateMachine<EnemyState>.Initialize(this, EnemyState.Run);
	}

	
	// Update is called once per frame
	public override void Update()
	{
		base.Update();
		disWithPlayer = Vector3.Distance(transform.position, player.Pos);
		BasicChangeCheck();
	}

	private void BasicChangeCheck()
	{
		if (stateMachine.State == EnemyState.Run && GetRayHit(horizentalTryDis).Length!=0)
		{
			//掉头
			_controller.velocity.x *= -1;
		}

		if (disWithPlayer < this.awakeDis)
		{
			stateMachine.ChangeState(EnemyState.Chase);
		}
	}

	private RaycastHit2D[] GetRayHit(float dis)
	{
		var ori = new Vector2(transform.position.x, transform.position.y + 0.2f);
		var dir = new Vector2(_faceDir, 0);
		return Physics2D.RaycastAll(ori, dir,dis,_controller.platformMask);

	}
	
	#region CallBackFunc
	
	#region Run

	private float runTimer=0;
	[Range(0, 1f)] public float run2Idle;
	private void Run_Enter()
	{
		_animator.Play(Animator.StringToHash("Move"));

	}

	private void Run_Update()
	{
		runTimer += Time.deltaTime;
		if (runTimer == 5)
		{
			if (Random.Range(0, 1f) < run2Idle)
				stateMachine.ChangeState(EnemyState.Idle);
			runTimer = 0;
		}
	}
	
	#endregion
	
	#region Idle

	private float idleTimer = 0;
	public float idle2Run;
	private void Idle_Enter()
	{
		_animator.Play(Animator.StringToHash("Idle"));
	}

	private void Idle_Update()
	{
		idleTimer += Time.deltaTime;
		if (idleTimer >= idle2Run)
		{
			idleTimer = 0;
			stateMachine.ChangeState(EnemyState.Run);
		}
	}
	
	#endregion
	
	#region dash
	
	public float _dashTime = .5f;
	public float _dashDistance = 8f;
	private float _dashTimer;
	private int _dashDirX;
	private float _dashVelocity;
	private void Dash_Enter()
	{	
		//Debug.Log("Dash Begin");
		_animator.Play(Animator.StringToHash("Dash"));
		_dashTimer = _dashTime;
		_dashVelocity = _dashDistance / _dashTime;
		_dashDirX = _faceDir;
	}
	
	private void Dash_Update()
	{
		if (_dashDirX == 1 && _controller.collisionState.right)
		{
			_dashVelocity = 0;
		}
		else if (_dashDirX == -1 && _controller.collisionState.left)
		{
			_dashVelocity = 0;
		}
			
		if (_dashTimer > 0)
		{
			_dashTimer -= Time.deltaTime;
			_velocity.x = _dashVelocity * _dashDirX;
		}
		else
		{
			stateMachine.ChangeState(EnemyState.Chase);
		}	
	}
	
	#endregion
	
	#region Jump
	
	public float _minJumpHeight = .25f;
	public int _jumpPoint = 2;
	private int _canJump = 2;
	private float _minJumpVelocity;
	private void Jump_Enter()
	{
		_animator.Play(Animator.StringToHash("Jump"));
		_controller.collisionState.below = false;
	}
	
	private void Jump_Update()
	{
		if (_controller.isGrounded)
		{
			_canJump = _jumpPoint;
			stateMachine.ChangeState(EnemyState.Chase);
		}
	}

	#endregion
	
	#region Chase

	private void Chase_Enter()
	{
		_animator.Play(Animator.StringToHash("Move"));

	}
	private void Chase_Update()
	{
		//转向
		if((player.Pos.x-self.Pos.x)*self.Dir<0)
		{
			_controller.velocity *= -1;
		}

		//跳跃
		if (GetRayHit(this.horizentalTryDis).Length != 0)
		{
			_controller.velocity.x = self.Dir * 1;
			stateMachine.ChangeState(EnemyState.Jump);
		}
		
		//射击
		var hits=GetRayHit(this.shootDis);
		var flag = AbstractPerson.GetInstance<Player>(hits[0].transform.gameObject);
		if (this.disWithPlayer < this.shootDis && flag != null)
			self.RunSkill("H_Skill");

		//刀砍
		if (this.disWithPlayer <= this.fuckDis)
			self.RunSkill("O_Skill",true);

		//冲刺  &&  Idle
		if (disWithPlayer >= this.dashDis)
		{
			if (self.Hp / (float) self.MaxHp > .7f)
				self.RunSkill("L_Skill");
			else
				stateMachine.ChangeState(EnemyState.Run);
		}
	}
	
	
	#endregion
	
	#endregion
}
