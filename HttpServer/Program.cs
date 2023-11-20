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
            Console.WriteLine(listener.Prefixes.ToString());

            while (true)
            {   
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                string clientIpAddress = request.RemoteEndPoint.Address.ToString();
                if (request.HttpMethod == "POST" && request?.Url?.LocalPath == "/login")
                {
                    string? username, password;
                    using Stream body = request.InputStream;
                    StreamReader reader = new(body, request.ContentEncoding);
                    string requestBody = reader.ReadToEnd();
                    // 여기서 requestBody를 파싱하여 username과 password를 추출하세요
                    // 예: "username=사용자명&password=비밀번호"
                    // 이 정보를 사용하여 로그인을 확인하고 응답을 보내세요
                    // 예를 들어, 간단하게 아이디가 "user"이고 비밀번호가 "password"인 경우를 확인하는 코드는 다음과 같을 수 있습니다.
                    var queryParams = HttpUtility.ParseQueryString(requestBody);
                    username = queryParams["username"];
                    password = queryParams["password"];
                    Console.WriteLine($"{clientIpAddress} : {requestBody}");


                    string responseString = "";
                    using var db = new UsersContext();
                    User? id = db.Users.SingleOrDefault(u => u.Username == username);
                    if (id == null)
                    {
                        responseString = "not exist id";
                        Console.WriteLine($"{clientIpAddress} : Result : {responseString}");
                    } 
                    else if(id.Password == password)
                    {
                        responseString = "success";
                        Console.WriteLine($"{clientIpAddress} : Result : {responseString}");
                    }
                    else
                    {
                        responseString = "not match password";
                        Console.WriteLine($"{clientIpAddress} : Result : {responseString}");
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    context.Response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = context.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                if (request?.HttpMethod == "POST" && request?.Url?.LocalPath == "/register")
                {
                    string? username, password, passwordCheck;
                    using Stream body = request.InputStream;
                    StreamReader reader = new(body, request.ContentEncoding);
                    string requestBody = reader.ReadToEnd();
                    var queryParams = HttpUtility.ParseQueryString(requestBody);
                    username = queryParams["username"];
                    password = queryParams["password"];
                    passwordCheck = queryParams["passwordCheck"];
                    Console.WriteLine($"{clientIpAddress} : {requestBody}");

                    string responseString = "";

                    using var db = new UsersContext();
                    if(db.Users.Any(u => u.Username == username))
                    {
                        responseString = "exists id";
                        Console.WriteLine($"{clientIpAddress} : Result : {responseString}");
                    }
                    else if(password == passwordCheck)
                    {
                        var user = new User
                        {
                            Username = username,
                            Password = password,
                        };
                        db.Users.Add(user);
                        db.SaveChanges();

                        responseString = "success";
                        Console.WriteLine($"{clientIpAddress} : Result : {responseString}");
                    }
                    else
                    {
                        responseString = "not match password";
                        Console.WriteLine($"{clientIpAddress} : Result : {responseString}");
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    context.Response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = context.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }
    }
}