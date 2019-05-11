using Lib_K_Relay.Networking;
using Lib_K_Relay.Utilities;

namespace Helper
{
    public static class Extension
    {
        public static void Notify(this Client client, string text)
        {
            client.SendToClient(PluginUtils.CreateOryxNotification("SkinChanger", text));
        }
    }

}