using System.Linq;
using Convert = System.Convert;

namespace GodotLib.Util;

public static class OsExtensions
{
    private static string[] args;

    extension(OS)
    {
        public static T GetCmdlineArg<T>(string argName, T defaultValue)
        {
            try
            {
                args ??= OS.GetCmdlineArgs();
            
                foreach (var arg in args)
                {
                    if (arg.StartsWith(argName))
                    {
                        var value = arg.Substring(argName.Length + 1);
                        return (T)Convert.ChangeType(value, typeof(T));
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

        public static bool HasCmdlineArg(string arg)
        {
            args ??= OS.GetCmdlineArgs();
            return args.Contains(arg);
        }
    }
}