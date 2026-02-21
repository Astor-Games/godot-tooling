using Convert = System.Convert;

namespace GodotLib.Util;

public static class OsExtensions
{
    private static string[] args;

    extension(OS)
    {
        public static T GetCmdlineArg<T>(string argName, T defaultValue = default)
        {
            try
            {
                argName = argName.StartsWith("--") ? argName : $"--{argName}";
                args ??= OS.GetCmdlineArgs();

                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];

                    // Check for --arg=value format
                    if (arg.StartsWith(argName + "=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var value = arg.Substring(argName.Length + 1);
                        return (T)Convert.ChangeType(value, typeof(T));
                    }

                    // Check for --arg value format
                    if (arg.Equals(argName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (i == args.Length - 1 || args[i+1].StartsWith("--"))
                        {
                            PrintErr($"Argument {argName} has no value");
                            return defaultValue;
                        }

                        return (T)Convert.ChangeType(args[i+1], typeof(T));
                    }
                }

                return defaultValue;
            }
            catch (Exception e)
            {
                PushError(e);
                return defaultValue;
            }
        }

        public static bool HasCmdlineArg(string argName)
        {
            argName = argName.StartsWith("--") ? argName : $"--{argName}";
            args ??= OS.GetCmdlineArgs();
            return args.Contains(argName);
        }
    }
}