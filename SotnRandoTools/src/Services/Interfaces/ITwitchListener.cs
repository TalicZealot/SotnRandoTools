using System.Threading.Tasks;

namespace SotnRandoTools.Services.Interfaces
{
	public interface ITwitchListener
	{
		Task<Models.Authorization> Listen();
		void Stop();
	}
}
