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
    class Program
    {
        private static int numThreads = 2;

        public static void Main(string[] args)
        {
            int countThread;

            Thread[] serverThreads = new Thread[numThreads];

            Console.WriteLine("Welcome! This is an example of implementing Name Pipe for inter-communication process.");
            Console.WriteLine("Program is currently waiting for client connection...");

            for (countThread = 0; countThread < numThreads; countThread++)
            {
                serverThreads[countThread] = new Thread(ServerThread);
                serverThreads[countThread].Start();
            } // END FOR

            Thread.Sleep(300);

            while (countThread > 0)
            {
                for (int count = 0; count < numThreads; count++)
                {
                    if (serverThreads[count] != null)
                    {
                        if (serverThreads[count].Join(300))
                        {
                            Console.WriteLine("Server thread[{0}] process completed.", serverThreads[count].ManagedThreadId);
                            serverThreads[count] = null;
                            countThread--;
                        } // END IF
                    } // END IF
                } // END FOR
            } // END WHILE

            Console.WriteLine("Server threads exhausted.");
            Console.WriteLine("Enter any key to exit the program");
            Console.ReadLine();
        }

        private static void ServerThread(object data)
        {
            // Setup Pipe Server with name "mypipe"
            NamedPipeServerStream myPipeServer = new NamedPipeServerStream("mypipe", PipeDirection.InOut, numThreads);

            int threadId = Thread.CurrentThread.ManagedThreadId;

            // Awaiting for client connection
            myPipeServer.WaitForConnection();

            Console.WriteLine("Connection found on thread[{0}]", threadId);

            try
            {
                // A security token will be available when client has written to the pipe.
                StreamString theStreamString = new StreamString(myPipeServer);

                // Authenticate the identity to connected client
                theStreamString.WriteString("Hello, Friend");
                string theFilename = theStreamString.ReadString();

                // read file while impersonating client
                ReadFileToStream fileReader = new ReadFileToStream(theStreamString, theFilename);

                Console.WriteLine("Reading file: {0} on thread[{1}] as user: {2}.", theFilename, threadId, myPipeServer.GetImpersonationUserName());

                myPipeServer.RunAsClient(fileReader.startRead);
            }
            catch (IOException exception) // IOException catch borken pipe and disconnected (naming issues etc)
            {
                Console.WriteLine("ERROR: {0}", exception.Message);
            } // END CATCH

            myPipeServer.Close();
        } // END ServerThread()
    }
}
