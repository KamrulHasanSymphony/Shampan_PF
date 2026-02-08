using System;
using System.IO;
using System.Threading;
using System.Web.Hosting;


namespace SymOrdinary
{
    public class FileLogger
    {
        public static void Log(string source, string actionName, string message)
        {
            /*Create Message object and assign values with log parameter*/
            MessageTemplate messageTemplate = new MessageTemplate();
            messageTemplate.Source = source;
            messageTemplate.ActionName = actionName;
            messageTemplate.Message = message;

            /*Create new parameterized thread object*/
            Thread newThread = new Thread(new ParameterizedThreadStart(FileLogger.WriteToFile));

            /*Start thread*/
            newThread.Start(messageTemplate);
        }
        public static void WriteToFile(object messageTemplate)
        {
            /*Cast message object with the value of parameter*/
            MessageTemplate msTemplate = (MessageTemplate)messageTemplate;

            /*Assign log path*/
            string path = HostingEnvironment.MapPath("~/Files/LoggerFile/Logs.txt");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            //string path = Application.StartupPath + "\\Logs.txt";
            string curDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss zzz");

            /*Assign message header line*/
            string dateText = Environment.NewLine + curDate + Environment.NewLine + " Source : " + msTemplate.Source + " , Method : " + msTemplate.ActionName;

            if (!string.IsNullOrEmpty(path) && !File.Exists(path))
            {
                // Create a file to write to.

                File.WriteAllText(path, "");

            }
            /*Write message header line*/
            File.AppendAllText(path, dateText);
            /*Write a new line*/
            File.AppendAllText(path, Environment.NewLine);
            /*Write message*/
            File.AppendAllText(path, msTemplate.Message);
            /*Write another new line after the message*/
            File.AppendAllText(path, Environment.NewLine);
        }

        public class MessageTemplate
        {
            public string Source { get; set; }
            public string ActionName { get; set; }
            public string Message { get; set; }
        }

    }
}
