using UnityEngine;
using UnityEditor;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using System.Text;


// 网络房间
public class NetRoom : MonoBehaviour
{

    private static Socket tcpSocket;
    private static PlayerClient player1;
    private static PlayerClient player2;

    static void Init()
    {
        /*创建一个socket对象*/
        //寻址方式 套接字类型 协议方式
        tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        /*绑定监听消息IP和端口号*/
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        EndPoint endPoint = new IPEndPoint(ip, 9000);
        tcpSocket.Bind(endPoint);
        tcpSocket.Listen(2);
        Debug.Log("开启网络服务, 绑定端口: 9000");

        new Thread(new ThreadStart(delegate
        {
            Debug.Log("等待玩家1加入...");
            player1 = new PlayerClient(tcpSocket.Accept());
            Debug.Log("等待玩家1加入成功.");

            Debug.Log("等待玩家2加入...");
            player2 = new PlayerClient(tcpSocket.Accept());
            Debug.Log("等待玩家2加入成功.");
        })).Start();
    }

    private void Awake()
    {
        Init();
    }

}

class PlayerClient
{
    private Socket socket;
    private Thread recvThread;

    public PlayerClient(Socket socket)
    {
        this.socket = socket;
        recvThread = new Thread(new ThreadStart(AsyncReceive));
        recvThread.Start();
    }

    // 异步接受
    private void AsyncReceive()
    {
        byte[] buf = new byte[1024];
        int len = socket.Receive(buf);
        Debug.Log("收到数据:" + ASCIIEncoding.UTF8.GetString(buf));
    }

    // 同步发送
    public void Send(String data)
    {
        socket.Send(ASCIIEncoding.UTF8.GetBytes(data));
    }

    // 销毁
    public void Destroy()
    {
        recvThread.Interrupt();
        recvThread = null;

        socket.Close();
        socket = null;
    }
}
