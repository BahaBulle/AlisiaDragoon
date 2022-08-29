namespace AlisiaDragoon
{
    public interface IDecompressArguments
    {
        uint Adress { get; }
        string OutputFile { get; set; }
        string InputFile { get; set; }
    }
}