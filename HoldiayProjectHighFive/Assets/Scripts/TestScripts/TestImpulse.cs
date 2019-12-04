using Cinemachine;
using UnityEngine;

public class TestImpulse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P))
		{
			Debug.Log("NIUBI");
			this.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
		}
	}
}
