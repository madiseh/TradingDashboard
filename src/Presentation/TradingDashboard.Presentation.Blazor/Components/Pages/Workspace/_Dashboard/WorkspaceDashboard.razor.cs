using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using TradingDashboard.Presentation.Common.Constants;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Workspace.Dashboard;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Workspace._Dashboard;

public partial class WorkspaceDashboard
{
    [Inject]
    private IStringLocalizer<WorkspaceDashboardResource> L { get; set; } = default!;

    private string HomeSeoTitle => T("Seo.Title");

    private string HomeSeoDescription => T("Seo.Description");

    private string HomeSeoKeywords => T("Seo.Keywords");

    private IReadOnlyList<string> BrokerHighlights =>
    [
        T("Broker.Highlight.RegulationExecution"),
        T("Broker.Highlight.SpreadCommission"),
        T("Broker.Highlight.PlatformStability"),
        T("Broker.Highlight.FundingWithdrawals")
    ];

    private IReadOnlyList<string> ChooseUsPoints =>
    [
        T("Choose.Point1"),
        T("Choose.Point2"),
        T("Choose.Point3"),
        T("Choose.Point4")
    ];

    private IReadOnlyList<string> VideoTopics =>
    [
        T("Video.Topic1"),
        T("Video.Topic2"),
        T("Video.Topic3"),
        T("Video.Topic4")
    ];

    private IReadOnlyList<string> DownloadBullets =>
    [
        T("Download.Bullet1"),
        T("Download.Bullet2"),
        T("Download.Bullet3"),
        T("Download.Bullet4")
    ];

    private IReadOnlyList<PackageCard> Packages =>
    [
        new("icon/chart.png", T("Features.Package.WeeklyCotReview.Title"), T("Features.Package.WeeklyCotReview.Description")),
        new("icon/rate.png", T("Features.Package.CurrencyStrengthScanner.Title"), T("Features.Package.CurrencyStrengthScanner.Description")),
        new("icon/interest-rate.png", T("Features.Package.MomentumShiftTracker.Title"), T("Features.Package.MomentumShiftTracker.Description")),
        new("icon/business.png", T("Features.Package.BrokerConditionWatchlist.Title"), T("Features.Package.BrokerConditionWatchlist.Description")),
        new("icon/business.png", T("Features.Package.TraderPlaybookLibrary.Title"), T("Features.Package.TraderPlaybookLibrary.Description"))
    ];

    private IReadOnlyList<BrokerReview> Brokers =>
    [
        new("01", "broker/ic-markets.png", "IC Markets", T("Broker.Item.ICMarkets.Description"), "44K+", "broker-detail-01", true),
        new("02", "broker/robofores.png", "RoboForex", T("Broker.Item.RoboForex.Description"), "44K+", "broker-detail-02", false),
        new("03", "broker/etoro.png", "eToro", T("Broker.Item.EToro.Description"), "25K+", "broker-detail-03", false),
        new("04", "broker/peppershone.png", "Pepperstone", T("Broker.Item.Pepperstone.Description"), "44K+", "broker-detail-04", false)
    ];

    private IReadOnlyList<WorkingStep> WorkingSteps =>
    [
        new("01", "icon/customer-support.png", T("Workflow.Step1.Title"), T("Workflow.Step1.Description")),
        new("02", "icon/choose.png", T("Workflow.Step2.Title"), T("Workflow.Step2.Description")),
        new("03", "icon/select-account.png", T("Workflow.Step3.Title"), T("Workflow.Step3.Description")),
        new("04", "icon/profit-sharing.png", T("Workflow.Step4.Title"), T("Workflow.Step4.Description"))
    ];

    private IReadOnlyList<AboutFeature> AboutFeatures =>
    [
        new("icon/knowledge.png", T("About.Feature1.Title"), T("About.Feature1.Description")),
        new("icon/advice.png", T("About.Feature2.Title"), T("About.Feature2.Description")),
        new("icon/setting.png", T("About.Feature3.Title"), T("About.Feature3.Description")),
        new("icon/idea.png", T("About.Feature4.Title"), T("About.Feature4.Description"))
    ];

    private IReadOnlyList<TestimonialCard> Testimonials =>
    [
        new("testimonial/balogh.png", T("Testimonials.Item1.Quote"), "Balogh Imre", T("Testimonials.Item1.Role")),
        new("testimonial/kende.png", T("Testimonials.Item2.Quote"), "Kende Attila", T("Testimonials.Item2.Role")),
        new("testimonial/balogh.png", T("Testimonials.Item3.Quote"), "Eleanor Pena", T("Testimonials.Item3.Role"))
    ];

    private IReadOnlyList<FaqItem> LeftFaqs =>
    [
        new(T("Faq.Left1.Question"), T("Faq.Left1.Answer"), "faq-accordion-1", true),
        new(T("Faq.Left2.Question"), T("Faq.Left2.Answer"), "faq-accordion-2", false),
        new(T("Faq.Left3.Question"), T("Faq.Left3.Answer"), "faq-accordion-3", false),
        new(T("Faq.Left4.Question"), T("Faq.Left4.Answer"), "faq-accordion-4", false),
        new(T("Faq.Left5.Question"), T("Faq.Left5.Answer"), "faq-accordion-5", false)
    ];

    private IReadOnlyList<FaqItem> RightFaqs =>
    [
        new(T("Faq.Right1.Question"), T("Faq.Right1.Answer"), "faq2-accordion-1", false),
        new(T("Faq.Right2.Question"), T("Faq.Right2.Answer"), "faq2-accordion-2", false),
        new(T("Faq.Right3.Question"), T("Faq.Right3.Answer"), "faq2-accordion-3", false),
        new(T("Faq.Right4.Question"), T("Faq.Right4.Answer"), "faq2-accordion-4", false),
        new(T("Faq.Right5.Question"), T("Faq.Right5.Answer"), "faq2-accordion-5", false)
    ];

    private string Img(string relativePath) => SiteBranding.Image(relativePath);

    private string T(string key) => L[key].Value;

    private string GetIconAlt(string title) => L["Common.IconAlt", title].Value;

    private sealed record PackageCard(string Icon, string Title, string Description);

    private sealed record BrokerReview(string Number, string Image, string Name, string Description, string Reviews, string CollapseId, bool IsExpanded);

    private sealed record WorkingStep(string Number, string Icon, string Title, string Description);

    private sealed record AboutFeature(string Icon, string Title, string Description);

    private sealed record TestimonialCard(string Image, string Quote, string Name, string Role);

    private sealed record FaqItem(string Question, string Answer, string CollapseId, bool Expanded);
}
