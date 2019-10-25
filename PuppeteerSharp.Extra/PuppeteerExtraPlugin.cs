using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp.Extra
{
    public interface IPluginOptions
    {

    }

    [Flags]
    public enum PluginRequirements
    {
        None = 0x0,
        Launch = 0x1,
        Headful = 0x2,
        DataFromPlugins = 0x4,
        RunLast = 0x8
    }

    public abstract class PuppeteerExtraPlugin
    {
        private readonly IPluginOptions _opts;

        protected PuppeteerExtraPlugin(IPluginOptions opts = null)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            _opts = opts ?? Defaults;
        }

        public abstract string Name { get; }

        public virtual IPluginOptions Defaults => null;

        public virtual PluginRequirements Requirements => PluginRequirements.None;

        public virtual ISet<Type> Dependencies => ImmutableHashSet<Type>.Empty;

        // ReSharper disable once ConvertToAutoProperty
        public IPluginOptions Opts => _opts;

        public virtual Task<LaunchOptions> BeforeLaunchAsync(LaunchOptions options) => Task.FromResult(options);

        public virtual Task AfterLaunchAsync(Browser browser, PuppeteerExtraPluginEventOpts opts) => Task.CompletedTask;

        public virtual Task AfterConnectAsync(Browser browser, PuppeteerExtraPluginEventOpts opts) => Task.CompletedTask;

        public virtual Task OnBrowserAsync(Browser browser, PuppeteerExtraPluginEventOpts opts) => Task.CompletedTask;

        public virtual Task OnTargetCreatedAsync(Target target) => Task.CompletedTask;

        public virtual Task OnPageCreatedAsync(Page page) => Task.CompletedTask;

        public virtual Task OnTargetChangedAsync(Target target) => Task.CompletedTask;

        public virtual Task OnTargetDestroyedAsync(Target target) => Task.CompletedTask;

        public virtual Task OnDisconnectedAsync() => Task.CompletedTask;

        // public virtual Task OnCloseAsync() => Task.CompletedTask;

        internal HashSet<Type> GetMissingDependencies(IEnumerable<PuppeteerExtraPlugin> plugins)
        {
            HashSet<Type> pluginTypes = new HashSet<Type>(plugins.Select(x => x.GetType()));
            HashSet<Type> missing = new HashSet<Type>(Dependencies.Except(pluginTypes));
            return missing;
        }

        internal async Task BindBrowserEventsAsync(Browser browser, PuppeteerExtraPluginEventOpts opts)
        {
            browser.TargetCreated += async (sender, args) => await InternalOnTargetCreatedAsync(args.Target);
            browser.TargetChanged += async (sender, args) => await OnTargetChangedAsync(args.Target);
            browser.TargetDestroyed += async (sender, args) => await OnTargetDestroyedAsync(args.Target);
            browser.Disconnected += async (sender, args) => await OnDisconnectedAsync();

            await AfterLaunchAsync(browser, opts);
            await AfterConnectAsync(browser, opts);
            await OnBrowserAsync(browser, opts);
        }

        private async Task InternalOnTargetCreatedAsync(Target target)
        {
            await OnTargetCreatedAsync(target);

            if (target.Type == TargetType.Page)
            {
                Page page = await target.PageAsync();
                await OnPageCreatedAsync(page);
            }
        }
    }

    public class PuppeteerExtraPluginEventOpts
    {
        public string Context { get; }

        public LaunchOptions Options { get; }

        public Func<string[]> DefaultArgs { get; }

        public PuppeteerExtraPluginEventOpts(string context, LaunchOptions options, Func<string[]> defaultArgs)
        {
            Context = context;
            Options = options;
            DefaultArgs = defaultArgs;
        }
    }
}
