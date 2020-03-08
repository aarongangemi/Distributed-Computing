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
    class TimeServer
    {
        [Obsolete]
        static void Main()
        {
            TcpServerChannel channel = new TcpServerChannel(1234);
            ChannelServices.RegisterChannel(channel);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Clock), "Clock", WellKnownObjectMode.SingleCall);
            Console.WriteLine("Press enter to terminate");
            Console.ReadLine();
        }
    }
}
