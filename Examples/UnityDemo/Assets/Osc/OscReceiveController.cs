using System;
using System.Net;
using System.Collections;
using UnityEngine;
using UnityEditor; 
using Rug.Osc;

public class OscReceiveController : MonoBehaviour {

	#region Private Members

	// Receiver Instance 
	private OscReceiver m_Receiver;

	// Namespace manager instance
	private OscAddressManager m_Manager = new OscAddressManager(); 

	#endregion

	public int ListenPort = 5001;

	public int MaxMessagesToProcessPerFrame = 10;

	public OscAddressManager Manager { get { return m_Manager; } }



	public OscReceiveController() { }

	void Awake () { 
		
		// Log the start
		Debug.Log ("Starting Osc Receiver"); 
		
		// Ensure that the receiver is disconnected
		Disconnect (); 
		
		// The address to listen on to 
		IPAddress addess = IPAddress.Any; 
		
		// The port to listen on 
		int port = ListenPort;
		
		// Create an instance of the receiver
		m_Receiver = new OscReceiver(addess, port);
		
		// Connect the receiver
		m_Receiver.Connect ();
		
		// We are now connected
		Debug.Log ("Connected Receiver"); 
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		int i = 0; 

		// if we are in a state to recieve
		while (i++ < MaxMessagesToProcessPerFrame && 
		       m_Receiver.State == OscSocketState.Connected)
		{
			OscPacket packet;
		
			// get the next message this will not block
			if (m_Receiver.TryReceive(out packet) == false) 
			{
				return; 
			}
			
			switch (m_Manager.ShouldInvoke(packet))
			{
				case OscPacketInvokeAction.Invoke:
					// Debug.Log ("Received packet");
					m_Manager.Invoke(packet);
					break;
				case OscPacketInvokeAction.DontInvoke:
					Debug.LogWarning ("Cannot invoke");
					Debug.LogWarning (packet.ToString()); 
					break;
				case OscPacketInvokeAction.HasError:
					Debug.LogError ("Error reading osc packet, " + packet.Error);
					Debug.LogError (packet.ErrorMessage);
					break;
				case OscPacketInvokeAction.Pospone:
					Debug.Log ("Posponed bundle");
					Debug.Log (packet.ToString()); 
					break;
				default:
					break;
			}											
		}
	}	
	
	// OnDestroy is called when the object is destroyed
	public void OnDestroy () {

		Disconnect (); 

		m_Manager.Dispose (); 

	}
	
	private void Disconnect () {
		
		// If the receiver exists
		if (m_Receiver != null) {
			
			// Disconnect the receiver
			Debug.Log ("Disconnecting Receiver"); 
			
			m_Receiver.Dispose (); 
			
			// Nullifiy the receiver 
			m_Receiver = null;
		}
	}
}
