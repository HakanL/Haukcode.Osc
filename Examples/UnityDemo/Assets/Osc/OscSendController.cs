using System.Net;
using System.Collections;
using UnityEngine;
using Rug.Osc;

public class OscSendController : MonoBehaviour {

	#region Private Members

	// Sender Instance 
	private OscSender m_Sender;

	#endregion
	
	public int RemotePort = 5000;
	
	public string RemoteAddress = "255.255.255.255"; 

	public OscSender Sender { get { return m_Sender; } }



	public OscSendController() { }

	void Awake () { 
		
		// Log the start
		Debug.Log ("Starting Osc"); 
		
		// Ensure that the sender is disconnected
		Disconnect (); 
		
		// The address to send to 
		IPAddress addess = IPAddress.Parse (RemoteAddress); 
		
		// The port to send to 
		int port = RemotePort;
		
		// Create an instance of the sender
		m_Sender = new OscSender(addess, 0, port);
		
		// Connect the sender
		m_Sender.Connect ();
		
		// We are now connected
		Debug.Log ("Sender Connected"); 
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}	
	
	// OnDestroy is called when the object is destroyed
	public void OnDestroy () {
		Disconnect (); 
	}
	
	private void Disconnect () {
		
		// If the sender exists
		if (m_Sender != null) {
			
			// Disconnect the sender
			Debug.Log ("Disconnecting Sender"); 
			
			m_Sender.Dispose (); 
			
			// Nullifiy the sender 
			m_Sender = null;
		}
	}
}
