using System.ServiceModel;
using System.Threading.Tasks;

namespace minibus
{
	[ServiceContract(CallbackContract = typeof(IGatewayTransferCallback), Namespace = "urn:ps", SessionMode = SessionMode.Required)]
	public interface IGatewayTransferService
	{
		[OperationContract(IsOneWay = true)]
		Task PingAsync();

		[OperationContract(IsOneWay = true)]
		Task TransferAsync(byte[] packet);
	}
}
