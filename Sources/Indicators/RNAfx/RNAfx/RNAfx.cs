/*
The MIT License (MIT)

Copyright (c) 2014 RNAfx

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using cAlgo.API;
using System.Threading;
using System.Collections;
using System.Globalization;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace cAlgo
{

    class Message
    {
        public const string OPEN = "o";

        public const string HEARTBEAT = "h";

        public const string FEED_INSERT = "feed:insert";

        public const string FEED_CHANNELS = "feed:channels";

        public const string FEED_SNAPSHOT = "feed:snapshot";

        public const string CHANNEL_CONNECT = "channel:connect";

        public string action { get; set; }

        public string channel { get; set; }

        public dynamic data { get; set; }
    }

    delegate void OnConnectionOpenDelegate(object sender, EventArgs e);

    delegate void OnMessageDelegate(object sender, MessageReceivedEventArgs e);

    delegate void OnConnectionErrorDelegate(object sender, ErrorEventArgs e);

    class Transport
    {
        protected WebSocket Connection;

        public Transport(string WebSocketUrl)
        {
            Connection = new WebSocket(WebSocketUrl);
        }

        public OnConnectionOpenDelegate OnConnectionOpen { get; set; }

        public OnMessageDelegate OnMessage { get; set; }

        public OnConnectionErrorDelegate OnConnectionError { get; set; }

        public void Init()
        {
            if (OnConnectionOpen != null)
            {
                Connection.Opened += new EventHandler(OnConnectionOpen);
            }

            if (OnMessage != null)
            {
                Connection.MessageReceived += new EventHandler<MessageReceivedEventArgs>(OnMessage);
            }

            if (OnConnectionError != null)
            {
                Connection.Error += new EventHandler<ErrorEventArgs>(OnConnectionError);
            }
        }

        public void Send(string msg)
        {
            Connection.Send(msg);
        }

        public void Connect()
        {
            Connection.Open();
        }

    }

    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class RNAfx : Indicator
    {

        [Parameter("EndPoint", DefaultValue = "ws://rnafx.com/demo/websocket")]
        public string EndPoint { get; set; }

        [Parameter("MaxItems", DefaultValue = 9)]
        public int MaxItems { get; set; }

        Transport transport = null;

        string FeedItemSeparator = "\n";

        List<string> messages = new List<string>();

        CultureInfo enUS = new CultureInfo("en-US");

        protected override void Initialize()
        {

            //ChartObjects.DrawText("meho", "ALini", StaticPosition.TopLeft, Colors.LimeGreen);

            transport = new Transport(EndPoint);
            transport.OnConnectionOpen = onConnectionOpen;
            transport.OnMessage = onConnectionMessage;
            transport.OnConnectionError = onConnectionError;
            transport.Init();
            transport.Connect();

            //ChartObjects.DrawText("meho1", "\nALL ARE HAHAH", StaticPosition.TopLeft, Colors.LimeGreen);

        }

        private void onConnectionOpen(object sender, EventArgs e)
        {
            Print("Connection opened");
            //ChartObjects.DrawText("meho", "ALL ARE WELCOME", StaticPosition.TopLeft, Colors.LimeGreen);

        }

        protected void onConnectionMessage(object sender, MessageReceivedEventArgs e)
        {
            string encoded = null;
            Message msg = new Message();

            //ChartObjects.DrawText("meho", "ALL ARE WELCOME", StaticPosition.TopLeft, Colors.LimeGreen);
            //Print(Thread.CurrentThread.Name);

            switch (e.Message)
            {
                case Message.OPEN:
                    Print("Connection allowed by the server");
                    msg.action = Message.FEED_CHANNELS;
                    msg.channel = null;
                    msg.data = null;
                    encoded = JsonConvert.SerializeObject(msg);
                    transport.Send(encoded.ToString());
                    break;
                case Message.HEARTBEAT:
                    Print("Heartbeat");
                    break;
                default:
                    dynamic d = JObject.Parse(e.Message);
                    string action = d.action.ToString();

                    //Print(action.ToUpperInvariant());

                    switch (action)
                    {
                        case Message.FEED_CHANNELS:
                            Print("FEED CHANNELS {0}", d.data);

                            foreach (var channel in d.data)
                            {
                                msg.action = Message.CHANNEL_CONNECT;
                                msg.data = channel;
                                msg.channel = null;

                                encoded = JsonConvert.SerializeObject(msg);
                                transport.Send(encoded.ToString());
                            }

                            break;
                        case Message.CHANNEL_CONNECT:
                            Print("CHANNEL CONNECTED {0}", d.channel.ToString());

                            msg.action = Message.FEED_SNAPSHOT;
                            msg.channel = d.channel.ToString();
                            msg.data = MaxItems;

                            encoded = JsonConvert.SerializeObject(msg);
                            transport.Send(encoded.ToString());

                            break;
                        case Message.FEED_INSERT:

                            string channelo = d.channel.ToString();
                            switch (channelo)
                            {
                                case "twitter":
                                    int idx = 0;
                                    string sep = "";
                                    string msgg = FormatTwitterMessage(d.data);
                                    messages.Insert(0, msgg);
                                    messages.RemoveAt(messages.Count - 1);

                                    Print("FEED INSERT FOR TWITTER {0}", msgg);

                                    ChartObjects.RemoveAllObjects();

                                    foreach (string mmsg in messages)
                                    {
                                        ChartObjects.DrawText("meh" + (idx++), sep + mmsg, StaticPosition.TopLeft, Colors.LimeGreen);
                                        sep += FeedItemSeparator;
                                    }

                                    break;
                                default:
                                    //Print("\t\tUNHANDLED channel {0}", channelo);

                                    break;
                            }

                            break;
                        case Message.FEED_SNAPSHOT:
                            Print("FEED SNAPSHOT for {0}", d.channel.ToString());

                            string chl = d.channel.ToString();
                            switch (chl)
                            {
                                case "twitter":
                                    int Index = 0;
                                    string Separatorr = "";

                                    Print("SNASHOT TWITTER RECEIVED");

                                    foreach (var item in d.data)
                                    {
                                        string NewsItem = FormatTwitterMessage(item);
                                        ChartObjects.RemoveObject("meh" + Index);
                                        ChartObjects.DrawText("meh" + (Index++), Separatorr + NewsItem.ToString(), StaticPosition.TopLeft, Colors.LimeGreen);
                                        Separatorr += FeedItemSeparator;
                                        messages.Insert(0, NewsItem);
                                        Print("TWITTER SNAPSHOT ITEM {0} - {1}", Index, NewsItem);
                                    }

                                    break;
                                default:
                                    //Print("\t\tUNHANDLED channel {0}", chl);

                                    break;
                            }

                            break;
                        default:
                            string chal = d.channel.ToString();
                            //Print("\tUNHANDLED action {0} for channel {1}", action, chal);
                            break;
                    }

                    break;
            }
        }

        protected string FormatTwitterMessage(dynamic message)
        {
            //const string format = "ddd MMM dd HH:mm:ss zzzz yyyy";
            return string.Format("{0} :: {1}", message.created_at, message.text).Replace("\n", " ");
        }

        protected void onConnectionError(object sender, ErrorEventArgs e)
        {
            Print("Connection error {0}", e.ToString());
        }

        public override void Calculate(int index)
        {
            //DO NOTHING HERE
        }

    }
}
