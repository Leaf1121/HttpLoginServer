using HttpServer.DateBase;
using System.Net;
using System.Text;
using System.Web;

namespace HttpServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            HttpListener listener = new();
            listener.Prefixes.Add("http://192.168.20.98:8145/");
            listener.Start();
            Console.WriteLine("Server Start");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                string clientIpAddress = request.RemoteEndPoint.Address.ToString();
                if (request.HttpMethod == "POST" && request?.Url?.LocalPath == "/login")
                {
                    string? key;
                    using Stream body = request.InputStream;
                    StreamReader reader = new(body, request.ContentEncoding);
                    string requestBody = reader.ReadToEnd();
                    var queryParams = HttpUtility.ParseQueryString(requestBody);
                    key = queryParams["key"];
                    Console.WriteLine($"{clientIpAddress} : Key = {key}");
                    
                    string responseString = "";

                    using var db = new KeyContext();
                    KeyDB? keys = db.Keys.SingleOrDefault(k => k.Key == key);
                    int datetime = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    int dbdatetime = 0;
                    if (keys == null)
                    {
                        responseString = "false";
                        Console.WriteLine($"{clientIpAddress} : {key}, {responseString}, Reason = not exists Key");
                    }
                    else
                    {
                        dbdatetime = int.Parse(keys.Datetime);
                    }
                    if(datetime <= dbdatetime)
                    {
                        responseString = "true";
                        Console.WriteLine($"{clientIpAddress} : {key}, {datetime}~{dbdatetime}, {responseString}");
                    }
                    else
                    {
                        responseString = "false";
                        Console.WriteLine($"{clientIpAddress} : {key}, {responseString}, Reason = Datetime Over");
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    context.Response.ContentLength64 = buffer.Length;
                    Stream output = context.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }
        public static bool KeyAuth(string key)
        {
            int count = 0;
            string[] lines = File.ReadAllLines("keyList.txt");
            foreach(string line in lines)
            {
                if (key == line) count++;
            }
            if(count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}   