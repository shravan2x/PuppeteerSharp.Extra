using System.Threading.Tasks;

namespace PuppeteerSharp.Extra.Evasions
{
    public class ChromeRuntimeEvasionPlugin : PuppeteerExtraPlugin
    {
        public override string Name => "stealth/evasions/chrome.runtime";

        public override async Task OnPageCreatedAsync(Page page)
        {
            await page.EvaluateFunctionOnNewDocumentAsync(@"() => {
              window.chrome = {
                runtime: {}
              }
            }");
        }
    }
}
