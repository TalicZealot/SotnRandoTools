using System.ComponentModel;

namespace SotnRandoTools.Khaos.Models
{
	public class KhaosViewModel : INotifyPropertyChanged
	{
		private bool started;

		public bool Started
		{
			get
			{
				return started;
			}
			set
			{
				started = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Started"));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
