using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.IO;
using vtortola.Core;
using vtortola.WebSockets;

namespace RemoteForJRiver
{
    class DebugLogger
    {
        static public string url { get; set; }
        private static string localtype { get; set; } = null;
        public static string LocalType { get { return localtype; } set { localtype = value; } }
        public static bool TimeStateForLog = false;
        public static Stopwatch elapsedtime;
        //public static string SendMsg { get; set; } = null;
        public static bool BrowserState { get; set; } = false;


        public static void Log(String line)
        {

            if (TimeStateForLog.Equals(false))
            {

                elapsedtime = Stopwatch.StartNew();

            }
            TimeStateForLog = true;

            Debug.WriteLine(elapsedtime.ElapsedMilliseconds + " Elapsed msecs. " + line);

        }
    }

   
    class WebSocketForRemote
    {
        readonly Int32 port = 8734;
        WebSocket server;
        CancellationTokenSource cancellation;

        public void WebSocketServerStart()
        {
            cancellation = new CancellationTokenSource();
            var endpoint = new IPEndPoint(IPAddress.Any, port);
            WebSocketListener server = new WebSocketListener(endpoint, new WebSocketListenerOptions() { SubProtocols = new[] { "text" } });
            var rfc6455 = new vtortola.WebSockets.Rfc6455.WebSocketFactoryRfc6455(server);
            server.Standards.RegisterStandard(rfc6455);
            server.Start();
            Log("Mono Echo Server started at:" + endpoint.ToString() + "and local endpoint is: " + server.LocalEndpoint.ToString());
            var task = Task.Run(() => AcceptWebSocketClientsAsync(server, cancellation.Token));

            Log("Server started and control passed to WebSocketClientAsync");
            
            task.Wait();
            Log("Server stoping");
            cancellation.Cancel();
           
        }

        private void OnStop()
        {
            if (server != null)
            {
                try { cancellation.Cancel(); }
                catch { }
                Log("Server stoping");
                server.Dispose();
                server = null;
            }
        }

        public static void Log(string msg)
        {
            DebugLogger.Log(msg);
        }

