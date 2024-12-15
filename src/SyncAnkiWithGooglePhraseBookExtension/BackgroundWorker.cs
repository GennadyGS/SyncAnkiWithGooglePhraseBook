using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazor.BrowserExtension;
using WebExtensions.Net.ActionNs;
using WebExtensions.Net.Tabs;

namespace SyncAnkiWithGooglePhraseBookExtension;

public partial class BackgroundWorker : BackgroundWorkerBase
{
    [GeneratedRegex(@"^https://docs\.google\.com/spreadsheets/d/([a-zA-Z0-9-_]+)")]
    private static partial Regex GoogleSheetUrlRegex();

    [BackgroundWorkerMain]
    public override void Main()
    {
        WebExtensions.Action.OnClicked.AddListener(OnClick);
    }


    public async Task OnClick(Tab tab, OnClickData data)
    {
        if (tab is not { Url: { } url })
        {
            return;
        }

        var match = GoogleSheetUrlRegex().Match(url);
        if (!match.Success)
        {
            return;
        }

        var sheetId = match.Groups[1].Value;
        var redirectUrl = $"exportGooglePhraseBookToAnki://open?spreadSheetId={sheetId}";
        var updateProperties = new UpdateProperties { Url = redirectUrl };
        await WebExtensions.Tabs.Update(tab.Id, updateProperties);
    }
}