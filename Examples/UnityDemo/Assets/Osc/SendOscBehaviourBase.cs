using System.Net;
using System.Collections;
using UnityEngine;
using Rug.Osc;

public class SendOscBehaviourBase : MonoBehaviour {

	private OscSendController m_SendController; 
	
	public string SendControllerObjectName = ""; 

	public virtual void Awake () { 		
		m_SendController = null; 
		
		if (string.IsNullOrEmpty (SendControllerObjectName) == true) {
			Debug.LogError("You must supply a SendControllerObjectName"); 
			return; 
		}
		
		GameObject controllerObject = GameObject.Find (SendControllerObjectName); 
		
		if (controllerObject == null) {
			Debug.LogError(string.Format("A GameObject with the name '{0}' could not be found", SendControllerObjectName)); 
			return; 
		}
		
		OscSendController controller = controllerObject.GetComponent<OscSendController> (); 
		
		if (controller == null) { 
			Debug.LogError(string.Format("The GameObject with the name '{0}' does not contain a OscSendController component", SendControllerObjectName)); 
			return; 
		}
		
		m_SendController = controller; 
	}

	public virtual void Start () {

	}

	public virtual void Update() {

	}

	public void Send(OscMessage msg) {
		
		if (m_SendController != null) {
			// Send the message
			m_SendController.Sender.Send (msg);
		}
	}
}
