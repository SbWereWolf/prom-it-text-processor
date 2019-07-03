using System;
using System.IO;
using System.Text;

namespace text_processor
{
    class FileHandler
    {
        private readonly string _filename;

        public FileHandler(string filename)
        {
            _filename = filename;
        }
        public string[] GetLines()
        {
            var lines = new string[0];
            if (this._filename != null)
            {
                var wholeText = string.Empty;
                try
                {
                    using (var sr = new StreamReader(this._filename, Encoding.UTF8))
                    {
                        wholeText = sr.ReadToEnd();
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }

                lines = wholeText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            return lines;
        }
    }
}
