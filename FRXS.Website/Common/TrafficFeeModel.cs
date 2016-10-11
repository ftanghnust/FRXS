using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FRXS.Website.Common
{
    public class TrafficFeeModel
    {
        [DisplayName("身份证号码")]
        public string IDCard { get; set; }
        [DisplayName("献血者姓名")]
        public string Name { get; set; }
        [DisplayName("手机号码")]
        public string BZ1 { get; set; }
        [DisplayName("淘汰方式")]
        public string OutReason { get; set; }
        [DisplayName("采集血量")]
        public string CollectionNum { get; set; }
        [DisplayName("应付金额（元）")]
        public Nullable<decimal> Fee { get; set; }
        [DisplayName("户名")]
        public string AccountName { get; set; }
        [DisplayName("银行帐号")]
        public string BankAccount { get; set; }
        [DisplayName("开户行")]
        public string BankName { get; set; }
        [DisplayName("工作人员签名")]
        public string WorkMan { get; set; }
        [DisplayName("创建时间")]
        public Nullable<System.DateTime> CreateTime { get; set; }
    }
}