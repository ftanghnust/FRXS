var frxs= {
    dateFormat: function (val) {
        /// <summary>日期格式化</summary>
        if (!val) { return ""; }
        var date = new Date(val);

        var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : +("0") + (date.getMonth() + 1);
        var day = date.getDate() > 9 ? date.getDate() : ("0" + date.getDate());
        var hours = date.getHours() > 9 ? date.getHours() : ("0" + date.getHours());
        var minutes = date.getMinutes() > 9 ? date.getMinutes() : ("0" + date.getMinutes());
        var seconds = date.getSeconds() > 9 ? date.getSeconds() : ("0" + date.getSeconds()); 

        return date.getFullYear() + "-" + month + "-" + day + " " + hours + ":" + minutes;
    },
    //获取url中的参数
    getUrlParam: function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
        var r = window.location.search.substr(1).match(reg); //匹配目标参数
        if (r != null) return unescape(r[2]);
        return null; //返回参数值
    },
    //js创建guid
    newGuid: function() {
        var guid = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            guid += n;
            if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                guid += "-";
        }
        return guid;
    }
}
