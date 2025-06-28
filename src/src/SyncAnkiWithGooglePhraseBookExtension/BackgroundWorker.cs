using System.Text.RegularExpressions;
using Blazor.BrowserExtension;
using Microsoft.JSInterop;
using WebExtensions.Net.ActionNs;
using WebExtensions.Net.Tabs;

namespace SyncAnkiWithGooglePhraseBookExtension;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Code Smell",
    "S1172:Unused method parameters should be removed",
    Justification = "Pending")]
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
        if (tab is not { Url: { } url, Id: { } tabId })
        {
            return;
        }

        if (url.Equals("https://docs.google.com/spreadsheets/import/inline?authuser=0"))
        {
            await ClickButtonBySelectorAsync(tab, "#confirmActionButton");
            return;
        }

        var match = GoogleSheetUrlRegex.Match(url);
        if (!match.Success)
        {
            await WaitForTabLoadAsync(tabId);
            await NavigateToUrlAsync(tabId, new Uri(GoogleTranslateUrl));
            await WaitForTabLoadAsync(tabId);
            await ClickButtonBySelectorAsync(tab, ExportButtonSelector);
            await WaitForTabLoadAsync(tabId);
            return;
        }

        var sheetId = match.Groups[1].Value;
        var redirectUrl = new Uri($"exportGooglePhraseBookToAnki://open?spreadSheetId={sheetId}");
        await NavigateToUrlAsync(tabId, redirectUrl);
    }

    private async Task NavigateToUrlAsync(int tabId, Uri url)
    {
        await LogInfoAsync($"NavigateToUrl {url}");
        var updateProperties = new UpdateProperties { Url = url.ToString() };
        await WebExtensions.Tabs.Update(tabId, updateProperties);
    }

    private async Task ClickButtonBySelectorAsync(Tab tab, string selector)
    {
        var tabId = tab.Id ?? throw new InvalidOperationException("Tab ID is not available");
        await LogInfoAsync($"Click button with selector {selector}");
        await SendMessageAsync(tabId, "clickButton", new { selector });
    }

    private async Task WaitForTabLoadAsync(int tabId, int timeoutMs = 10_000)
    {
        try
        {
            await LogInfoAsync("Waiting for page load.");
            await _jsRuntime.InvokeVoidAsync("waitForTabToLoad", tabId);
            await LogInfoAsync("Page finished loading.");
        }
        catch (JSException e)
        {
            if (e.Message.Contains("Timeout waiting for tab"))
            {
                await LogErrorAsync(tabId, $"Tab {tabId} timed out after waiting: {e.Message}");
            }

            throw;
        }
    }

    private async Task SendMessageAsync(int tabId, string command, object args)
    {
        await _jsRuntime.InvokeVoidAsync("chrome.tabs.sendMessage", tabId, new { command, args });
    }

    private async Task LogInfoAsync(string message)
    {
        await _jsRuntime.InvokeVoidAsync("console.log", message);
    }

    private async Task LogErrorAsync(int tabId, string message)
    {
        await _jsRuntime.InvokeVoidAsync("console.error", message);
    }
}
