namespace AlisiaDragoon
{
    using CommandLine;

    [Verb("compress", false, new string[] { "c" }, HelpText = "Compress data")]
    public class CompressArguments : ICompressArguments
    {
        public uint Adress => this.AdressString.StartsWith("0x") ? Convert.ToUInt32(this.AdressString, 16) : Convert.ToUInt32(this.AdressString);

        [Option('a', "address", Default = "0", HelpText = "Adress in output file where insert compressed data")]
        public string AdressString { get; set; } = string.Empty;

        [Option('i', "input", Required = true, HelpText = "Path of the file containing uncompressed data")]
        public string InputFile { get; set; } = string.Empty;

        [Option('o', "output", Required = true, HelpText = "Path of file containing compressed data")]
        public string OutputFile { get; set; } = string.Empty;

        public int? Size => !string.IsNullOrWhiteSpace(this.SizeString) ? (this.SizeString.StartsWith("0x") ? Convert.ToInt32(this.SizeString, 16) : Convert.ToInt32(this.SizeString)) : null;

        [Option('s', "size", Required = false, HelpText = "Size of space in output to insert compressed data")]
        public string SizeString { get; set; } = string.Empty;
    }
}