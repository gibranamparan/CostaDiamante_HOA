using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace CostaDiamante_HOA
{
    //[Microsoft.AspNet.SignalR.Hubs.HubName("mailerNotifierHub")]
    public class MailerNotifierHub : Hub
    {
        //public MailerResult resultNotify { get; set; }

        /*public MailerNotifierHub() { }
        public MailerNotifierHub(MailerResult res) {
            this.resultNotify = res;
        }*/

        public void Send(GeneralTools.MailerSendGrid.MailerResult mailerResult)
        {
            // Call the broadcastMessage method to update clients.
            //Clients.All.broadcastMessage(this.resultNotify, message);
            Clients.All.broadcastMessage(mailerResult);
        }
    }
}