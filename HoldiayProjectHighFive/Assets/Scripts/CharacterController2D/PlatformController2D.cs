using System.Collections.Generic;
using UnityEngine;
using zoroiscrying;

public class PlatformController2D : RayCastController2D
{
	public Vector3 move;
	public LayerMask passengerMask;
	public bool cyclic;
	public float waitTime;
	[Range(0,2)]
	public float easeAmount;
	public Vector3[] localWayPoints;

	public float speed;

	private float nextMoveTime; 
	private Vector3[] globalWayPoints;
	private float percentBetweenWayPoints;
	private int fromWayPointIndex = 0;
	public List<PassengerMovement> passengerMovement = new List<PassengerMovement>();
	public Dictionary<Transform, CharacterController2D> passengerDictionary = new Dictionary<Transform, CharacterController2D>();
	
	public override void Awake()
	{
		base.Awake();
		
		globalWayPoints = new Vector3[localWayPoints.Length];
		for (int i = 0; i < localWayPoints.Length; i++)
		{
			globalWayPoints[i] = localWayPoints[i] + transform.position;
		}
		
	}

	private void Update()
	{
		PrimeRaycastOrigins();
		
		Vector3 velocity = CalculatePlatformMovement();
		
		CalculatePassengerMovement(velocity);
		
		MovePassengers(true);
		transform.Translate(velocity.x,velocity.y,velocity.z);//platformMoving
		MovePassengers(false);
		
		//horizontal
		for (int i = 0; i < totalHorizontalRays; i++)
		{
			Vector2 rayOrigin =  _raycastOrigins.bottomLeft;
			rayOrigin += Vector2.up * _verticalDistanceBetweenRays *i;
			Debug.DrawRay( rayOrigin, Vector2.up, Color.red );                                                                                             	
		}
		for (int i = 0; i < totalHorizontalRays; i++)
		{
			Vector2 rayOrigin = _raycastOrigins.bottomRight ;
			rayOrigin += Vector2.up * _verticalDistanceBetweenRays *i;
			Debug.DrawRay( rayOrigin, Vector2.down, Color.red );                                                                                                 	
		}
		//vertical
		for (int i = 0; i < totalVerticalRays; i++)
		{
			Vector2 rayOrigin =  _raycastOrigins.topLeft;
			rayOrigin += Vector2.right * _horizontalDistanceBetweenRays *i;
			Debug.DrawRay( rayOrigin, Vector2.up, Color.red );                                                                                             	
		}
		for (int i = 0; i < totalVerticalRays; i++)
		{
			Vector2 rayOrigin = _raycastOrigins.bottomLeft ;
			rayOrigin += Vector2.right * _horizontalDistanceBetweenRays *i;
			Debug.DrawRay( rayOrigin, Vector2.down, Color.red );                                                                                                 	
		}
		
	}

