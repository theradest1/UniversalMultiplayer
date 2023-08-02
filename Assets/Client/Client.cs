using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.PackageManager;
using UnityEngine;

public class Client: MonoBehaviour
{
	public int TCP_PORT = 4242;
	public int UDP_PORT = 6969;
	public string SERVER_IP = "127.0.0.1";

	private void Start()
	{
		Debug.Log("starting server...");
		// UDP client
		using (UdpClient udpClient = new UdpClient())
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
	}

		// TCP client
		using (TcpClient tcpClient = new TcpClient())
		{
			tcpClient.Connect(SERVER_IP, TCP_PORT);

			NetworkStream tcpStream = tcpClient.GetStream();
			string tcpMessage = "Hello TCP Server!";
			byte[] tcpData = Encoding.ASCII.GetBytes(tcpMessage);

			tcpStream.Write(tcpData, 0, tcpData.Length);

			byte[] tcpReceivedData = new byte[1024];
			int bytesRead = tcpStream.Read(tcpReceivedData, 0, tcpReceivedData.Length);
			string tcpResponse = Encoding.ASCII.GetString(tcpReceivedData, 0, bytesRead);
			Debug.Log("TCP Server says: " + tcpResponse);
		}
	}
}
