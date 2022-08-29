namespace AlisiaDragoon
{
    using System;
    using System.Collections.Generic;
    using NLog;

    internal class Compresser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static void ControlParameters(ICompressArguments parameters)
        {
            if (!File.Exists(parameters.InputFile))
            {
                throw new ArgumentException("'input' parameter not found!");
            }
        }

        internal void Compress(ICompressArguments parameters)
        {
            ControlParameters(parameters);

            string? outputPath = Path.GetDirectoryName(parameters.OutputFile);

            _ = Directory.CreateDirectory(outputPath ?? string.Empty);

            using (var reader = new BinaryReader(new FileStream(parameters.InputFile, FileMode.Open)))
            using (var stream = new MemoryStream())
            {
                ushort nbLoop = 0x8000;
                stream.WriteBytes(0);

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    ushort[] header = new ushort[2];
                    var bytesList = new List<byte>();

                    for (int i = 0; i < 2; i++)
                    {
                        uint[] valuesRead = new uint[4];

                        for (int j = 0; j < 4; j++)
                        {
                            valuesRead[j] = reader.ReadUInt32BE();
                            logger.Debug($"Read {j + 1} 0x{reader.BaseStream.Position - 4:X8} : {valuesRead[j]:X8}");
                        }

                        (header[i], var bytes) = this.GetBytes(valuesRead);
                        bytesList.AddRange(bytes);
                        logger.Debug(string.Empty);
                    }

                    stream.WriteBytes(header[0]);
                    stream.WriteBytes(header[1]);
                    bytesList.ForEach(x => stream.WriteByte(x));

                    nbLoop++;
                }

                stream.Position = 0;
                stream.WriteBytes(nbLoop);

                if (!parameters.Size.HasValue || (stream.Length <= parameters.Size))
                {
                    using (var writer = new BinaryWriter(new FileStream(parameters.OutputFile, FileMode.OpenOrCreate)))
                    {
                        writer.BaseStream.Position = parameters.Adress;

                        stream.Position = 0;
                        writer.Write(stream.ToArray());
                    }
                }
                else
                {
                    throw new ArgumentException("Not enough space");
                }
            }
        }

        private (ushort, List<byte>) GetBytes(uint[] values)
        {
            uint[] datas = new uint[4];
            ushort header = 0;
            var bytes = new List<byte>();

            uint bitSelect1 = 0x11111111;
            uint bitSelect2 = 0x22222222;
            uint bitSelect4 = 0x44444444;
            uint bitSelect8 = 0x88888888;

            datas[0] = (values[0] & bitSelect8) >> 3;
            datas[1] = (values[0] & bitSelect4) >> 2;
            datas[2] = (values[0] & bitSelect2) >> 1;
            datas[3] = values[0] & bitSelect1;

            datas[0] |= (values[1] & bitSelect8) >> 2;
            datas[1] |= (values[1] & bitSelect4) >> 1;
            datas[2] |= values[1] & bitSelect2;
            datas[3] |= (values[1] & bitSelect1) << 1;

            datas[0] |= (values[2] & bitSelect8) >> 1;
            datas[1] |= values[2] & bitSelect4;
            datas[2] |= (values[2] & bitSelect2) << 1;
            datas[3] |= (values[2] & bitSelect1) << 2;

            datas[0] |= values[3] & bitSelect8;
            datas[1] |= (values[3] & bitSelect4) << 1;
            datas[2] |= (values[3] & bitSelect2) << 2;
            datas[3] |= (values[3] & bitSelect1) << 3;

            logger.Debug($"Value 1 : {datas[0]:X8}");
            logger.Debug($"Value 2 : {datas[1]:X8}");
            logger.Debug($"Value 3 : {datas[2]:X8}");
            logger.Debug($"Value 4 : {datas[3]:X8}");

            for (int i = 0; i < 4; i++)
            {
                for (int j = 3; j >= 0; j--)
                {
                    byte b = (byte)((datas[i] >> (j * 8)) & 0xFF);

                    if (b != 0)
                    {
                        bytes.Add(b);
                        header |= (ushort)(1 << (16 - (4 - j + (4 * i))));
                    }
                }
            }

            logger.Debug($"Header : {header:X4}");
            logger.Debug($"Bytes  : {string.Join(", ", bytes.Select(x => $"0x{x:X2}"))}");

            return (header, bytes);
        }
    }
}
