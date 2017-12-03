using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeConnection
{
    // A simple class to open file and impersonated user.
    class ReadFileToStream
    {
        private string filename;
        private StreamString streamString;

        public ReadFileToStream(StreamString inString, string inFilename)
        {
            streamString = inString;
            filename = inFilename;
        }

        public void startRead()
        {
            string content = File.ReadAllText(filename);
            streamString.WriteString(content);
        }
    }
}
