using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PasteApeart
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageHelper
    {

        #region 生产验证码
        /// <summary>
        /// 
        /// </summary>
        public enum VerifyCodeType
        {
            /// <summary>
            /// 
            /// </summary>
            NumberVerifyCode,
            /// <summary>
            /// 
            /// </summary>
            AbcVerifyCode,
            /// <summary>
            /// 
            /// </summary>
            MixVerifyCode
        };

        /// <summary>
        /// 1.数字验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string CreateNumberVerifyCode(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值  
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字  
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字  
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码  
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        /// <summary>
        /// 2.字母验证码
        /// </summary>
        /// <param name="length">字符长度</param>
        /// <returns>验证码字符</returns>
        private static string CreateAbcVerifyCode(int length)
        {
            char[] verification = new char[length];
            char[] dictionary = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                verification[i] = dictionary[random.Next(dictionary.Length - 1)];
            }
            return new string(verification);
        }

        /// <summary>
        /// 3.混合验证码
        /// </summary>
        /// <param name="length">字符长度</param>
        /// <returns>验证码字符</returns>
        private static string CreateMixVerifyCode(int length)
        {
            char[] verification = new char[length];
            char[] dictionary = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                verification[i] = dictionary[random.Next(dictionary.Length - 1)];
            }
            return new string(verification);
        }

        /// <summary>
        /// 产生验证码（随机产生4-6位）
        /// </summary>
        /// <param name="type">验证码类型：数字，字符，符合</param>
        /// <returns></returns>
        public static string CreateVerifyCode(VerifyCodeType type)
        {
            string verifyCode = string.Empty;
            Random random = new Random();
            int length = random.Next(4, 6);
            switch (type)
            {
                case VerifyCodeType.NumberVerifyCode:
                    verifyCode = CreateNumberVerifyCode(length);
                    break;
                case VerifyCodeType.AbcVerifyCode:
                    verifyCode = CreateAbcVerifyCode(length);
                    break;
                case VerifyCodeType.MixVerifyCode:
                    verifyCode = CreateMixVerifyCode(length);
                    break;
            }
            return verifyCode;
        }
        #endregion


        /// <summary>
        /// 验证码图片 => byte[]
        /// </summary>
        /// <param name="verifyCode">验证码</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns>byte[]</returns>
        public static byte[] CreateByteByImgVerifyCode(string verifyCode, int width, int height)
        {

            using Image image = new Image<Rgba32>(width, height);
            //漆底色白色
            image.Mutate(x => x.DrawLine(Pens.DashDot(Color.White, width), new PointF[] { new PointF() { X = 0, Y = 0 }, new PointF() { X = width, Y = height } }));

            FontCollection collection = new FontCollection();
            FontFamily family = collection.Add("font/font.ttf");
            Font font = family.CreateFont(20, FontStyle.Bold);

            PointF startPointF = new PointF(5, 5);
            Random random = new Random(); //随机数产生器

            Color[] colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Purple, Color.Peru, Color.LightSeaGreen, Color.Lime, Color.Magenta, Color.Maroon, Color.MediumBlue, Color.MidnightBlue, Color.Navy };
            //绘制大小
            for (int i = 0; i < verifyCode.Length; i++)
            {
                image.Mutate(x => x.DrawText(verifyCode[i].ToString(), font, colors[random.Next(colors.Length)], startPointF));
                //Console.WriteLine($"draw code:{verifyCode[i]} point:{startPointF.X}-{startPointF.Y}");
                startPointF.X += (int)(width - 10) / verifyCode.Length;
                startPointF.Y = random.Next(5, 10);
            }

            Pen pen = Pens.DashDot(Color.Silver, 1);

            //绘制干扰线
            for (var k = 0; k < 40; k++)
            {
                PointF[] points = new PointF[2];
                points[0] = new PointF(random.Next(width), random.Next(height));
                points[1] = new PointF(random.Next(width), random.Next(height));
                image.Mutate(x => x.DrawLine(pen, points));
            }


            using MemoryStream stream = new MemoryStream();
            image.Save(stream, JpegFormat.Instance);
            //输出图片流  
            return stream.ToArray();

        }
    }

}
