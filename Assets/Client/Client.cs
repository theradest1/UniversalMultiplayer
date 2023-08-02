using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class Client: MonoBehaviour
{
	public int TCP_PORT = 4242;
	public int UDP_PORT = 6969;
	public string SERVER_IP = "127.0.0.1";
	public int udpTimoutMS = 1000;

	IPEndPoint remoteEndPoint;
	UdpClient udpClient;

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
		initUDP();

		//new Thread(() =>
		//{
		//	Thread.CurrentThread.IsBackground = true;
			udpReciever();
		//}).Start();

		InvokeRepeating("TestMessages", 1, .05f);

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

	void TestMessages()
	{
		sendUDPMessage("Hi UDP (:");
	}

	void initUDP()
	{
		remoteEndPoint = new IPEndPoint(IPAddress.Any, UDP_PORT);

		udpClient = new UdpClient();
		udpClient.Connect(SERVER_IP, UDP_PORT);
	}

	async void udpReciever()
	{
		while(true)
		{
			byte[] receiveBytes = new byte[0];
			await Task.Run(() => receiveBytes = udpClient.Receive(ref remoteEndPoint));
			string message = Encoding.ASCII.GetString(receiveBytes);
			processUDPMessage(message);
		}
	}

	void sendUDPMessage(string message)
	{
		//load message
		byte[] udpData = Encoding.ASCII.GetBytes(message);

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
