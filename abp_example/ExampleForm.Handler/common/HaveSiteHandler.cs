//using StackExchange.Redis;
//using Volo.Abp.DependencyInjection;

//namespace ExampleForm.Handler
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class HaveSiteHandler : ITransientDependency
//    {
//        private RedisClient _appCache;

//        /// <summary>
//        /// 班次 添加了日期的，精确命中
//        /// </summary>
//        private string shiftNo { get; set; } = "251011:K12:02";

//        /// <summary>
//        /// 座位数
//        /// </summary>
//        private int seatCount { get; set; } = 1600;

//        /// <summary>
//        /// 站点数
//        /// </summary>
//        private int stationCount { get; set; } = 60;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="appCache"></param>
//        public HaveSiteHandler(RedisClient appCache)
//        {
//            _appCache = appCache;
//        }

//        /// <summary>
//        /// 初始化
//        /// </summary>
//        /// <param name="_seatCount">座位总数</param>
//        /// <param name="_stationCount">站点总数</param>
//        public async Task Start(string _shiftNo, int _seatCount = 1600, int _stationCount = 60)
//        {
//            shiftNo = _shiftNo;
//            seatCount = _seatCount;
//            stationCount = _stationCount;

//            var sites = new List<SortedSetEntry>();// [] {  };
//            for (var k = 0; k < seatCount; k++)
//            {
//                sites.Add(new SortedSetEntry(k, stationCount));
//            }
//            await _appCache.SortedSetAddAsync($"{shiftNo}:sorted:{stationCount}", sites.ToArray());
//        }

//        /// <summary>
//        /// 添加节点
//        /// </summary>
//        /// <param name="shiftNo"></param>
//        /// <param name="seatNo">座位</param>
//        /// <param name="startStation">开始站点</param>
//        /// <param name="stationTotal">站点总数，比如从A到B</param>
//        /// <returns></returns>
//        private async Task AddSection(int seatNo, int startStation, int stationTotal)
//        {
//            //一下后续可以考虑合并提交，原子操作
//            await SectionHashIncrementAsync(stationTotal);
//            //记录 X个站点的 哪个座位 从哪个站点开始
//            await _appCache.SortedSetAddAsync($"{shiftNo}:sorted:{stationTotal}", seatNo, startStation);
//        }

//        /// <summary>
//        /// 标记多少站点，有多少座位空余
//        /// </summary>
//        /// <param name="stationTotal"></param>
//        /// <returns></returns>
//        private async Task SectionHashIncrementAsync(int stationTotal)
//        {
//            var key = $"{shiftNo}:count";
//            //记录 段数有的个数 比如3站长度的 有多少座位 5站长度的有多少座位 如果没有，则从更长的地方截断
//            await _appCache.HashIncrementAsync(key, stationTotal);
//        }

//        /// <summary>
//        /// 标记多少站点，有多少座位空余
//        /// </summary>
//        /// <param name="stationTotal"></param>
//        /// <returns></returns>
//        private async Task SectionHashDecrementAsync(int stationTotal)
//        {
//            var key = $"{shiftNo}:count";
//            //记录 段数有的个数 比如3站长度的 有多少座位 5站长度的有多少座位 如果没有，则从更长的地方截断
//            await _appCache.HashDecrementAsync(key, stationTotal);
//        }

//        /// <summary>
//        /// 获取一个座位
//        /// </summary>
//        /// <param name="startStation">从哪个站点开始</param>
//        /// <param name="stationTotal">要做几个站</param>
//        /// <returns></returns>
//        public async Task<int> GetSeat(int startStation, int stationTotal)
//        {
//            var key1 = $"{shiftNo}:sorted:{stationTotal}";
//            var seatNo = await ReadSeatCurrent(stationTotal, startStation, stationTotal);
//            if (seatNo > 0)
//            {
//                return seatNo;
//            }
//            //第一个没有命中，就要找下一个的了，站点大于当前的
//            if (stationTotal < stationCount)
//            {
//                //站点合计
//                var stas = new List<int>();
//                //HashEntry[]
//                var fields = await _appCache.HashReadAllAsync($"{shiftNo}:count");
//                if (fields != null)
//                {
//                    if (fields?.Any() == true)
//                    {
//                        foreach (var item in fields)
//                        {
//                            if (int.TryParse(item.Value, out var num))
//                            {
//                                if (num > 0)
//                                {
//                                    //item.Name;
//                                    if (int.TryParse(item.Name, out var start))
//                                    {
//                                        if (start > stationTotal)
//                                        {
//                                            stas.Add(start);
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }

//                if (stas.Any() == true)
//                {
//                    //遍历获取... .. .
//                    foreach (var item in stas)
//                    {
//                        var seat = await ReadSeatCurrent(item, startStation, stationTotal);
//                        if (seat > 0)
//                        {
//                            return seat;
//                        }
//                    }
//                }
//            }

//            //从全站获取 下面代码 未完待续
//            seatNo = await ReadSeatCurrent(stationCount, startStation, stationTotal);
//            if (seatNo > 0)
//            {
//                //这里有问题，开头，结尾都减去了...
//                return seatNo;
//            }
//            return 0;
//        }

