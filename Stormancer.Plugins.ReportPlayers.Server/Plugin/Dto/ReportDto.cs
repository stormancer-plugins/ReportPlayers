using MsgPack.Serialization;
using Newtonsoft.Json.Linq;

namespace Stormancer.Server.ReportPlayers
{
    public class ReportDto
    {
        [MessagePackMember(0)]
        public string ReportedUserId;
        [MessagePackMember(1)]
        public long Timestamp;
        [MessagePackMember(2)]
        public string Category;
        [MessagePackMember(3)]
        public string Message;        
        [MessagePackMember(4)]
        public JObject CustomData;
    }
}