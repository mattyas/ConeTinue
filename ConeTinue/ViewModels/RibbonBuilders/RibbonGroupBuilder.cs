namespace ConeTinue.ViewModels.RibbonBuilders
{
	public class RibbonGroupBuilder
	{
		private readonly string displayName;
		private IRibbonControlViewModel[] buttons = new IRibbonControlViewModel[0];
		private IRibbonControlViewProvider provider;

		public RibbonGroupBuilder(string displayName)
		{
			this.displayName = displayName;
		}

		public RibbonGroupViewModel Build()
		{
			var ribbonGroup = new RibbonGroupViewModel { DisplayName = displayName };

			ribbonGroup.Items.AddRange(buttons);
			if (provider != null)
				ribbonGroup.SetProvider(provider);
			return ribbonGroup;
		}

		public RibbonGroupBuilder WithItems(params IRibbonControlViewModel[] buttons)
		{
			this.buttons = buttons;
			return this;
		}

		public RibbonGroupBuilder WithItemsProvider(IRibbonControlViewProvider provider)
		{
			this.provider = provider;
			return this;
		}
	}
}