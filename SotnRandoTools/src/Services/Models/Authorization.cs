namespace SotnRandoTools.Services.Models
{
	public class Authorization
	{
		public string Code { get; }

		public Authorization(string code)
		{
			Code = code;
		}
	}
}
