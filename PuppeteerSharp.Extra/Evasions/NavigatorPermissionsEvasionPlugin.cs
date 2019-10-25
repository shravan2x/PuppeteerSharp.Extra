using System.Threading.Tasks;

namespace PuppeteerSharp.Extra.Evasions
{
    public class NavigatorPermissionsEvasionPlugin : PuppeteerExtraPlugin
    {
        public override string Name => "stealth/evasions/navigator.permissions";

        public override async Task OnPageCreatedAsync(Page page)
        {
            await page.EvaluateFunctionOnNewDocumentAsync(@"() => {
              const originalQuery = window.navigator.permissions.query
              // eslint-disable-next-line
              window.navigator.permissions.__proto__.query = parameters =>
                parameters.name === 'notifications'
                  ? Promise.resolve({ state: Notification.permission }) //eslint-disable-line
                  : originalQuery(parameters)

              // Inspired by: https://github.com/ikarienator/phantomjs_hide_and_seek/blob/master/5.spoofFunctionBind.js
              const oldCall = Function.prototype.call
              function call () {
                return oldCall.apply(this, arguments)
              }
              // eslint-disable-next-line
              Function.prototype.call = call

              const nativeToStringFunctionString = Error.toString().replace(
                /Error/g,
                'toString'
              )
              const oldToString = Function.prototype.toString

              function functionToString () {
                if (this === window.navigator.permissions.query) {
                  return 'function query() { [native code] }'
                }
                if (this === functionToString) {
                  return nativeToStringFunctionString
                }
                return oldCall.call(oldToString, this)
              }
              // eslint-disable-next-line
              Function.prototype.toString = functionToString
            }");
        }
    }
}
