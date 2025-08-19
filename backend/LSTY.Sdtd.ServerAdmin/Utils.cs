namespace LSTY.Sdtd.ServerAdmin
{
    public static class Utils
    {
        public static string FormatCommandArgs(string? args)
        {
            if (args == null || args.Length == 0)
            {
                return string.Empty;
            }

            if (args[0] == '\"' && args[^1] == '\"')
            {
                return args;
            }

            if (args.Contains('\"'))
            {
                throw new Exception("Parameters should not contain the character double quotes.");
            }

            if (args.Contains(' '))
            {
                return $"\"{args}\"";
            }

            return args;
        }
    }
}
