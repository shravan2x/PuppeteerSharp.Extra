using System.Threading.Tasks;

namespace PuppeteerSharp.Extra.Evasions
{
    public class WindowOuterDimensionsEvasionPlugin : PuppeteerExtraPlugin
    {
        public override string Name => "stealth/evasions/window.outerdimensions";

        public override async Task OnPageCreatedAsync(Page page)
        {
            await page.EvaluateFunctionOnNewDocumentAsync(@"() => {
              try {
                if (window.outerWidth && window.outerHeight) {
                  return // nothing to do here
                }
                const windowFrame = 85 // probably OS and WM dependent
                window.outerWidth = window.innerWidth
                window.outerHeight = window.innerHeight + windowFrame
              } catch (err) {}
            }");
        }

        public override async Task<LaunchOptions> BeforeLaunchAsync(LaunchOptions options)
        {
            options.DefaultViewport = null;
            return options;
        }
    }
}
