//using Sdcb.SimdPaddleOCR;
//using Sdcb.SimdPaddleOCR.Models.ChineseV6Tiny;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Formats;
//using SixLabors.ImageSharp.PixelFormats;
//using System.Runtime.InteropServices;
//using Xunit;

//namespace ExampleApeart.Test
//{
//    public class ScanImageTest
//    {

//        private readonly ITestOutputHelper _output;

//        public ScanImageTest(ITestOutputHelper output)
//        {
//            _output = output;
//        }

//        [Fact]
//        public async Task Scan()
//        {
//            if (System.IO.File.Exists("sample.jpg"))
//            {
//                _output.WriteLine("文件存在");
//            }
//            using PaddleOcrAll ocr = await PaddleOcrAll.LoadAsync(ChineseV6TinyModels.Default);
//            using Image<Rgba32> image = await Image.LoadAsync<Rgba32>(CreateDecoderOptions(), "sample.jpg");
//            Rgba32[]? packedCopy = null;
//            if (!image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory))
//            {
//                packedCopy = new Rgba32[checked(image.Width * image.Height)];
//                image.CopyPixelDataTo(packedCopy);
//                memory = packedCopy;
//            }

//            PaddleOcrResult result = ocr.Run(MemoryMarshal.AsBytes(memory.Span), image.Width, image.Height,
//                format: ImagePixelFormat.Rgba32);
//            _output.WriteLine(result.Text);
//            Assert.Equal(1, 1);
//        }



//        // 必须 Clone Default：直接改 Configuration.Default 会丢掉 PNG/JPEG 解码器。
//        // 默认分配器按 4MB 分块，大图会让 DangerousTryGetSinglePixelMemory 失败。
//        static DecoderOptions CreateDecoderOptions()
//        {
//            Configuration configuration = Configuration.Default.Clone();
//            configuration.PreferContiguousImageBuffers = true;
//            return new DecoderOptions { Configuration = configuration };
//        }

//    }
//}