//        /// <summary>
//        /// 从某一个站点段中获取座位
//        /// 下面代码有问题，比如从8个站点的数据中，我从2站点做到4站点，则要分2段另存
//        /// </summary>
//        /// <param name="sectionLong">从几段站点库获取,示例:8,表示里面的都是8个连续站点的数据，从哪个站点开始则要看score,filed是座位号</param>
//        /// <param name="startStation">从哪个站开始(用户要从哪个站开始乘坐)</param>
//        /// <param name="stationTotal">要做几个站(用户本次旅行要做的总站数)</param>
//        /// <returns>返回座位号</returns>
//        private async Task<int> ReadSeatCurrent(int sectionLong, int startStation, int stationTotal)
//        {
//            var key1 = $"{shiftNo}:sorted:{sectionLong}";

//            // 重新计算开始站点，比如当前是8站点段位 你需要第13站点-第15站点，则至少是要多少站点开头13-(8-2)的
//            // 修正计算逻辑：需要找到起始站点 <= startStation 且能够覆盖所需行程的座位段
//            var currentStart = startStation - (sectionLong - stationTotal);
//            if (currentStart < 1) currentStart = 1;

//            // 从有序排序中，获取站点数大于等于currentStart的座位，比如上面段数是8，这里currentStart为5表示返回第一个，开始站点是5的座位，5+8，表示这个座位空闲的站点是5-13
//            var read = await _appCache.SortedSetStartPopScoreAsync(key1, currentStart);

//            if (read != null && read.HasValue)
//            {
//                // 座位
//                int.TryParse(read.Value.Element, out var seatNo);
//                int.TryParse(read.Value.Score.ToString(), out var readStart);

//                // 获取到了座位
//                await SectionHashDecrementAsync(stationTotal);

//                var readEnd = readStart + sectionLong; // 这一段的末站
//                var userEnd = startStation + stationTotal; // 用户行程的末站

//                // 处理前面的多余段位
//                if (readStart < startStation)
//                {
//                    // 前面有一段需要回收
//                    var frontSegmentLength = startStation - readStart;
//                    await AddSection(seatNo, readStart, frontSegmentLength);
//                }

//                // 处理后面的多余段位
//                if (readEnd > userEnd)
//                {
//                    // 尾巴还有一段需要回收
//                    var tailSegmentStart = userEnd;
//                    var tailSegmentLength = readEnd - userEnd;
//                    await AddSection(seatNo, tailSegmentStart, tailSegmentLength);
//                }

//                if (seatNo > 0)
//                {
//                    await SectionHashDecrementAsync(stationTotal);
//                }
//                return seatNo;
//            }
//            return 0;
//        }


//        ///// <summary>
//        ///// 从某一个站点段中获取座位 未完待续
//        ///// 下面代码有问题，比如从8个站点的数据中，我从2站点做到4站点，则要分2段另存
//        ///// </summary>
//        ///// <param name="sectionLong">从几段站点库获取,示例:8,表示里面的都是8个连续站点的数据，从哪个站点开始则要看score,filed是座位号</param>
//        ///// <param name="startStation">从哪个站开始(用户要从哪个站开始乘坐)</param>
//        ///// <param name="stationTotal">要做几个站(用户本次旅行要做的总站数)</param>
//        ///// <returns>返回座位号</returns>
//        //private async Task<int> ReadSeatCurrent(int sectionLong, int startStation, int stationTotal)
//        //{
//        //    var key1 = $"{shiftNo}:sorted:{sectionLong}";
//        //    //重新计算开始站点，比如当前是8站点段位 你需要第13站点-第15站点，则至少是要多少站点开头13-(8-2)的
//        //    var currentStart = startStation - sectionLong - stationTotal;
//        //    //从有序排序中，获取站点数大于等于currentStart的座位，比如上面段数是8，这里currentStart为5表示返回第一个，开始站点是5的座位，5+8，表示这个座位空闲的站点是5-13
//        //    var read = await _appCache.SortedSetStartPopScoreAsync(key1, currentStart);
//        //    if (read != null && read.HasValue)
//        //    {
//        //        //座位
//        //        int.TryParse(read.Value.Element, out var seatNo);

//        //        int.TryParse(read.Value.Score.ToString(), out var readStart);

//        //        //获取到了座位
//        //        await SectionHashDecrementAsync(stationTotal);

//        //        //把前后多余的段位放回座位池子
//        //        if (readStart < startStation)
//        //        {
//        //            //前面有一段
//        //            //待完善
//        //        }

//        //        var readEnd = readStart + sectionLong;//这一段的末站

//        //        if (readEnd > (startStation + stationTotal))
//        //        {
//        //            //尾巴还有一段
//        //            //待完善
//        //            // await AddSection(seatNo, lessStart, lessTotal);//表示把剩余座位加回队列 第一个参数座位号 第二个参数从哪个站开始 第三个参数，这一段的长度
//        //        }

//        //        //if (sectionLong > stationTotal)
//        //        //{
//        //        //    //1-2-3-4-5-6-7
//        //        //    //1-2-3
//        //        //    //6  1  2
//        //        //    var lessTotal = sectionLong - stationTotal;//剩余段
//        //        //    var lessStart = sectionLong - stationTotal - 1;//开始几个段
//        //        //    await AddSection(seatNo, lessStart, lessTotal);
//        //        //}
//        //        if (seatNo > 0)
//        //        {
//        //            await SectionHashDecrementAsync(stationTotal);
//        //        }
//        //        return seatNo;
//        //    }
//        //    return 0;
//        //}


//    }
//}
