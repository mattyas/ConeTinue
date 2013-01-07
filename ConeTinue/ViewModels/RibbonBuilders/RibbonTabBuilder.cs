namespace ConeTinue.ViewModels.RibbonBuilders
{
	public class RibbonTabBuilder
	{
		private readonly string displayName;
		private readonly string keyTip;
		private RibbonGroupViewModel[] groups;

		public RibbonTabBuilder(string displayName, string keyTip)
		{
			this.displayName = displayName;
			this.keyTip = keyTip;
		}

		public RibbonTabBuilder WithRibbonGroups(params RibbonGroupViewModel[] groups)
		{
			this.groups = groups;
			return this;
		}

		public RibbonTabViewModel Build()
		{
			var ribbonTabViewModel = new RibbonTabViewModel(keyTip) { DisplayName = displayName };
			ribbonTabViewModel.Groups.AddRange(groups);
			return ribbonTabViewModel;
		}
	}
}