namespace AlisiaDragoon
{
    using CommandLine;

    [Verb("decompress", false, new string[] { "d" }, HelpText = "Decompress data")]
    public class DecompressArguments : IDecompressArguments
    {
        public uint Adress => this.AdressString.StartsWith("0x") ? Convert.ToUInt32(this.AdressString, 16) : Convert.ToUInt32(this.AdressString);

        [Option('a', "address", Required = true, HelpText = "Adress in output file of the data to decompress")]
        public string AdressString { get; set; } = string.Empty;

        [Option('i', "input", Required = true, HelpText = "Path of the file containing compressed data")]
        public string InputFile { get; set; } = string.Empty;

        [Option('o', "output", Required = true, HelpText = "Path of the file containing uncompressed data")]
        public string OutputFile { get; set; } = string.Empty;
    }
}