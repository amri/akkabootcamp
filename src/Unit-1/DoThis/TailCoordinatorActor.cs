using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    class TailCoordinatorActor:UntypedActor
    {
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x =>
            {
                if (x is ArithmeticException)
                {
                    return Directive.Resume;
                }
                else if (x is NotSupportedException)
                {
                    return Directive.Stop;
                }
                else
                {
                    return Directive.Restart;
                }
            });
        }

        protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = message as StartTail;
                var tailActorProps = Props.Create<TailActor>(msg.ConsoleWriterActor,msg.Msg);
                var tailActor = Context.ActorOf(tailActorProps, "TailActor");

            }
        }

        public class StartTail
        {
            public string Msg { get; private set; }
            public IActorRef ConsoleWriterActor { get; private set; }

            public StartTail(string msg, IActorRef consoleWriterActor)
            {
                Msg = msg;
                ConsoleWriterActor = consoleWriterActor;
            }
        }

        public class StopTail
        {
            public string Msg { get; private set; }

            public StopTail(string msg)
            {
                Msg = msg;
            }
        }
    }
}
