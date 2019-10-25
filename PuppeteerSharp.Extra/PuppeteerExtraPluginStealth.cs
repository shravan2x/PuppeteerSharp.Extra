using PuppeteerSharp.Extra.Evasions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PuppeteerSharp.Extra
{
    public class PuppeteerExtraPluginStealth : PuppeteerExtraPlugin
    {
        private static readonly IReadOnlyDictionary<string, Type> EvasionPluginTypes = new Dictionary<string, Type>
        {
            { "navigator.languages", typeof(NavigatorLanguagesEvasionPlugin) },
            { "navigator.plugins", typeof(NavigatorPluginsEvasionPlugin) },
            { "window.outerdimensions", typeof(WindowOuterDimensionsEvasionPlugin) }
        };

        public override string Name => "stealth";

        public override IPluginOptions Defaults
        {
            get
            {
                HashSet<string> availableEvasions = new HashSet<string>
                {
                    //"chrome.runtime",
                    //"console.debug",
                    "navigator.languages",
                    //"navigator.permissions",
                    //"navigator.webdriver",
                    "navigator.plugins",
                    "window.outerdimensions",
                    //"webgl.vendor",
                    //"user-agent"
                };

                return new Options
                {
                    AvailableEvasions = availableEvasions,
                    EnabledEvasions = new HashSet<string>(availableEvasions)
                };
            }
        }

        public override ISet<Type> Dependencies => new HashSet<Type>(((Options) Opts).EnabledEvasions.Select(x => EvasionPluginTypes[x]));

        public HashSet<string> AvailableEvasions => ((Options) Defaults).AvailableEvasions;

        public HashSet<string> EnabledEvasions
        {
            get => ((Options) Opts).EnabledEvasions;
            set => ((Options) Opts).EnabledEvasions = value;
        }

        public class Options : IPluginOptions
        {
            public HashSet<string> AvailableEvasions { get; set; }
            public HashSet<string> EnabledEvasions { get; set; }
        }
    }
}
