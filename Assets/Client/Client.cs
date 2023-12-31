using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.PackageManager;

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

	int udpLatency;
	int tcpLatency;
	int sendBytesUDP;
	int sendBytesTCP;
	int getBytesUDP;
	int getBytesTCP;

	public TextMeshProUGUI udpPing;
	public TextMeshProUGUI tcpPing;
	public TextMeshProUGUI tcpSendBytes;
	public TextMeshProUGUI udpSendBytes;
	public TextMeshProUGUI tcpGetBytes;
	public TextMeshProUGUI udpGetBytes;

	public Events events;

	string tcpMessageQueue = "";

	public Transform playerTransform;
	public Transform camTransform;

	private void Start()
	{
		initUDP();
		initTCP();

		InvokeRepeating("Ping", 1, 1f);
		InvokeRepeating("DebugText", 1, 1f);
		InvokeRepeating("TransformUpdate", 1.5f, 1f);// .00778f);
	}

	void Ping()
	{
		udpPingStartTime = Time.time;
		sendUDPMessage("ping");
		tcpPingStartTime = Time.time;
		sendTCPMessage("ping");
	}

	void DebugText()
	{
		tcpSendBytes.text = "TCP send b/s: " + sendBytesTCP;
		udpSendBytes.text = "UDP send b/s: " + sendBytesUDP;
		tcpGetBytes.text = "TCP get b/s: " + getBytesTCP;
		udpGetBytes.text = "UDP get b/s: " + getBytesUDP;

		sendBytesTCP = 0;
		sendBytesUDP = 0;
		getBytesTCP = 0;
		getBytesUDP = 0;
	}

	void TransformUpdate()
	{
		sendUDPMessage(events.ID + "~" + playerTransform.position + "~" + camTransform.rotation);
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
			getBytesUDP += Encoding.UTF8.GetByteCount(message);
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
			string message = Encoding.UTF8.GetString(tcpReceivedData, 0, bytesRead);

			getBytesTCP += Encoding.UTF8.GetByteCount(message);

			Debug.Log("Got TCP Message: " + message);

			//loop through messages
			string[] messages = message.Split('|');
			foreach(string finalMessage in messages)
			{
				if(finalMessage != "") //to get rid of final message
				{
					processTCPMessage(finalMessage);
				}
			}
		}
	}

	public void sendTCPMessage(string message)
	{
		message += "|";
		sendBytesTCP += Encoding.UTF8.GetByteCount(message);
		byte[] tcpData = Encoding.ASCII.GetBytes(message);
		tcpStream.Write(tcpData, 0, tcpData.Length);
	}

	public void sendUDPMessage(string message)
	{
		sendBytesUDP += Encoding.UTF8.GetByteCount(message);
		//load message
		byte[] udpData = Encoding.ASCII.GetBytes(message);

		//send message
		udpClient.Send(udpData, udpData.Length);
	}

	void processUDPMessage(string message)
	{

		if(message == "pong")
		{
			udpPing.text = "UDP Latency: " + (int)((Time.time - udpPingStartTime) * 1000) + "ms";
		}
		else
		{
			Debug.Log("Got UDP message from server:\n" + message);
		}
	}

	void processTCPMessage(string message)
	{
		//Debug.Log("Got TCP message from server: " + message);

		if (message == "pong")
		{
			tcpPing.text = "TCP Latency: " + (int)((Time.time - tcpPingStartTime) * 1000) + "ms";
			return;
		}

		events.rawEvent(message);
	}

	void OnApplicationQuit()
	{
		/*//close tcp
		tcpStream.Close();
		tcpClient.Close();

		//close udp
		udpClient.Close();*/ //just makes error rn
	}
}