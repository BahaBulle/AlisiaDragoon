namespace AlisiaDragoon
{
    using System;
    using NLog;

    internal class Decompresser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static void ControlParameters(IDecompressArguments parameters)
        {
            if (!File.Exists(parameters.InputFile))
            {
                throw new ArgumentException("'rom' parameter not found!");
            }
        }

        internal void Decompress(IDecompressArguments parameters)
        {
            ControlParameters(parameters);

            string? outputPath = Path.GetDirectoryName(parameters.OutputFile);

            _ = Directory.CreateDirectory(outputPath ?? string.Empty);

            using (var reader = new BinaryReader(new FileStream(parameters.InputFile, FileMode.Open)))
            using (var writer = new BinaryWriter(new FileStream(parameters.OutputFile, FileMode.Create)))
            {
                reader.BaseStream.Position = parameters.Adress;

                ushort infos = reader.ReadUInt16BE();
                logger.Debug($"Infos 0x{reader.BaseStream.Position - 2:X8} : {infos:X4}");

                ushort nbLoop = (ushort)(infos & 0x3FFF);
                logger.Debug($"Loop number : {nbLoop:X4}");

                while (nbLoop > 0)
                {
                    ushort[] header = new ushort[2];
                    header[0] = reader.ReadUInt16BE();
                    header[1] = reader.ReadUInt16BE();
                    nbLoop--;
                    logger.Debug($"Header 0x{reader.BaseStream.Position - 4:X8} : {header[0]:X4}{header[1]:X4}");

                    for (int i = 0; i < 2; i++)
                    {
                        uint[] arrayOfData = new uint[4];

                        int cpt = 0;
                        for (int j = 3; j >= 0; j--)
                        {
                            uint data = ((uint)header[i] >> (j * 4)) & 0xFFFF;

                            uint value = 0;
                            for (int k = 3; k >= 0; k--)
                            {
                                if (((data >> k) & 0x01) == 1)
                                {
                                    byte b = reader.ReadByte();
                                    value |= (uint)(b << (k * 8));
                                }
                            }

                            arrayOfData[cpt++] = value;
                            logger.Debug($"  Value {cpt} : {value:X8}");
                        }

                        uint bitSelect1 = 0x11111111;
                        uint bitSelect2 = 0x22222222;
                        uint bitSelect4 = 0x44444444;
                        uint bitSelect8 = 0x88888888;

                        uint valueToWrite = (arrayOfData[0] & bitSelect1) << 3;
                        valueToWrite |= (arrayOfData[1] & bitSelect1) << 2;
                        valueToWrite |= (arrayOfData[2] & bitSelect1) << 1;
                        valueToWrite |= arrayOfData[3] & bitSelect1;

                        logger.Debug($"  Write 0x{writer.BaseStream.Position:X8} : {valueToWrite:X8}");
                        writer.WriteUIntBE(valueToWrite);

                        valueToWrite = (arrayOfData[0] & bitSelect2) << 2;
                        valueToWrite |= (arrayOfData[1] & bitSelect2) << 1;
                        valueToWrite |= arrayOfData[2] & bitSelect2;
                        valueToWrite |= (arrayOfData[3] & bitSelect2) >> 1;

                        logger.Debug($"  Write 0x{writer.BaseStream.Position:X8} : {valueToWrite:X8}");
                        writer.WriteUIntBE(valueToWrite);

                        valueToWrite = (arrayOfData[0] & bitSelect4) << 1;
                        valueToWrite |= arrayOfData[1] & bitSelect4;
                        valueToWrite |= (arrayOfData[2] & bitSelect4) >> 1;
                        valueToWrite |= (arrayOfData[3] & bitSelect4) >> 2;

                        logger.Debug($"  Write 0x{writer.BaseStream.Position:X8} : {valueToWrite:X8}");
                        writer.WriteUIntBE(valueToWrite);

                        valueToWrite = arrayOfData[0] & bitSelect8;
                        valueToWrite |= (arrayOfData[1] & bitSelect8) >> 1;
                        valueToWrite |= (arrayOfData[2] & bitSelect8) >> 2;
                        valueToWrite |= (arrayOfData[3] & bitSelect8) >> 3;

                        logger.Debug($"  Write 0x{writer.BaseStream.Position:X8} : {valueToWrite:X8}");
                        writer.WriteUIntBE(valueToWrite);
                    }
                }
            }
        }
    }
}
