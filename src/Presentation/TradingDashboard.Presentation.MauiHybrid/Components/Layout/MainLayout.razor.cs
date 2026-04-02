namespace TradingDashboard.Presentation.MauiHybrid.Components.Layout;

public partial class MainLayout
{
    protected string CurrentPlatformLabel => DeviceInfo.Platform.ToString();
}
