using Server;
using System.Net;
using System.Net.Sockets;
using System.Text;


var server = new UdpClient(27001);

var remoteEP = new IPEndPoint(IPAddress.Any, 0);

while (true)
{
    var bytes = server.Receive(ref remoteEP);
    var str = Encoding.Default.GetString(bytes);

    //  Seperate string to width and height
    int index = str.IndexOf(":");
    int width = int.Parse(str.Substring(0, index));
    int height = int.Parse(str.Substring(index + 1));

    while (true)
    {
        var screenshot = AssistentFunctions.CaptureScreen(width, height);
        var imgBytes = AssistentFunctions.ImageToByteArray(screenshot);
        var currentBytes = 0;
        var bufferSize = ushort.MaxValue - 28;
        
        while (imgBytes.Length > currentBytes)
        {
            if (imgBytes.Length - currentBytes < bufferSize)
            {
                bufferSize = imgBytes.Length - currentBytes;
            }
            var buffer = imgBytes.Skip(currentBytes).Take(bufferSize).ToArray();
            await server.SendAsync(buffer, remoteEP);
            currentBytes += bufferSize;
        }
    }
}