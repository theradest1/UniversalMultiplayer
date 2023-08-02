using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.PackageManager;
using UnityEngine;
using System.Threading.Tasks;

public class Client: MonoBehaviour
{
	public int TCP_PORT = 4242;
	public int UDP_PORT = 6969;
	public string SERVER_IP = "127.0.0.1";
	public int udpTimoutMS = 1000;

	private void Start()
	{
		Debug.Log("starting server...");
		// UDP client
		/*using (UdpClient udpClient = new UdpClient())
		{
			//create message
			string udpMessage = "Hello UDP Server!";
			byte[] udpData = Encoding.ASCII.GetBytes(udpMessage);

			//create and send using client
			udpClient.Connect(SERVER_IP, UDP_PORT);
			udpClient.Send(udpData, udpData.Length);

			IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, UDP_PORT);
			byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);

			string receivedString = Encoding.ASCII.GetString(receiveBytes);
			print("UDP Server says: " + receivedString);
		}*/
		udpReciever();
		sendUDPMessage("Hi UDP (:");

		// TCP client
		using (TcpClient tcpClient = new TcpClient())
		{
			tcpClient.Connect(SERVER_IP, TCP_PORT);

			NetworkStream tcpStream = tcpClient.GetStream();
			string tcpMessage = "newClient~Test Username";
			byte[] tcpData = Encoding.ASCII.GetBytes(tcpMessage);
			byte[] tcpReceivedData = new byte[1024];

			float startTime = Time.time;

			tcpStream.Write(tcpData, 0, tcpData.Length);
			int bytesRead = tcpStream.Read(tcpReceivedData, 0, tcpReceivedData.Length);

			float latency = Time.time - startTime;

			Debug.Log("Latency of " + latency);
			string tcpResponse = Encoding.ASCII.GetString(tcpReceivedData, 0, bytesRead);
			Debug.Log("TCP Server says: " + tcpResponse);
		}
	}

	async void udpReciever()
	{
		UdpClient udpClient = new UdpClient();
		udpClient.Connect(SERVER_IP, UDP_PORT);
		IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, UDP_PORT);

		while(true)
		{
			byte[] receiveBytes = new byte[0];
			await Task.Run(() => receiveBytes = udpClient.Receive(ref remoteEndPoint));
			string receivedString = Encoding.ASCII.GetString(receiveBytes);
			processUDPMessage(receivedString);
		}
	}

	void sendUDPMessage(string message)
	{
		//load message
		byte[] udpData = Encoding.ASCII.GetBytes(message);

		//create client
		UdpClient udpClient = new UdpClient(); 
		udpClient.Connect(SERVER_IP, UDP_PORT);

		//send message
		udpClient.Send(udpData, udpData.Length);
	}
	
	void processUDPMessage(string message)
	{
		Debug.Log("Got UDP message from server:\n" + message);
	}

	void processTCPMessage(string message)
	{
		Debug.Log("Got TCP message from server:\n" + message);
	}
}
