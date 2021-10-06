#if XAMARIN
namespace Uno.UI.Controls
{
	public  partial class CommandBarOverflowPresenter : ItemsControl
	{
		public CommandBarOverflowPresenter() : base()
		{
			ItemsPanel = new ItemsPanelTemplate(() => new StackPanel());
		}
	}
}
#endif
