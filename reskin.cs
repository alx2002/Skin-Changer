using System;
using System.Linq;
using System.Net;
using System.Threading;
using Helper;
using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.Server;
using static System.Diagnostics.Process;
using static Lib_K_Relay.Utilities.PluginUtils;

namespace SkinChanger
{
    public class SkinChanger : IPlugin
    {
        private const short Timeout = 2000;
        private bool enabled;

        public string GetAuthor() => "CD";

        public string GetName() => "Reskin changer";

        public string GetDescription() => "Reskin in any map, limitation is the skins you own, you can find your skinIDs from https://static.drips.pw/rotmg/production/current/json/Objects.json";

        public string[] GetCommands() => new[] {"[/reskin Id1 Id2 Id3]", "[/skin Id]", "[/use] displays command list"};

        public void Initialize(Proxy proxy)
        {
            proxy.HookCommand("reskin", ChangeSkin);
            proxy.HookCommand("skin", Skin);
            proxy.HookCommand("use", Helper);
        }
        private void ChangeSkin(Client client, string command, string[] args)
        {
            var reskin = (ReskinPacket) Packet.Create(PacketType.RESKIN);
            short pd = 8000; //increase if server is laggy 
            var iterateskin = int.Parse(args[0]);
            void SkinIndex()
            {
                if (reskin != null)
                {
                    reskin.SkinId = int.Parse(args[iterateskin]); //iterate new array string by input
                    client.SendToServer(reskin);
                }
                Thread.Sleep(Timeout);
            }
            try
            {
                client.Notify("How many skins do you want to switch?");
                enabled = true;
                Label_0000:
                Delay(pd, SkinIndex);
                goto Label_0000;
            }
            catch (Exception e)
            {
                enabled = false;
#if DEBUG
                LogPluginException(e, "Something is wrong, dropping connection to avoid any possible DC");
#endif
            }

            switch (enabled)
            {
                case false:
                {
                    var failure = (FailurePacket) Packet.Create(PacketType.FAILURE);
                    client.SendToClient(failure);
#if DEBUG
                    Console.WriteLine(failure);
#endif
                    break;
                }
            }
        }

        internal void Skin(Client client, string command, string[] args)
        {
            var reskin = (ReskinPacket) Packet.Create(PacketType.RESKIN);
            try
            {
                if (args.Length > 0 && !string.IsNullOrEmpty(args[0])) reskin.SkinId = int.Parse(args[0]);
                client.SendToServer(reskin);
            }
            catch (Exception ex)
            {
                client.SendToClient(CreateNotification(client.ObjectId, "Please enter a skinType Id!"));
            }
        }

        internal void Helper(Client client, string command, string[] args)
        {
            const string url = "https://static.drips.pw/rotmg/production/current/json/Objects.json";
            try
            {
                using (var cl = new WebClient())
                {
                    try
                    {
                        cl.DownloadFile("url", "Objects.json");
                    }
                    catch (Exception q)
                    {
                        client.Notify("Download Failed!\n You can do it yourself from:" + url);
                    }

                    client.Notify("Download Completed! Find skins Ids in Objects.Json!");
                    try
                    {
                        Start("Objects.json");
                    }
                    catch (Exception q)
                    {
                        client.Notify(q + "       Opening failed");
                    }
                };
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
            }

            if (args?.Any() == true || args.Length == 0)
            {
                client.Notify("[/reskin] resets to original skin!");
                client.Notify("[/skin ID] aka skinType)");
                client.Notify("[/reskin ID1 ID2 ID3] (this may cause unstable gameplay!)");
            }
        }
    }
}