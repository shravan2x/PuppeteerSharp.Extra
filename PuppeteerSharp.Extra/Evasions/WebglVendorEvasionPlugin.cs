using System.Threading.Tasks;

namespace PuppeteerSharp.Extra.Evasions
{
    public class WebglVendorEvasionPlugin : PuppeteerExtraPlugin
    {
        public override string Name => "stealth/evasions/webgl.vendor";

        public override async Task OnPageCreatedAsync(Page page)
        {
            await page.EvaluateFunctionOnNewDocumentAsync(@"() => {
              try {
                /* global WebGLRenderingContext */
                const getParameter = WebGLRenderingContext.getParameter
                WebGLRenderingContext.prototype.getParameter = function (parameter) {
                  // UNMASKED_VENDOR_WEBGL
                  if (parameter === 37445) {
                    return 'Intel Inc.'
                  }
                  // UNMASKED_RENDERER_WEBGL
                  if (parameter === 37446) {
                    return 'Intel Iris OpenGL Engine'
                  }
                  return getParameter(parameter)
                }
              } catch (err) {}
            }");
        }
    }
}
