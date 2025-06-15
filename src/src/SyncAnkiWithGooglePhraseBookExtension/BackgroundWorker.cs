using System.Text.RegularExpressions;
using Blazor.BrowserExtension;
using Microsoft.JSInterop;
using WebExtensions.Net.ActionNs;
using WebExtensions.Net.Tabs;

namespace SyncAnkiWithGooglePhraseBookExtension;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Code Smell",
    "S1144:Unused private types or members should be removed",
    Justification = "Pending")]
public partial class BackgroundWorker(IJSRuntime jsRuntime) : BackgroundWorkerBase
{
    private const string GoogleTranslateUrl = "https://translate.google.com/saved";
    private const string ExportButtonSelector = "button[aria-label=\"Export to Google Sheets\"]";

    private readonly IJSRuntime _jsRuntime = jsRuntime;

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
            await Task.Delay(TimeSpan.FromSeconds(2));
            var tabId = tab.Id ?? throw new InvalidOperationException("Tab ID is not available");
            await ClickButtonBySelectorAsync(ExportButtonSelector, tabId);
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

    private async Task ClickButtonBySelectorAsync(string selector, int tabId)
    {
        var message = new
        {
            command = "clickButton",
            selector,
        };
        await _jsRuntime.InvokeVoidAsync("chrome.tabs.sendMessage", tabId, message);
    }
}
