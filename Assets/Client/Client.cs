using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine.UIElements;

public class Client: MonoBehaviour
{
	public int TCP_PORT = 4242;
	public int UDP_PORT = 6969;
	public string SERVER_IP = "127.0.0.1";

	IPEndPoint remoteEndPoint;
	UdpClient udpClient;

	TcpClient tcpClient;
	NetworkStream tcpStream;

	float udpPingStartTime;
	float tcpPingStartTime;

	public int udpLatency;
	public int tcpLatency;

	public TextMeshProUGUI udpPing;
	public TextMeshProUGUI tcpPing;

	char[] whitespaceChars = { ' ', '\t', '\n', '\r', '\f', '\v' };

	private void Start()
	{
		initUDP();
		initTCP();

		InvokeRepeating("Ping", 1, 1f);
		InvokeRepeating("TestMessage", 1, .05f);
	}

	void Ping()
	{
		udpPingStartTime = Time.time;
		sendUDPMessage("ping");
		tcpPingStartTime = Time.time;
		sendTCPMessage("ping");
	}
	void TestMessage()
	{
		udpPingStartTime = Time.time;
		sendUDPMessage("hi");
		tcpPingStartTime = Time.time;
		sendTCPMessage("howdy");
	}

	void initUDP()
	{
		remoteEndPoint = new IPEndPoint(IPAddress.Any, UDP_PORT);

		udpClient = new UdpClient();
		udpClient.Connect(SERVER_IP, UDP_PORT);

		udpReciever();
	}

	void initTCP()
	{
		tcpClient = new TcpClient();
		tcpClient.Connect(SERVER_IP, TCP_PORT);
		tcpStream = tcpClient.GetStream();

		tcpReciever();
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

	async void tcpReciever()
	{
		while(true)
		{
			byte[] tcpReceivedData = new byte[1024];
			int bytesRead = 0; //this might cause problems, but I don't think so

			await Task.Run(() => bytesRead = tcpStream.Read(tcpReceivedData, 0, tcpReceivedData.Length));
			//string message = Encoding.ASCII.GetString(tcpReceivedData, 0, bytesRead);
			string message = Encoding.UTF8.GetString(tcpReceivedData, 0, bytesRead);

			processTCPMessage(message);
		}
	}

	void sendTCPMessage(string message)
	{
		byte[] tcpData = Encoding.ASCII.GetBytes(message);
		tcpStream.Write(tcpData, 0, tcpData.Length);
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

		if(message == "pong")
		{
			udpPing.text = "UDP Latency: " + (int)((Time.time - udpPingStartTime) * 1000) + "ms";
		}
	}

	void processTCPMessage(string message)
	{
		Debug.Log("Got TCP message from server: " + message);

		if (message == "pong")
		{
			tcpPing.text = "TCP Latency: " + (int)((Time.time - tcpPingStartTime) * 1000) + "ms";
		}
	}

	void OnApplicationQuit()
	{
		tcpClient.Close();
		udpClient.Close();
	}
}