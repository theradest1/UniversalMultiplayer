using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEvents : MonoBehaviour
{
	[SerializeField] UDPServer server;
    public void sendEvent(string eventName, string data)
	{
		server.sendMessage(eventName + "~" + data);
	}

	public void processEvent(string message)
	{
		print("Processed event: " + message);
	}
}
