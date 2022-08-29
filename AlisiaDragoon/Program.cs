namespace AlisiaDragoon
{
    using CommandLine;
    using CommandLine.Text;
    using NLog;

    internal class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static int DecompressAction(DecompressArguments parameters)
        {
            try
            {
                var action = new Decompresser();

                action.Decompress(parameters);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, ex.Message);

                return Constants.RETURN_CODE_ERRORS;
            }

            return Constants.RETURN_CODE_OK;
        }
        private static int CompressAction(CompressArguments parameters)
        {
            try
            {
                var action = new Compresser();

                action.Compress(parameters);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, ex.Message);

                return Constants.RETURN_CODE_ERRORS;
            }

            return Constants.RETURN_CODE_OK;
        }

        private static int DisplayHelp<T>(ParserResult<T> parserResult, IEnumerable<Error> errors)
        {
            var helpText = HelpText.AutoBuild(
                parserResult,
                h =>
                {
                    h.AddDashesToOption = true;
                    h.AutoHelp = false;     // hides --help
                    h.AdditionalNewLineAfterOption = false;
                    h.AutoVersion = false;  // hides --version
                    h.MaximumDisplayWidth = 200;

                    return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                },
                y => y);

            logger.Info(helpText);

            //helpText
            //    .ToString()
            //    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
            //    .ToList()
            //    .ForEach(x => logger.Info(x));

            return Constants.RETURN_CODE_ARGUMENTS_ERRORS;
        }

        private static int Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<DecompressArguments, CompressArguments>(args);

            return parserResult.MapResult(
                (DecompressArguments decompressArguments) => DecompressAction(decompressArguments),
                (CompressArguments compressArguments) => CompressAction(compressArguments),
                errors => DisplayHelp(parserResult, errors));
        }
    }
}