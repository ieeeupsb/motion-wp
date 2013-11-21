using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Common
{
    public class API
    {
        public enum Actions
        {
            CheckIfSomeoneIsPresent,
            GetLastTimestamp
        }

        private static Uri getUrlFromAction(Actions action, params object[] parameters)
        {
            switch (action)
            {
                case Actions.CheckIfSomeoneIsPresent:
                    return new Uri(string.Format("http://nuieee.fe.up.pt/motion/CheckIfSomeoneIsPresent.php", parameters));
                case Actions.GetLastTimestamp:
                    return new Uri(string.Format("http://nuieee.fe.up.pt/motion/GetLastTimestamp.php", parameters));
            }
            return null;
        }

        public static async Task<T> GetAsync<T>(Actions action, params object[] parameters)
        {
            Uri uri = getUrlFromAction(action, parameters);

            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(uri);
            //HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            request.AllowReadStreamBuffering = true;
            var task = Task<WebResponse>.Factory.FromAsync(
                new Func<AsyncCallback, object, IAsyncResult>(request.BeginGetResponse),
                new Func<IAsyncResult, WebResponse>(request.EndGetResponse), null);
            var response = await task;

            using (Stream streamResponse = response.GetResponseStream())
            using (StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8))
            {
                var responseString = streamRead.ReadToEnd();

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
                return (T)jsonSerializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(responseString)));
            }
        }
    }
}
