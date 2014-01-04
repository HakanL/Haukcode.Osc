using UnityEngine;
using System.Collections;

public class OscPositionSender : SendOscBehaviourBase {
	
	private Vector3 m_LastPosition; 

	public string OscAddress = "/test";
	
	// Use this for initialization
	public override void Start () {
		m_LastPosition = this.gameObject.transform.position; 
	}
	
	// Update is called once per frame
	public override void Update () {	

		Vector3 pos = this.gameObject.transform.position; 

		// only send if the position has changed
		if (m_LastPosition != pos) 
		{
			Debug.Log("Send position"); 

			Send(new Rug.Osc.OscMessage(OscAddress, pos.x, pos.y, pos.z));
		}

		m_LastPosition = pos; 
	}
}
