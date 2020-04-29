using System;
using UnityEngine;


/// <summary>
/// 根据Collider，计算出raycast的起点，用于碰撞检测。
/// </summary>
[RequireComponent( typeof( BoxCollider2D ) )]
public class RayCastController2D : MonoBehaviour {

    [SerializeField]
    [Range( 0.001f, 0.3f )]
    private float _skinWidth = 0.02f;
    public float skinWidth
    {
        get { return _skinWidth; }
        set
        {
            _skinWidth = value;
            RecalculateDistanceBetweenRays();//重新设置skinWidth后需要重新计算rayCast
        }
    }
    
    [Range( 2, 20 )]
    public int totalHorizontalRays = 8;
    [Range( 2, 20 )]
    public int totalVerticalRays = 4;
    
    [HideInInspector]
    public float _verticalDistanceBetweenRays;
    [HideInInspector]
    public float _horizontalDistanceBetweenRays;
    
    public struct CharacterRaycastOrigins //角色的检测射线
    {
        public Vector2 topLeft;
        public Vector2 bottomRight;
        public Vector2 bottomLeft;
        public Vector2 topRight;
    }
    public CharacterRaycastOrigins _raycastOrigins;
    
    [HideInInspector][NonSerialized]
    public BoxCollider2D boxCollider;
    
    // we use this flag to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
    // the reason is so that if we reach the end of the slope we can make an adjustment to stay grounded
    [HideInInspector]
    public bool _isGoingUpWall = false;
    
    public void RecalculateDistanceBetweenRays()//高度和宽度考虑到colliderSize * localScale - 2个皮肤宽度
    {
        // figure out the distance between our rays in both directions
        // horizontal
        var colliderUseableHeight = boxCollider.size.y * Mathf.Abs( transform.localScale.y ) - ( 2f * _skinWidth );
        _verticalDistanceBetweenRays = colliderUseableHeight / ( totalHorizontalRays - 1 );

        // vertical
        var colliderUseableWidth = boxCollider.size.x * Mathf.Abs( transform.localScale.x ) - ( 2f * _skinWidth );
        _horizontalDistanceBetweenRays = colliderUseableWidth / ( totalVerticalRays - 1 );
    }

    public void PrimeRaycastOrigins()
    {
        // our raycasts need to be fired from the bounds inset by the skinWidth
        var modifiedBounds = boxCollider.bounds;
        modifiedBounds.Expand( -2f * _skinWidth );

        _raycastOrigins.topLeft = new Vector2( modifiedBounds.min.x, modifiedBounds.max.y );
        _raycastOrigins.bottomRight = new Vector2( modifiedBounds.max.x, modifiedBounds.min.y );
        _raycastOrigins.bottomLeft = new Vector2(modifiedBounds.min.x,modifiedBounds.min.y);
        _raycastOrigins.topRight = new Vector2(modifiedBounds.max.x,modifiedBounds.max.y);
		
    }
    
    [System.Diagnostics.Conditional( "DEBUG_CC2D_RAYS" )]
    public void DrawRay()
    {
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

    [System.Diagnostics.Conditional("DEBUG_CC2D_RAYS")]
    public void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        Debug.DrawRay(start,dir,color);
    }
    
    public virtual void Awake()
    {
        
        // add our one-way platforms to our normal platform mask so that we can land on them from above

        // cache some components
        boxCollider = GetComponent<BoxCollider2D>();
        PrimeRaycastOrigins();
        // here, we trigger our properties that have setters with bodies
        skinWidth = _skinWidth;
    }

    public virtual void Update()
    {
        //horizontal
        DrawRay();
        //vertical
    }

}
