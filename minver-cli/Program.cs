using System;
using System.IO;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using MinVer.Lib;

namespace MinVer;

internal static class Program
{
    private static readonly string informationalVersion = typeof(Versioner).Assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().Single().InformationalVersion;

    private static int Main(string[] args)
    {
        using var app = new CommandLineApplication { Name = "minver", FullName = $"MinVer CLI {informationalVersion}", };

        app.HelpOption();

        var workDirArg = app.Argument("workingDirectory", "Working directory (optional)");

        var autoIncrementOption = app.Option("-a|--auto-increment <VERSION_PART>", VersionPartExtensions.ValidValues, CommandOptionType.SingleValue);
        var buildMetaOption = app.Option("-b|--build-metadata <BUILD_METADATA>", "", CommandOptionType.SingleValue);
        var defaultPreReleasePhaseOption = app.Option("-d|--default-pre-release-phase <PHASE>", "alpha (default), preview, etc.", CommandOptionType.SingleValue);
        var ignoreHeightOption = app.Option<bool>("-i|--ignore-height", "Use the latest tag (or root commit) as-is, without adding height", CommandOptionType.NoValue);
        var minMajorMinorOption = app.Option("-m|--minimum-major-minor <MINIMUM_MAJOR_MINOR>", MajorMinor.ValidValues, CommandOptionType.SingleValue);
        var tagPrefixOption = app.Option("-t|--tag-prefix <TAG_PREFIX>", "", CommandOptionType.SingleValue);
        var verbosityOption = app.Option("-v|--verbosity <VERBOSITY>", VerbosityMap.ValidValues, CommandOptionType.SingleValue);
#if MINVER
        var versionOverrideOption = app.Option("-o|--version-override <VERSION>", "", CommandOptionType.SingleValue);
#endif

        app.OnExecute(() =>
        {
            var workDir = workDirArg.Value ?? ".";

            if (!Directory.Exists(workDir))
            {
                Logger.ErrorWorkDirDoesNotExist(workDir);
                return 2;
            }

            if (!Options.TryParse(
                    autoIncrementOption.Value(),
                    buildMetaOption.Value(),
                    defaultPreReleasePhaseOption.Value(),
                    ignoreHeightOption.HasValue() ? true : null,
                    minMajorMinorOption.Value(),
                    tagPrefixOption.Value(),
                    verbosityOption.Value(),
#if MINVER
                    versionOverrideOption.Value(),
#endif
                    out var options))
            {
                return 2;
            }

#if MINVER_CLI
            if (!Options.TryParseEnvVars(out var envOptions))
            {
                return 2;
            }

            options = options.Mask(envOptions);
#endif

            var log = new Logger(options.Verbosity ?? default);

            _ = log.IsDebugEnabled && log.Debug($"MinVer {informationalVersion}.");

            if (options.VersionOverride != null)
            {
                _ = log.IsInfoEnabled && log.Info($"Using version override {options.VersionOverride}.");

                Console.Out.WriteLine(options.VersionOverride);

                return 0;
            }

            var version = Versioner.GetVersion(workDir, options.TagPrefix ?? "", options.MinMajorMinor ?? MajorMinor.Zero, options.BuildMeta ?? "", options.AutoIncrement ?? default, options.DefaultPreReleasePhase ?? "", log, options.IgnoreHeight ?? false);

            Console.Out.WriteLine(version);

            return 0;
        });

        return app.Execute(args);
    }
}
