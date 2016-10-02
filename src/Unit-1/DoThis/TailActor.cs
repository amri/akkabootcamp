using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    class TailActor:UntypedActor
    {
        private readonly IActorRef _reporterActor;
        private readonly string _filePath;
        private FileObserver _observer;
        private FileStream _fileStream;
        private StreamReader _fileStreamReader;

        public TailActor(IActorRef reporterActor, string filePath)
        {
            _reporterActor = reporterActor;
            _filePath = filePath;

            _observer = new FileObserver(Self, filePath);
            _observer.Start();

            _fileStream = new FileStream(Path.GetFullPath(filePath),FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
            _fileStreamReader = new StreamReader(_fileStream,Encoding.UTF8);
            var txt = _fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(filePath, txt));
        }

        protected override void OnReceive(object message)
        {
            if (message is FileWrite)
            {
                var text = _fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    _reporterActor.Tell(text);
                }
            }
            else if (message is FileError)
            {
                var fe = message as FileError;
                _reporterActor.Tell(string.Format("Tail Error: {0}",fe.Message));
            }
            else if (message is InitialRead)
            {
                var ir = message as InitialRead;
                _reporterActor.Tell(ir.Text);
            }
        }

        public class FileError
        {
            public string FileNameOnly { get; private set; }
            public string Message { get; private set; }

            public FileError(string fileNameOnly, string message)
            {
                FileNameOnly = fileNameOnly;
                Message = message;
            }
        }

        public class FileWrite
        {
            public string Name { get; private set; }

            public FileWrite(string name)
            {
                Name = name;
            }
        }

        public class InitialRead
        {
            public string FileName { get; private set; }
            public string Text { get; private set; }

            public InitialRead(string fileName, string text)
            {
                FileName = fileName;
                Text = text;
            }
        }
    }
}
