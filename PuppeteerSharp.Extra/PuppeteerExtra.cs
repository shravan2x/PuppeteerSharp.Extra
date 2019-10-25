using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp.Extra
{
    public class PuppeteerExtra
    {
        private readonly List<PuppeteerExtraPlugin> _plugins;

        public PuppeteerExtra()
        {
            _plugins = new List<PuppeteerExtraPlugin>();
        }

        public PuppeteerExtra Use(PuppeteerExtraPlugin plugin)
        {
            if (plugin.Name == null)
                throw new ArgumentNullException(nameof(plugin));

            if ((plugin.Requirements & PluginRequirements.DataFromPlugins) == PluginRequirements.DataFromPlugins)
                ;//plugin.getDataFromPlugins = this.getPluginData.bind(this)
            //plugin._register(Object.getPrototypeOf(plugin))
            _plugins.Add(plugin);

            return this;
        }

        public async Task<Browser> LaunchAsync(LaunchOptions options)
        {
            //options = merge(this._defaultLaunchOptions, options)
            ResolvePluginDependencies();
            OrderPlugins();

            foreach (PuppeteerExtraPlugin plugin in _plugins)
                options = (await plugin.BeforeLaunchAsync(options)) ?? options;

            PuppeteerExtraPluginEventOpts opts = new PuppeteerExtraPluginEventOpts("launch", options, DefaultArgs);

            CheckPluginRequirements(opts);

            Browser browser = await Puppeteer.LaunchAsync(options);
            PatchPageCreationMethods(browser);

            foreach (PuppeteerExtraPlugin plugin in _plugins)
                await plugin.BindBrowserEventsAsync(browser, opts);

            return browser;
        }

        public IEnumerable<PuppeteerExtraPlugin> Plugins => _plugins;

        // TODO: Maybe use fully qualified names instead of Types and load using reflection?
        public IEnumerable<Type> PluginTypes => _plugins.Select(x => x.GetType());

        public void ResolvePluginDependencies()
        {
            HashSet<Type> missingPlugins = new HashSet<Type>(_plugins.SelectMany(x => x.GetMissingDependencies(_plugins)));
            if (missingPlugins.Count == 0)
                return;

            foreach (Type type in missingPlugins)
            {
                if (PluginTypes.Contains(type))
                    continue;

                PuppeteerExtraPlugin dep = (PuppeteerExtraPlugin) Activator.CreateInstance(type);
                Use(dep);

                if (dep.Dependencies.Count > 0)
                    ResolvePluginDependencies();
            }
        }

        public void OrderPlugins()
        {
            IReadOnlyList<PuppeteerExtraPlugin> runLast = _plugins.Where(x => (x.Requirements & PluginRequirements.RunLast) == PluginRequirements.RunLast).ToList();
            foreach (PuppeteerExtraPlugin plugin in runLast)
            {
                _plugins.Remove(plugin);
                _plugins.Add(plugin);
            }
        }

        public void CheckPluginRequirements(PuppeteerExtraPluginEventOpts opts)
        {
            foreach (PuppeteerExtraPlugin plugin in _plugins)
            {
                if (opts.Context == "launch" && ((plugin.Requirements & PluginRequirements.Headful) == PluginRequirements.Headful) && opts.Options.Headless)
                    throw new InvalidOperationException($"Plugin '{plugin.Name}' is not supported in headless mode.");
                if (opts.Context == "connect" && ((plugin.Requirements & PluginRequirements.Launch) == PluginRequirements.Launch))
                    throw new InvalidOperationException($"Plugin '{plugin.Name}' doesn't support puppeteer.connect().");
            }
        }

        public static string[] DefaultArgs()
        {
            return Puppeteer.DefaultArgs;
        }

        private static void PatchPageCreationMethods(Browser browser)
        {
            browser.TargetCreated += (sender, args) =>
            {
                //TODO
            };
        }
    }
}
