using System.Threading.Tasks;

namespace PuppeteerSharp.Extra.Evasions
{
    public class ConsoleDebugEvasionPlugin : PuppeteerExtraPlugin
    {
        public override string Name => "stealth/evasions/console.debug";

        public override async Task OnPageCreatedAsync(Page page)
        {
            await page.EvaluateFunctionOnNewDocumentAsync(@"() => {
              window.console.debug = () => {
                return null
              }
            }");
        }
    }
}
