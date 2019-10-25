using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PuppeteerSharp.Extra
{
    public class AnonymizeUaPlugin : PuppeteerExtraPlugin
    {
        private static readonly Regex MakeWindowsRegex = new Regex(@"\(([^)]+)\)");

        public override string Name => "anonymize-ua";

        public override IPluginOptions Defaults => new Options { StripHeadless = true, MakeWindows = true, CustomFn = null };

        public override async Task OnPageCreatedAsync(Page page)
        {
            string ua = await page.Browser.GetUserAgentAsync();
            if (((Options) Opts).StripHeadless)
                ua = ua.Replace("HeadlessChrome/", "Chrome/");
            if (((Options) Opts).MakeWindows)
                ua = MakeWindowsRegex.Replace(ua, "(Windows NT 10.0; Win64; x64)");
            if (((Options) Opts).CustomFn != null)
                ua = ((Options) Opts).CustomFn(ua);

            await page.SetUserAgentAsync(ua);
        }

        public class Options : IPluginOptions
        {
            public bool StripHeadless { get; set; }
            public bool MakeWindows { get; set; }
            public Func<string, string> CustomFn { get; set; }
        }
    }
}
