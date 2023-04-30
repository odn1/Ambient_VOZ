using System;
using System.IO;

namespace AmbientService.src.utils
{
    public class Logger
    {
        #region Singlton
        private static Logger instanse;

        public static Logger GetInstanse()
        {
            return instanse is null ? instanse = new Logger() : instanse;
        }
        #endregion

        private string logPath = "Log.txt";
        private long maximumSize = 10485760;

        private Logger()
        {

        }

        public void SetData(string operation, string message)
        {
            WriteInFile(DateTime.Now.ToString("G") + " || " + operation + "||" + message);
        }

        private void WriteInFile(string text)
        {
            using (StreamWriter writer = new StreamWriter(logPath, SizeIsCorrect()))
            {
                writer.WriteLine(text);
            }
        }

        private bool SizeIsCorrect()
        {
            try
            {
                FileInfo info = new FileInfo(logPath);

                return info.Length <= maximumSize;
            }
            catch (Exception)
            {
                using (File.Create(logPath))
                {

                }

                return true;
            }
        }
    }
}
