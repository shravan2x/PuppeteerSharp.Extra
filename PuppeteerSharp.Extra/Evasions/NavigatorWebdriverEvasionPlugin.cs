using System.Threading.Tasks;

namespace PuppeteerSharp.Extra.Evasions
{
    public class NavigatorWebdriverEvasionPlugin : PuppeteerExtraPlugin
    {
        public override string Name => "stealth/evasions/navigator.webdriver";

        public override async Task OnPageCreatedAsync(Page page)
        {
            await page.EvaluateFunctionOnNewDocumentAsync(@"() => {
              // eslint-disable-next-line
              const newProto = navigator.__proto__
              delete newProto.webdriver
              // eslint-disable-next-line
              navigator.__proto__ = newProto
            }");
        }
    }
}
