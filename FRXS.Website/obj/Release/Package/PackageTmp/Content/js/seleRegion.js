//确认
function sure() {

    //保存CooKie
    //过期时间 30分钟
    var date = new Date();
    date.setTime(date.getTime() + (120 * 60 * 1000));

    var Region = $("#region").combobox('getValue');  //获取区域值

    debugger;
    //调用保存
    $.cookie("Region", Region, { path: "/", expires: date });

    location.href = '../Home';
}

//取消
function cancel() {

    location.href = '../Home/Logout';
}