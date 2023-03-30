using System.ServiceModel;
using System.Threading.Tasks;


namespace minibus
{
	public interface IGatewayTransferCallback
	{
		[OperationContract(IsOneWay = true)]
		Task TransferCallbackAsync(byte[] packet);
	}
}
