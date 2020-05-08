using ClientServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Tutorial_6_Web_Server.Models
{
    public class ClientModel
    {
        private NetTcpBinding tcp;
        private string URL;
        ChannelFactory<IClient> foobFactory;
        IClient foob;

        public ClientModel()
        {
            tcp = new NetTcpBinding();
            URL = "net.tcp://localhost:8200/DataService";
            foobFactory = new ChannelFactory<IClient>(tcp, URL);
            foob = foobFactory.CreateChannel();
        }

        public void RequestJob()
        {
            foob.RequestJob();
        }

        public void UploadSolution()
        {
            foob.UploadSolution();
        }

    }
}