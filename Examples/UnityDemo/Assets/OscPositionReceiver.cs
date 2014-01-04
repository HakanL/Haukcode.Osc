using UnityEngine;
using System.Collections;
using Rug.Osc;

public class OscPositionReceiver : ReceiveOscBehaviourBase {

	public Vector3 Offset = new Vector3(4f, 0, 0); 

	protected override void ReceiveMessage (OscMessage message) {

		Debug.Log("Receive position"); 

		if (message.Count != 3) 
		{
			Debug.LogError(string.Format("Unexpected argument count {0}", message.Count));  
		
			return; 
		}

		if (!(message[0] is float) || 
		    !(message[1] is float) ||
		    !(message[2] is float))
		{
			Debug.LogError(string.Format("Unexpected argument type"));  

			return; 
		}

		// get the position from the message arguments 
		float x = (float)message [0];
		float y = (float)message [1];
		float z = (float)message [2];

		// assign the transform position from the x, y, z and add the offset
		this.gameObject.transform.position = new Vector3 (x, y, z) + Offset; 
	}
}
