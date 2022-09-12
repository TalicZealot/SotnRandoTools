using System.Threading.Tasks;

namespace SotnRandoTools.Services.Interfaces
{
	internal interface ITwitchListener
	{
		Task<Models.Authorization> Listen();
		void Stop();
	}
}
