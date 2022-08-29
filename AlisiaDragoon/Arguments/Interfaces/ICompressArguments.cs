namespace AlisiaDragoon
{
    public interface ICompressArguments
    {
        uint Adress { get; }

        string InputFile { get; set; }

        string OutputFile { get; set; }

        int? Size { get; }
    }
}