namespace Generate_Flows_1
{
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class Progress : Dialog
	{
		public Progress(IEngine engine) : base(engine)
		{
		}

		public void ShowLine(string s)
		{
			AddWidget(new Label(s), 0,0);
			IsEnabled = false;
			Show(false);
		}
	}
}