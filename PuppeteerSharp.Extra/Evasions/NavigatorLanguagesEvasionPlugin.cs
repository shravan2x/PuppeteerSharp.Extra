using System.Threading.Tasks;

namespace PuppeteerSharp.Extra.Evasions
{
    public class NavigatorLanguagesEvasionPlugin : PuppeteerExtraPlugin
    {
        public override string Name => "stealth/evasions/navigator.languages";

        public override async Task OnPageCreatedAsync(Page page)
        {
            await page.EvaluateFunctionOnNewDocumentAsync(@"() => {
              // Overwrite the `plugins` property to use a custom getter.
              Object.defineProperty(navigator, 'languages', {
                get: () => ['en-US', 'en']
              })
            }");
        }
    }
}
