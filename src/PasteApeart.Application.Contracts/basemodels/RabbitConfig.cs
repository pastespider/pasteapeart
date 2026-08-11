using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteApeart
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitConfig
    {
        /// <summary>
        /// 地址
        /// </summary>
        [Description("地址")]
        public string HostName { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        [Description("端口")]
        public int Port { get; set; }

        /// <summary>
        /// 作用域
        /// </summary>
        [Description("作用域")]
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// 账号
        /// </summary>
        [Description("账号")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Description("密码")]
        public string Password { get; set; }

        ///// <summary>
        ///// 交换机名称
        ///// </summary>
        //public string ExchangeName { get; set; }

        ///// <summary>
        ///// 交换机类型 direct直连模式 topic统配模式
        ///// </summary>
        //public string ExchangeType { get; set; }

        /// <summary>
        /// true:Durable(持久化) false:Transient(瞬时模式)
        /// </summary>
        [Description("true:Durable(持久化) false:Transient(瞬时模式)")]
        public bool Durable { get; set; } = true;

        /// <summary>
        /// 超时队列的交换机模型
        /// direct
        /// </summary>
        [Description("超时队列的交换机模型 direct")]
        public string ExpireExchangeType { get; set; } = "direct";

        ///// <summary>
        ///// 直连模式的交换类型 topic
        ///// </summary>
        //public string DirectExchangeType { get; set; } = "topic";

        /// <summary>
        /// channel.expire 延迟队列发送的路由key
        /// *:匹配一个单词
        /// #:匹配多个单词，单词之间用.隔开
        /// 匹配模式的时候exchangetype需要为topic
        /// </summary>
        [Description("分配规则 msg.* msg.#  *:匹配一个单词 #:匹配多个单词，单词之间用.隔开 匹配模式的时候exchangetype需要为topic")]
        public string ExpireRouteKey { get; set; } = "channel.expire";

        ///// <summary>
        ///// 直连模式的分配规则 channel.*
        ///// </summary>
        //public string DirectRouteKey { get; set; } = "channel.*";

        /// <summary>
        /// 超时队列
        /// </summary>
        [Description("超时队列")]
        public List<RabbitExcepireItem> ExpireTimeList { get; set; }

        /// <summary>
        /// 超时队列的exchange 过期后写入到待读取队列
        /// talk_expire_exchange 写入到过期后的队列
        /// </summary>
        [Description("超时队列的exchange 过期后写入到待读取队列 talk_expire_exchange 写入到过期后的队列")]
        public string ExpiredExchange { get; set; } = "talk_expire_exchange";

        /// <summary>
        /// 超时队列的最终队列 这个队列的是过期后的数据
        /// talk_expired_queue
        /// </summary>
        [Description("超时队列的最终队列 这个队列的是过期后的数据 talk_expired_queue")]
        public string ExpiredWatchQueue { get; set; } = "talk_expired_queue";

        /// <summary>
        /// 客户端名称
        /// </summary>
        [Description("客户端名称")]
        public string ClientName { get; set; }

        /// <summary>
        /// 配送模式 1不持续模式 2持续模式
        /// </summary>
        [Description("配送模式 1不持续模式 2持续模式")]
        public int DeliveryMode { get; set; } = 2;

        /// <summary>
        /// 是否启用后台线程处理IO
        /// </summary>
        [Description("是否启用后台线程处理IO")]
        public bool UseBackgroundThreadsForIO { get; set; } = true;

        /// <summary>
        /// 写入通道数量
        /// </summary>
        [Description("写入通道数量")]
        public int MaxChannelCount { get; set; } = 2;

        /// <summary>
        /// 读取Channel通道数
        /// </summary>
        public int ReadChannelCount { get; set; } = 2;


        /// <summary>
        /// 使用广播模式
        /// </summary>
        public bool SubscribeEnable { get; set; } = false;

        /// <summary>
        /// 广播模式的交换机名称
        /// </summary>
        public string SubscribeExchange { get; set; } = "ud_exchange_subscribe";

        /// <summary>
        /// 广播模式的队列名称 每个容器不一样，动态赋值！！！
        /// </summary>
        public string SubscribeQueue { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class RabbitExcepireItem
    {
        ///// <summary>
        ///// 队列名称 用于写入的时候查询
        ///// </summary>
        //[Description("队列名称 用于写入的时候查询")]
        //public string Key { get; set; } = "";

        ///// <summary>
        ///// 排序 在一个阶梯停留多久? 0不参与排序 1,2,3,4,5
        ///// </summary>
        //[Description("排序 在一个阶梯停留多久? 0不参与排序 1,2,3,4,5")]
        //public int Zindex { get; set; }

        /// <summary>
        /// 超时队列的待超时队列 在这个队列中等待过期
        /// talk_wait_hour_queue 这个是hour模式
        /// </summary>
        [Description("超时队列的待超时队列 在这个队列中等待过期 talk_wait_hour_queue 这个是hour模式")]
        public string ExpireExchange { get; set; } = "";

        /// <summary>
        /// 超时队列的待超时队列 在这个队列中等待过期
        /// talk_wait_hour_queue 这个是hour模式
        /// </summary>
        [Description("超时队列的待超时队列 在这个队列中等待过期 talk_wait_hour_queue 这个是hour模式")]
        public string ExpireQueue { get; set; } = "";

        ///// <summary>
        ///// 超时时间 单位毫秒
        ///// </summary>
        //[Description("超时时间 单位毫秒")]
        //public long ExpireTime { get; set; }

        /// <summary>
        /// 超时时间 单位秒
        /// </summary>
        public long ExpireSecond { get; set; }

        ///// <summary>
        ///// 停留次数 比如5表示命中5次后往前或者往后 有数据变动往前，没数据往后
        ///// </summary>
        //[Description("停留次数 比如5表示命中5次后往前或者往后 有数据变动往前，没数据往后")]
        //public int StayTime { get; set; }
    }

}
