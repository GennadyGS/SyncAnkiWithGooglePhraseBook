using System.Text.RegularExpressions;
using Blazor.BrowserExtension;
using WebExtensions.Net.ActionNs;
using WebExtensions.Net.Tabs;

namespace SyncAnkiWithGooglePhraseBookExtension;

public partial class BackgroundWorker : BackgroundWorkerBase
{
    private const string GoogleTranslateUrl = "https://translate.google.com/saved";

    [GeneratedRegex(@"^https://docs\.google\.com/spreadsheets/d/([a-zA-Z0-9-_]+)")]
    private static partial Regex GoogleSheetUrlRegex { get; }

    [BackgroundWorkerMain]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Reliability",
        "CA2012:Use ValueTasks correctly",
        Justification = "Code is coming from official template")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage",
        "VSTHRD110:Observe result of async calls",
        Justification = "Code is coming from official template")]
    public override void Main()
    {
        WebExtensions.Action.OnClicked.AddListener(OnClickAsync);
    }

    public async Task OnClickAsync(Tab tab, OnClickData data)
    {
        if (tab is not { Url: { } url })
        {
            return;
        }

        var match = GoogleSheetUrlRegex.Match(url);
        if (!match.Success)
        {
            await NavigateToUrlAsync(tab, new Uri(GoogleTranslateUrl));
            return;
        }

        var sheetId = match.Groups[1].Value;
        var redirectUrl = new Uri($"exportGooglePhraseBookToAnki://open?spreadSheetId={sheetId}");
        await NavigateToUrlAsync(tab, redirectUrl);
    }

    private async Task NavigateToUrlAsync(Tab tab, Uri uri)
    {
        var updateProperties = new UpdateProperties { Url = uri.ToString() };
        await WebExtensions.Tabs.Update(tab.Id, updateProperties);
    }
}
