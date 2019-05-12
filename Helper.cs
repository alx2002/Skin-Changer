using Lib_K_Relay.Networking;
using Lib_K_Relay.Utilities;

namespace Helper
{
    internal static class Extension
    {
        internal static void Notify(this Client client, string text)
        {
            client.SendToClient(PluginUtils.CreateOryxNotification("SkinChanger", text));
        }
    }

}