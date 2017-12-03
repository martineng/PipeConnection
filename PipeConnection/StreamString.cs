using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace PipeConnection
{
    // StreamString class is primarily being implemented to define the data protocol for reading and writing strings
    class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamUniEncoding;

        public StreamString()
        {
            // empty
        }

        public StreamString(Stream inIoStream)
        {
            this.ioStream = inIoStream;
            streamUniEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int length = 0;

            length = ioStream.ReadByte() * 256;
            length += ioStream.ReadByte();

            byte[] inBuffer = new byte[length];
            ioStream.Read(inBuffer, 0, length);

            return streamUniEncoding.GetString(inBuffer);
        } // END ReadString()

        public int WriteString(string inOutputString)
        {
            byte[] outputBuffer = streamUniEncoding.GetBytes(inOutputString);
            int length = outputBuffer.Length;

            if (length > UInt16.MaxValue)
            {
                length = (int)UInt16.MaxValue;
            } // END IF

            ioStream.WriteByte((byte)(length/256));
            ioStream.WriteByte((byte)(length & 255));
            ioStream.Write(outputBuffer, 0, length);
            ioStream.Flush();

            return outputBuffer.Length + 2;
        } // END WriteString()

    }
}
