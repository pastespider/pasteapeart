//using System.Threading.Channels;
//using Volo.Abp.DependencyInjection;

//namespace ExampleForm
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class ChannelHelper : ISingletonDependency
//    {

//        private Channel<ChannelNoticeModel> _channelnotice;

//        /// <summary>
//        /// 
//        /// </summary>
//        public ChannelHelper()
//        {
//            _channelnotice = Channel.CreateUnbounded<ChannelNoticeModel>();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="input"></param>
//        public async void Write(ChannelNoticeModel input)
//        {
//            await _channelnotice.Writer.WriteAsync(input);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public Channel<ChannelNoticeModel> Notice { get { return _channelnotice; } }

//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class ChannelNoticeModel
//    {
//        /// <summary>
//        /// 内容
//        /// </summary>
//        public string body { get; set; }

//        /// <summary>
//        /// 动作名称 waitrevice:任务待领取 finish:任务完成 cannel:任务取消 waithand:待处理 waitreply:待反馈
//        /// </summary>
//        public string action { get; set; }

//        /// <summary>
//        /// 用户id
//        /// </summary>
//        public int userid { get; set; }

//        /// <summary>
//        /// 部门ID
//        /// </summary>
//        public int partid { get; set; }

//        /// <summary>
//        /// 整个项目组
//        /// </summary>
//        public int projectid { get; set; }

//        /// <summary>
//        /// 排除谁
//        /// </summary>
//        public int exceptuid { get; set; }
//    }
//}