	float Ease(float x)
	{
		float a = easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));//magic!
	}
	
	Vector3 CalculatePlatformMovement()
	{
		if (Time.time < nextMoveTime)
		{
			return Vector3.zero;
		}
		
		fromWayPointIndex %= globalWayPoints.Length;
		int toWayPointIndex = (fromWayPointIndex + 1) % globalWayPoints.Length;
		float distanceBetweenWayPoints =
			Vector3.Distance(globalWayPoints[fromWayPointIndex], globalWayPoints[toWayPointIndex]);
		percentBetweenWayPoints += Time.deltaTime * speed / distanceBetweenWayPoints;
		percentBetweenWayPoints = Mathf.Clamp01(percentBetweenWayPoints);
		float easedPercentBetweenWayPoints = Ease(percentBetweenWayPoints);
		
		Vector3 newPos = Vector3.Lerp(globalWayPoints[fromWayPointIndex], globalWayPoints[toWayPointIndex],
			easedPercentBetweenWayPoints);

		if (percentBetweenWayPoints >= 1)
		{
			percentBetweenWayPoints = 0;
			fromWayPointIndex++;

			if (!cyclic)
			{
				if (fromWayPointIndex >= globalWayPoints.Length - 1)
				{
					fromWayPointIndex = 0;
					System.Array.Reverse(globalWayPoints);
				}
			}

			nextMoveTime = Time.time + waitTime;
		}
		return newPos - transform.position;//因为速度是相对的，所以距离也换成相对的，这里其实是deltaMovement,这一帧移动的距离
	}
	
	void MovePassengers(bool beforeMovePlatform)
	{
		foreach (var passenger in passengerMovement)
		{
			if (!passengerDictionary.ContainsKey(passenger.transform))
			{
				passengerDictionary.Add(passenger.transform,passenger.transform.GetComponent<CharacterController2D>());
			}

			if (passenger.moveBeforePlatform == beforeMovePlatform)
			{
				passengerDictionary[passenger.transform].Move(passenger.velocity,passenger.standingOnPlatform);
			}
		}
	}
	
	void CalculatePassengerMovement(Vector3 velocity)
	{
		HashSet<Transform> movedPassengers = new HashSet<Transform>();
		passengerMovement = new List<PassengerMovement>();
		
		float directionX = Mathf.Sign(velocity.x);
		float directionY = Mathf.Sign(velocity.y);
		
		//if a passenger is on a horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
		{
			float rayLength = 2 * skinWidth;

			for (int i = 0; i < totalVerticalRays; i++)
			{
				Vector2 rayOrigin = _raycastOrigins.topLeft;
				rayOrigin += Vector2.right * _horizontalDistanceBetweenRays *i;
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up , rayLength, passengerMask);
				if (hit&& hit.distance != 0)
				{
					if (!movedPassengers.Contains(hit.transform))
					{
						float pushY = velocity.y ;
						float pushX = velocity.x ;//if the passenger is standing on the platform

						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY),true,false));
						movedPassengers.Add(hit.transform);
					}
				}                                                                                                 	
			}
		}
		
		//vertically moving platform
		if (velocity.y != 0 && directionY == 1)
		{
			float rayLength = Mathf.Abs(velocity.y) + skinWidth;

			for (int i = 0; i < totalVerticalRays; i++)
			{
				Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
				rayOrigin += Vector2.right * _horizontalDistanceBetweenRays *i;
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
				if (hit&& hit.distance != 0)//&& hit.distance != 0
				{
					if (!movedPassengers.Contains(hit.transform))
					{
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
						float pushX = (directionY == 1) ? velocity.x : 0;//if the passenger is standing on the platform

						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY),directionY==1,true));
						movedPassengers.Add(hit.transform);
					}
				}                                                                                                 	
			}
		}
		//horizontally moving platform
		if (velocity.x != 0)
		{
			float rayLength = Mathf.Abs(velocity.x) + skinWidth;

			for (int i = 0; i < totalHorizontalRays; i++)
			{
				Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * _verticalDistanceBetweenRays *i;
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);
				
				if (hit&&hit.distance != 0)
				{
					if (!movedPassengers.Contains(hit.transform))
					{
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;
						
						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY),false,true));
						movedPassengers.Add(hit.transform);
					}
				}                                                                                                 	
			}
		}
	}

	public struct PassengerMovement
	{
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;

		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform,
			bool _moveBeforePlatform)
		{
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}
	}

	private void OnDrawGizmos()
	{
		if (localWayPoints != null)
		{
			Gizmos.color = Color.green;
			float size = .3f;
			for (int i = 0; i < localWayPoints.Length; i++)
			{
				Vector3 globalWayPointPosition =
					(Application.isPlaying) ? globalWayPoints[i] : localWayPoints[i] + transform.position;
				Gizmos.DrawLine(globalWayPointPosition-Vector3.up*size,globalWayPointPosition+Vector3.up*size);
				Gizmos.DrawLine(globalWayPointPosition-Vector3.left*size,globalWayPointPosition+Vector3.left*size);
			}
		}
	}
}
