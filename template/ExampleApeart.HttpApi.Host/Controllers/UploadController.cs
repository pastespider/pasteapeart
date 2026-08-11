using Microsoft.AspNetCore.Mvc;
using $ext_safeprojectname$.Controllers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;

namespace $ext_safeprojectname$.HttpApi.Host.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("/api/app/[controller]/[action]")]
    public class UploadController : IBaseController
    {
        /// <summary>
        /// 富文本上传图片 控件
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="objid"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        public WangEditorUpload UploadImage(int typeid = 0, int objid = 0)
        {
            var one = new WangEditorUpload();
            one.errno = 1;
            if (base.Request.Form != null)
            {
                if (base.Request.Form.Files != null)
                {
                    if (base.Request.Form.Files.Count > 0)
                    {
                        one.errno = 0;
                        one.data = new List<WangEditorUploadItem>();
                        for (var k = 0; k < base.Request.Form.Files.Count; k++)
                        {
                            var finput = base.Request.Form.Files[k];
                            //这里还要进行文件后缀的过滤
                            var item = new WangEditorUploadItem();
                            var extension = System.IO.Path.GetExtension(finput.FileName);
                            var filename = $"{finput.FileName}{k}{DateTimeOffset.Now.ToUnixTimeMilliseconds()}".ToMd5Lower();
                            var savepath = $"upload/{typeid}/{objid}/{filename}.{extension}";
                            var path = System.IO.Path.GetDirectoryName(savepath);
                            if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
                            item.alt = finput.FileName;
                            item.href = $"/{savepath}";
                            item.url = $"/{savepath}";
                            using var image = Image.Load<Rgba32>(finput.OpenReadStream());
                            image.Mutate(x => x.Resize(0, 180));
                            image.Save(savepath);
                            item.url = $"/{savepath}";
                            finput.OpenReadStream().Dispose();
                            item.size = finput.Length;
                            one.data.Add(item);
                        }
                    }
                }
            }
            return one;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        public async Task<WangEditorUpload> UpImage(string type = "head", string size = "")
        {
            var one = new WangEditorUpload();
            one.errno = 1;
            var _month = DateTime.Now.ToString("yyyyMM");
            if (base.Request.Form != null)
            {
                if (base.Request.Form.Files != null)
                {
                    if (base.Request.Form.Files.Count > 0)
                    {
                        one.errno = 0;
                        one.data = new List<WangEditorUploadItem>();
                        for (var k = 0; k < base.Request.Form.Files.Count; k++)
                        {
                            var finput = base.Request.Form.Files[k];

                            var _file_md5 = CalculateMd5(finput);//获取文件码

                            //var file = await _dbContext.UploadImageInfo.Where(x => x.Md5 == _file_md5).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                            //if (file != null && file != default)
                            //{
                            //    one.data.Add(new WangEditorUploadItem { href = file.Url, url = file.Url, size = file.Size });
                            //    continue;
                            //}

                            //这里还要进行文件后缀的过滤
                            var item = new WangEditorUploadItem();
                            var extension = System.IO.Path.GetExtension(finput.FileName);
                            if (String.IsNullOrEmpty(extension))
                            {
                                extension = ".jpg";
                            }
                            else
                            {
                                if (!extension.StartsWith("."))
                                {
                                    extension = "." + extension;
                                }
                            }
                            if (extension != ".gif")
                            {
                                extension = ".webp";
                            }
                            var filename = $"{finput.FileName}{k}{DateTimeOffset.Now.ToUnixTimeMilliseconds()}".ToMd5Lower();
                            var _char = Path.DirectorySeparatorChar.ToString();
                            var savepath = $"upload/{type}/{_month}/{filename}{extension}";

                            var path = System.IO.Path.GetDirectoryName(savepath);
                            if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
                            if (System.IO.File.Exists(savepath)) { System.IO.File.Delete(savepath); }

                            item.alt = finput.FileName;
                            item.href = $"/{savepath}";
                            item.url = $"/{savepath}";
                            if (IsImageExtension(finput.FileName))
                            {
                                var _width = 0;
                                var _height = 0;
                                if (!String.IsNullOrEmpty(size))
                                {
                                    var _sizes = size.Split('*');
                                    int.TryParse(_sizes[0], out _width);
                                    int.TryParse(_sizes[1], out _height);
                                }
                                using var image = Image.Load<Rgba32>(finput.OpenReadStream());
                                if (image.Width > _width && _width != 0)
                                {
                                    image.Mutate(x => x.Resize(_width, _height));
                                }
                                else
                                {
                                    if (image.Height > _height && _height != 0)
                                    {
                                        image.Mutate(x => x.Resize(_width, _height));
                                    }
                                }
                                if (extension == ".webp")
                                {
                                    image.Save(savepath, new WebpEncoder());
                                }
                                else
                                {
                                    image.Save(savepath);
                                }
                                item.url = $"/{savepath}";
                                finput.OpenReadStream().Dispose();
                            }
                            else
                            {
                                using (FileStream fs = new FileStream(savepath, FileMode.Create))
                                {
                                    finput.CopyTo(fs);
                                    fs.Flush();
                                }
                            }
                            item.size = finput.Length;
                            one.data.Add(item);

                            //var entity = new UploadImageInfo
                            //{
                            //    Md5 = _file_md5,
                            //    Size = (int)item.size,
                            //    Path = savepath,
                            //    Url = item.url,
                            //    //Width = item.width,
                            //    //Height = item.height,
                            //    Dir = type,
                            //    Extension = extension,
                            //    //UserId = _user_id
                            //};
                            //_dbContext.Add(entity);
                            //await _dbContext.SaveChangesAsync();

                        }
                    }
                }
            }
            return await Task.FromResult(one);
        }


        /// <summary>
        /// 测试上传
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        public async Task<string> TestUpload(IFormFile input)
        {
            return await Task.FromResult($"大小:{input?.Length}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static string CalculateMd5(IFormFile file)
        {
            // 确保文件不为空
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }
            // 使用 MD5 创建哈希计算器
            using (var md5 = MD5.Create())
            {
                // 读取文件流
                using (var stream = file.OpenReadStream())
                {
                    // 计算哈希值
                    var hashBytes = md5.ComputeHash(stream);
                    // 将字节数组转换为十六进制字符串
                    var sb = new StringBuilder();
                    foreach (var b in hashBytes)
                    {
                        sb.Append(b.ToString("x2")); // "x2" 表示两位小写的十六进制
                    }
                    return sb.ToString();
                }
            }
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        public WangEditorUpload UpFile(string type = "file")
        {
            var one = new WangEditorUpload();
            one.errno = 1;
            if (base.Request.Form != null)
            {
                if (base.Request.Form.Files != null)
                {
                    if (base.Request.Form.Files.Count > 0)
                    {
                        one.errno = 0;
                        one.data = new List<WangEditorUploadItem>();
                        for (var k = 0; k < base.Request.Form.Files.Count; k++)
                        {
                            var finput = base.Request.Form.Files[k];
                            //这里还要进行文件后缀的过滤
                            var item = new WangEditorUploadItem();
                            var extension = System.IO.Path.GetExtension(finput.FileName);
                            var filename = $"{finput.FileName}{k}{DateTimeOffset.Now.ToUnixTimeMilliseconds()}".ToMd5Lower();
                            var savepath = $"upload/{type}/{filename}.{extension}";
                            var path = System.IO.Path.GetDirectoryName(savepath);
                            if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
                            item.alt = finput.FileName;
                            item.href = $"/{savepath}";
                            item.url = $"/{savepath}";
                            using var image = Image.Load<Rgba32>(finput.OpenReadStream());
                            image.Mutate(x => x.Resize(0, 180));
                            image.Save(savepath);
                            item.url = $"/{savepath}";
                            finput.OpenReadStream().Dispose();
                            item.size = finput.Length;
                            one.data.Add(item);
                        }
                    }
                }
            }
            return one;
        }


        ///// <summary>
        ///// 读取图片库 用于历史图片选择
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        //public async Task<string[]> Images([FromQuery] InputSearchBase input)
        //{
        //    var images = await _dbContext.UploadImageInfo.AsQueryable()
        //        .OrderByDescending(x => x.Id)
        //        .Page(input.page, input.size)
        //        .Select(x => x.Url)
        //        .ToArrayAsync();
        //    if (images?.Any() == false)
        //    {
        //        return null;
        //    }
        //    return images;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool IsImageExtension(string filename)
        {
            if (filename.ToLower().EndsWith(".jpeg"))
            {
                return true;
            }
            if (filename.ToLower().EndsWith(".jpg"))
            {
                return true;
            }
            if (filename.ToLower().EndsWith(".bmp"))
            {
                return true;
            }
            if (filename.ToLower().EndsWith(".gif"))
            {
                return true;
            }
            if (filename.ToLower().EndsWith(".webp"))
            {
                return true;
            }
            if (filename.ToLower().EndsWith(".png"))
            {
                return true;
            }

            return false;
        }


        ///// <summary>
        ///// 上传图片
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<string> UpWord(IFormFile file)
        //{

        //    return await ReadImageText(file);
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="file"></param>
        ///// <returns></returns>
        //private static async Task<byte[]> ConvertIFormFileToByteArrayAsync(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return Array.Empty<byte>();

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        await file.CopyToAsync(memoryStream);
        //        return memoryStream.ToArray();
        //    }
        //}

        ///// <summary>
        ///// 识别图片文字
        ///// </summary>
        ///// <param name="file"></param>
        ///// <returns></returns>
        //private static async Task<string> ReadImageText(IFormFile file)
        //{
        //    var sb = new StringBuilder();
        //    //var imagebyte = System.IO.File.ReadAllBytes(path);
        //    var imagebyte = await ConvertIFormFileToByteArrayAsync(file);
        //    sb.AppendLine("识别开始");
        //    using (var engine = new TesseractEngine(@"./tessdata", "eng+chi_sim", EngineMode.TesseractAndLstm))
        //    {
        //        using (var img = Pix.LoadFromMemory(imagebyte))
        //        {
        //            using (var page = engine.Process(img))
        //            {
        //                sb.AppendLine(page.GetText());
        //            }
        //        }
        //    }
        //    sb.AppendLine("识别结束");
        //    return sb.ToString();
        //}

        ///// <summary>
        ///// 图文识别
        ///// </summary>
        ///// <param name="path"></param>
        //private static void ReadImageText(string path)
        //{

        //    var imagebyte = System.IO.File.ReadAllBytes(path);
        //    Console.WriteLine("识别开始");
        //    using (var engine = new TesseractEngine(@"./tessdata", "eng+chi_sim", EngineMode.TesseractAndLstm))
        //    {
        //        using (var img = Pix.LoadFromMemory(imagebyte))
        //        {
        //            using (var page = engine.Process(img))
        //            {
        //                Console.WriteLine(page.GetText());
        //                //page.GetText();
        //            }
        //        }
        //    }
        //    Console.WriteLine("识别结束");
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    public class WangEditorUpload
    {

        /// <summary>
        /// 有几个错误
        /// </summary>
        public int errno { get; set; } = 0;
        /// <summary>
        /// 数据集
        /// </summary>
        public List<WangEditorUploadItem> data { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class WangEditorUploadItem
    {

        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string alt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string href { get; set; }

        /// <summary>
        /// 资源大小
        /// </summary>
        public long size { get; set; }
    }
}
