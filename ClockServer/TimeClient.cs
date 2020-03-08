using ClockServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace ClockServer
{
    class TimeClient
    {
        [Obsolete]
        static void Main()
        {
            TcpClientChannel channel = new TcpClientChannel();
            ChannelServices.RegisterChannel(channel);
            RemotingConfiguration.RegisterWellKnownClientType(typeof(Clock), "tcp://localhost:1234/Clock");
            Clock clock = new Clock();
            Console.WriteLine(clock.GetCurrentTime());
        }
    }
}
