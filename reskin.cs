﻿using System;
using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.Networking.Packets.Client;
using System.Threading;
using Lib_K_Relay.Utilities;
using System.Linq;
using Helper;
using System.Net;
using System.Diagnostics;

namespace SkinSelector
{
    public class SkinChanger : IPlugin
    {
        public string GetAuthor() => "CD";
        public string GetName() => "Reskin changer";
        public string GetDescription() => "Reskin in any map, limitation is the skins you own, you can find your skinIDs from https://static.drips.pw/rotmg/production/current/json/Objects.json";
        public string[] GetCommands() => new string[] { "[/reskin Id1 Id2 Id3]", "[/skin Id]", "[/use] displays command list" };
        public void Initialize(Proxy proxy)
        {
            proxy.HookCommand("reskin", ChangeSkin);
            proxy.HookCommand("skin", Skin);
            proxy.HookCommand("use", Helper);
        }
        private const short Timeout = 2000;
        bool enabled;
        private void ChangeSkin(Client client, string command, string[] args)
        {
            ReskinPacket reskin = (ReskinPacket)Packet.Create(PacketType.RESKIN);
            short pd = 8000; //up this if server very laggy
            try
            {
                client.Notify("How many skins do you want to switch?");
                int iterateskin = int.Parse(args[0]); 
                enabled = true;
            Label_0000:
                PluginUtils.Delay(pd, delegate
                {
                    reskin.SkinId = int.Parse(args[iterateskin]); //iterate new array string by input
                    client.SendToServer(reskin);
                });
                Thread.Sleep(Timeout);
                PluginUtils.Delay(pd, delegate
                {
                    reskin.SkinId = int.Parse(args[1]);
                    client.SendToServer(reskin);
                });
                Thread.Sleep(Timeout);
                PluginUtils.Delay(pd, delegate
                {
                    reskin.SkinId = int.Parse(args[2]);
                    client.SendToServer(reskin);
                });
                Thread.Sleep(Timeout);
                goto Label_0000;
            }
            catch (Exception e)
            {
                enabled = false;
#if DEBUG
                PluginUtils.LogPluginException(e, "Something is wrong, dropping connection to avoid any possible DC");
#endif
            }
            if (enabled == false)
            {
                FailurePacket failure = (FailurePacket)Packet.Create(PacketType.FAILURE);
                client.SendToClient(failure);
#if DEBUG
                Console.WriteLine(failure);
#endif
            }
        }

        private void Skin(Client client, string command, string[] args)
        {
            ReskinPacket reskin = (ReskinPacket)Packet.Create(PacketType.RESKIN);
            try
            {
                if (args.Length > 0 && !String.IsNullOrEmpty(args[0]))
                {
                    reskin.SkinId = int.Parse(args[0]);
                }
                client.SendToServer(reskin);
            }
            catch (Exception ex)
            {
                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Please enter a skinType Id!"));
            }
        }
        private void Helper(Client client, string command, string[] args)
        {
            string url = "https://static.drips.pw/rotmg/production/current/json/Objects.json";
            try
            {
                using (var cl = new WebClient())
                {
                    try
                    {
                        cl.DownloadFile("url", "Objects.json");
                    }
                    catch(Exception q)
                    {
                        client.Notify("Download Failed!\n You can do it yourself from:"+url);
                    }
                    client.Notify("Download Completed! Find skins Ids in Objects.Json!");
                    try
                    {Process.Start("Objects.json");}
                    catch (Exception q)
                    {
                        client.Notify(q.ToString()+"       Opening failed");
                    }
                };
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
            }
            if (args?.Any() == true || (args.Length == 0))
            {
                client.Notify("[/reskin] resets to original skin!");
                client.Notify("[/skin ID] aka skinType)");
                client.Notify("[/reskin ID1 ID2 ID3] (this may cause unstable gameplay!)");
            }

        }
    }
}