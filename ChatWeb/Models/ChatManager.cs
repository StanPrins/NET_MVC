using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatWeb.Hubs;

namespace ChatWeb.Models
{
    public class ChatManager
    {
        public static String publicRoomName = "PUBLIC_ROOM";
        public static String channelRoomName = "PUBLIC_CHANNEL";


        private static void parseMessage(string messsge)
        {

        }

        //public static void onChatEvent(ChatHub hub, string chatEvent)
        //{
        //    // log the event
        //    Console.Out.WriteLine(chatEvent);
        //    Console.Out.Flush();
        //}

        //public static void onChannleMessage(ChatHub hub, string sendername, string message, string roomName, string strdate)
        //{
        //    // log message
        //    string response = "RELAYED BY SERVER: " + message;

        //    hub.RelayMessage("SERVER", response, publicRoomName, DateTime.UtcNow.Ticks.ToString());
        //}

        //public static void handleChannelMessage(string sendername, string message, string strdate)
        //{

        //}
    }
}