using Game.Control.PersonSystem;
using UnityEngine;

public class TempAI : MonoBehaviour
{
	private AbstractPerson self;

	public float attackDistance;

	public float moveSpeed;
	// Use this for initialization
	void Start ()
	{
		self = AbstractPerson.GetInstance(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
