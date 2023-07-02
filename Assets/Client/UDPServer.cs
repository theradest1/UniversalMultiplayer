using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;

public class UDPServer : MonoBehaviour
{
	UdpClient client;
	IPEndPoint remoteEndPoint;
	int SERVERPORT;
	string SERVERADDRESS;
	int ID;
	int TPS;
	[SerializeField] ServerEvents serverEvents;

	public async void connectToServer(string username, int serverPort, string serverAddress, int clientPort = -1)
	{
		SERVERADDRESS = serverAddress;
		SERVERPORT = serverPort;

		//attempt connect
		try
		{
			Debug.Log("Setting server...");
			if (clientPort == -1)
			{
				client = new UdpClient();
			}
			else
			{
				client = new UdpClient(clientPort);
			}
			client.Connect(SERVERADDRESS, SERVERPORT);
			remoteEndPoint = new IPEndPoint(IPAddress.Any, SERVERPORT);

			Debug.Log("Server set, Sending Join Message...");
			sendMessage("newClient~" + username);

			//wait for response
			byte[] receiveBytes = new byte[0];
			await Task.WhenAny(Task.Run(() => receiveBytes = client.Receive(ref remoteEndPoint)), Task.Delay(1000));
			Debug.Log("Accepted, processing data...");
			string recieveString = Encoding.ASCII.GetString(receiveBytes);
			ID = int.Parse(recieveString.Split('~')[1]);
			TPS = int.Parse(recieveString.Split('~')[2]);

			Debug.Log("User ID: " + ID);
			Debug.Log("Given TPS: " + TPS);

			serverUpdater(1/(float)TPS);
		}
		catch (Exception e)
		{
			Debug.LogError("Couldn't connect to server: " + e.Message);
			return;
		}
	}
	async void serverUpdater(float messageIncrements)
	{
		Invoke("serverUpdater", messageIncrements);
		Debug.Log("Update");
	}

	public void sendMessage(string message)
	{
		Debug.Log("Sent: " + message);
		byte[] sendBytes = Encoding.ASCII.GetBytes(message);
		client.Send(sendBytes, sendBytes.Length);
	}

	void onMessageRecieve(string message)
	{
		serverEvents.processEvent(message);
	}
}
