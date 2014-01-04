using System.Collections;
using UnityEngine;
using Rug.Osc;

public abstract class ReceiveOscBehaviourBase : MonoBehaviour {

	private OscReceiveController m_ReceiveController; 

	public string ReceiveControllerObjectName = ""; 

	public string OscAddress = "/test";

	public void Awake () {
		
		m_ReceiveController = null; 
		
		if (string.IsNullOrEmpty (ReceiveControllerObjectName) == true) {
			Debug.LogError("You must supply a ReceiverControllerObjectName"); 
			return; 
		}
		
		GameObject controllerObject = GameObject.Find (ReceiveControllerObjectName); 
		
		if (controllerObject == null) {
			Debug.LogError(string.Format("A GameObject with the name '{0}' could not be found", ReceiveControllerObjectName)); 
			return; 
		}
		
		OscReceiveController controller = controllerObject.GetComponent<OscReceiveController> (); 
		
		if (controller == null) { 
			Debug.LogError(string.Format("The GameObject with the name '{0}' does not contain a OscReceiveController component", ReceiveControllerObjectName)); 
			return; 
		}
		
		m_ReceiveController = controller; 
	}

	// Use this for initialization
	public virtual void Start () {

		if (m_ReceiveController != null) {

			m_ReceiveController.Manager.Attach (OscAddress, ReceiveMessage); 
		}
	}

	// Update is called once per frame
	public virtual void Update () {
	
	}

	public virtual void OnDestroy () {

		// detach from the OscAddressManager
		if (m_ReceiveController != null) {
			m_ReceiveController.Manager.Detach (OscAddress, ReceiveMessage);
		}
	}

	protected abstract void ReceiveMessage (OscMessage message);
}