        static async Task AcceptWebSocketClientsAsync(WebSocketListener server, CancellationToken token)
        {
            Log("AcceptWebSocketClient task has been entered");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var ws = await server.AcceptWebSocketAsync(token);
                    Log("Connected " + ws ?? "Null");
                    if (ws != null)
                        await Task.Run(() => HandleConnectionAsync(ws, token));
                }
                catch (Exception aex)
                {
                    Log("Error Accepting clients: " + aex.GetBaseException().Message);
                }
            }
            Log("Server Stop accepting clients");
        }

        static async Task HandleConnectionAsync(vtortola.WebSockets.WebSocket ws, CancellationToken cancellation)
        {
            try
            {

                while (ws.IsConnected && !cancellation.IsCancellationRequested)
                {
                    WebSocketMessageReadStream messageReadStream = await ws.ReadMessageAsync(cancellation);
                    String msg = String.Empty;
                    using (var sr = new StreamReader(messageReadStream, Encoding.UTF8))
                        msg = await sr.ReadToEndAsync();

                    Log("Message: " + msg);
                    HandleRoute(msg, ws);
                }

            }
            catch (Exception aex)
            {
                Log("Error Handling connection: " + aex.GetBaseException().Message);
                try { ws.Close(); }
                catch { }
            }
            finally
            {
                ws.Dispose();
            }
        }

        internal static async void SendMessageAsync(vtortola.WebSockets.WebSocket ws, string message)
        {
            using (WebSocketMessageWriteStream messageWriterStream = ws.CreateMessageWriter(vtortola.WebSockets.WebSocketMessageType.Text))
            using (var sw = new StreamWriter(messageWriterStream, Encoding.UTF8))
                await sw.WriteAsync(message);
            Log("Message sent to client:" + message);
        }

        private static void HandleRoute(string message, vtortola.WebSockets.WebSocket ws)
        {
            //const string Proc = "Process";
            //const string Setup = "SetupRequest";
            //const string MInputs = "mouse";
            //const string KInputs = "Keys";
            //const string DeviceCmd = "Cmd";
            string[] words = message.Split(',');
            Log("Request from Client is:" + message);
            string[] FirstRequest = words[0].Split(':');

            //switch (FirstRequest[0])
            //{
            //    case MInputs:
            //        Program.Log("A " + FirstRequest[0] + "input was received to process " + FirstRequest[1]);
            //        switch (FirstRequest[1])
            //        {
            //            case "mousemove":
            //                {
            //                    string[] SecondRequest = words[1].Split(':');
            //                    string MvalueX = SecondRequest[0];
            //                    string MvalueY = SecondRequest[1];
            //                    int _moveX = Convert.ToInt32(MvalueX);
            //                    int _moveY = Convert.ToInt32(MvalueY);
            //                    MouseHandler.MouseMove(_moveX, _moveY);
            //                    Program.Log("A mouse move was received, x= " + MvalueX + " y= " + MvalueY);

            //                    break;
            //                }
            //            case "hscroll":
            //                {
            //                    string[] SecondRequest = words[1].Split(':');
            //                    string MvalueX = SecondRequest[0];
            //                    int _moveX = Convert.ToInt32(MvalueX);
            //                    MouseHandler.HMouseScroll(_moveX);
            //                    Program.Log("A horizontal scroll move was received, x= " + MvalueX + " y= 0");
            //                    break;
            //                }
            //            case "mousetype":
            //                {
            //                    string MValue = words[1];
            //                    MouseHandler.MouseType(MValue, ws);

            //                    break;
            //                }
            //            case "hscrolltype":
            //                {
            //                    string MValue = words[1];
            //                    MouseHandler.HScrollType(MValue, ws);
            //                    break;
            //                }

            //        }
            //        break;
            //    case KInputs:
            //        {
            //            switch (FirstRequest[1])
            //            {
            //                case "Key":
            //                    {
            //                        Program.Log("A " + FirstRequest[0] + "input was received to process " + FirstRequest[1]);
            //                        KeyHandler.HandleKeys(words, ws);

            //                        break;
            //                    }
            //                case "KeyInput":
            //                    {

            //                        string[] FirstValue = words[1].Split(':');
            //                        string[] SecondValue = words[2].Split(':');
            //                        string[] ThirdValue = words[3].Split(':');
            //                        string[] FourthValue = words[4].Split(':');
            //                        string[] FifthValue = words[5].Split(':');

            //                        string VirtKey = FirstValue[1].ToString();
            //                        bool Shifty = Convert.ToBoolean(SecondValue[1]);
            //                        bool ControlKey = Convert.ToBoolean(ThirdValue[1]);
            //                        bool AltKey = Convert.ToBoolean(FourthValue[1]);
            //                        bool WinKey = Convert.ToBoolean(FifthValue[1]);

            //                        SendInputCommand.SendComboKeyCommand(VirtKey, Shifty, ControlKey, AltKey, WinKey);
            //                        break;
            //                    }
            //                case "SpKeyInput":
            //                    {

            //                        string[] FirstValue = words[1].Split(':');
            //                        string[] SecondValue = words[2].Split(':');
            //                        string[] ThirdValue = words[3].Split(':');
            //                        string[] FourthValue = words[4].Split(':');
            //                        string[] FifthValue = words[5].Split(':');

            //                        string VirtKey1 = FirstValue[1].ToString();
            //                        string VirtKey2 = SecondValue[1].ToString();
            //                        bool Shifty = Convert.ToBoolean(ThirdValue[1]);
            //                        bool ControlKey = Convert.ToBoolean(FourthValue[1]);
            //                        bool AltKey = Convert.ToBoolean(FifthValue[1]);


            //                        SendInputCommand.SpecialSendComboKeyCommand(VirtKey1, VirtKey2, Shifty, ControlKey, AltKey);
            //                        break;
            //                    }

            //            };
            //            break;
            //        }
            //    case Proc:
            //        {

            //            //if (words[1] ==null)
            //            //{
            //            HandleProcess(FirstRequest[1], words, ws);
            //            Program.Log("A " + FirstRequest[0] + "input was received to process " + FirstRequest[1]);

            //            //}
            //            //else
            //            //{
            //            //    string[] Secondword;
            //            //    Secondword = words[1].Split(':');


            //            //    HandleProcess(FirstRequest[1], Secondword[1]);
            //            //    Program.Log("A " + FirstRequest[0] + "input was received to process " + FirstRequest[1]);
            //            //}

            //            break;
            //        }
            //    case Setup:
            //        Program.Log("A " + FirstRequest[0] + "input was received to process " + FirstRequest[1]);
            //        break;
            //    case DeviceCmd:
            //        Program.Log("A " + FirstRequest[0] + "input was received to process " + FirstRequest[1]);
            //        switch (FirstRequest[1])
            //        {
            //            case "audio":
            //                {
            //                    string DValue = words[1];
            //                    DevicesHandler.HandleAudio(ws, DValue);
            //                }
            //                break;
            //            case "tv": { } break;
            //            case "lights": { } break;

            //        }
            //        break;

            //}

        }



        private static void HandleProcess(string msg, string[] words, vtortola.WebSockets.WebSocket ws)
        {




            //    Process proc = new Process();

            //    switch (msg)
            //    {
            //        case "AState":
            //            {
            //                //proc.Start("https://www.netflix.com/login");
            //                //ConsoleApplication1.WebDriver.ControlEdge("https://www.netflix.com/login");




            //                //Process.Start("microsoft - edge: https://www.netflix.com/login");
            //                //EdgeDiverForRemote.UseEdge();
            //                //url = "https://www.netflix.com/login";
            //                //BrowserEventHandlers.BrowserNavigate(url, ws);

            //                //SendInputCommand.SpecialSendComboKeyCommand("43", false, false, true, true);
            //                Process.Start("microsoft-edge:https://www.netflix.com/login");


            //            }
            //            break;
            //        case "BState":
            //            {

            //                //ConsoleApplication1.WebDriver.ControlEdge("https://www.amazon.com/");
            //                //url = "https://www.amazon.com/";
            //                //browserEventHandlers.BrowserNavigate(url, ws);
            //                Process.Start("microsoft-edge:https://www.amazon.com/");
            //            }
            //            break;
            //        case "CState":
            //            {
            //                //COMMANDS AND ARGUMENTS TO BE SENT TO JRIVER

            //                //proc.StartInfo.Arguments = "/MCC 22001,10";
            //                //   proc.StartInfo.FileName = "C:\\Windows\\System32\\MC22.exe";
            //            }
            //            break;
            //        case "DState":
            //            {
            //                url = "https://www.netflix.com/browse/my-list";
            //                //BrowserEventHandlers.BrowserNavigate(url, ws);
            //            }
            //            break;
            //        case "EState":
            //            {
            //                //Changes the Browser's setting
            //                string word = words[1].ToString();
            //                // BrowserEventHandlers.BrowserChange(word);
            //            }
            //            break;

            //    }
            //    //proc.Start();
            //    //int Handle = proc.Id;
            //    //IntPtr MainWindowHandle = proc.MainWindowHandle;
            //    //IntPtr MyHandles = proc.Handle;
            //    //int HandleCount = proc.HandleCount;
            //    //Log("Id is:" + Handle + ",Main Window Handle is:" + MainWindowHandle + ", and the handles are:" + MyHandles + ":handlecount is:" + HandleCount);


            //}
            //private static void HandleProcess(string msg, string args)
            //{
            //    string arguments = "/MCC " + args;
            //    Process proc = new Process();
            //    switch (msg)
            //    {

            //        case "CState":
            //            {
            //                proc.StartInfo.Arguments = arguments;
            //                proc.StartInfo.FileName = "C:\\Windows\\System32\\MC22.exe";

            //                break;
            //            }

            //    }
            //    proc.Start();
            //    int Handle = proc.Id;
            //    IntPtr MainWindowHandle = proc.MainWindowHandle;
            //    IntPtr MyHandles = proc.Handle;
            //    int HandleCount = proc.HandleCount;
            //    Log("Id is:" + Handle + ",Main Window Handle is:" + MainWindowHandle + ", and the handles are:" + MyHandles + ":handlecount is:" + HandleCount);


            //}
        }
    }
}

    