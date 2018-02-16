using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JBP.ConnectionUtilities
{
    public class ConnectionUtility
    {
        public static string ReadClientStream(TcpClient client)
        {
            try
            {
                StringBuilder content = new StringBuilder();
                byte[] bytesFrom = new byte[10025];
                NetworkStream networkStream = client.GetStream();
                var _byteread = networkStream.Read(bytesFrom, 0, (int)bytesFrom.Length);
                if (_byteread > 0)
                {
                    content.Append(Encoding.ASCII.GetString(bytesFrom));
                    var _eof_index = content.ToString().IndexOf("<EOF>");
                    if (_eof_index > -1)
                    {
                        return content.ToString().Substring(0, _eof_index);
                    }
                    else
                    {
                        return content.Append(ReadClientStream(client)).ToString();
                    }
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool WriteClientStream(TcpClient client, string message)
        {
            try
            {
                NetworkStream serverStream = client.GetStream();
                byte[] outStream = Encoding.ASCII.GetBytes(message + "<EOF>");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }         

    }
}
