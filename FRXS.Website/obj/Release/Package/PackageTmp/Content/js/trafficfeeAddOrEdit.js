var id;

$(function () {

    $("#trOutReason").hide();
    $("#trCollectionNum").hide();
    $("#trFee").hide();

    id = frxs.getUrlParam("ID");

    $("#formAdd").form("clear");

    if (id) {
        $.ajax({
            url: "../TrafficFee/GetTrafficFee",
            type: "post",
            data: { id: id },
            dataType: 'json',
            success: function (obj) {
                $('#formAdd').form('load', obj);
                if (obj.IsPass == "是") {
                    $("#trOutReason").hide();
                    $("#trCollectionNum").show();
                    $("#trFee").show();
                } else {
                    $("#trOutReason").show();
                    $("#trCollectionNum").hide();
                    $("#trFee").show();
                }
            }
        });
    }

    $("input[name='IsPass']").click(function () {
        if ($(this).val() == "是") {  //通过
            $("#trOutReason").hide();
            $("#trCollectionNum").show();
            $("#trFee").show();

            $('input:radio[name="OutReason"]').attr("checked", false);
        } else {
            $("#trOutReason").show();
            $("#trCollectionNum").hide();
            $("#trFee").show();

            $("#CollectionNum").combobox('setValue', '');
        }
        $('#Fee').numberbox('setValue', '');
    });

    $("input[name='OutReason']").click(function () {
        if ($(this).val() == "体检") {
            $('#Fee').numberbox('setValue', 50.00);
        } else {
            $('#Fee').numberbox('setValue', 80.00);
        }
    });

    $('#IDCard').autocomplete("../TrafficFee/GetTrafficFeeAutoComplete", {
        dataType: 'json', //返回数据类型
        autoFill: false,
        selectFirst: false,
        extraParams: { IDCard: function () { return $('#IDCard').val(); } },
        parse: function (data) { //后台返回的数据传给自定义的rows  
            debugger;
            var rows = [];
            for (var i = 0; i < data.length; i++) {
                rows[rows.length] = {
                    data: data[i],
                    value: data[i].IDCard,
                    result: data[i].IDCard
                };
            }
            return rows;
        },
        formatItem: function (row, i, n) {
            return row.IDCard;
        }
    }).result(function (event, data, formatted) {
        debugger;
        if (!data) {
            $("#Name").val("");
            $("#AccountName").val("");
            $("#BankAccount").val("");
            $("#BankName").val("");
            $("#BZ1").val("");
        } else {
            $("#Name").val(data.Name);
            $("#AccountName").val(data.AccountName);
            $("#BankAccount").val(data.BankAccount);
            $("#BankName").val(data.BankName);
            $("#BZ1").val(data.BZ1);   //手机号码
        }
    });

    $('#IDCard').blur(function () {
        $("#IDCard").search();
    });

    var length = $(":input").length;
    $(":input").keyup(function (e) {
        var key = e.which;
        var index = $(":input").index(this);
        var newIndex;
        switch (key) {
            case 13:
                newIndex = index + 1;
                if (length - 1 == newIndex) {
                    newIndex = 0;
                }
                break;
            case 40:
                if (this.name != "IDCard") {
                    newIndex = index + 1;
                    if (length - 1 == newIndex) {
                        newIndex = 0;
                    }
                }
                break;
            case 38:
                newIndex = index - 1;
                if (0 == newIndex) {
                    newIndex = length;
                }
                break;
        }
        $(":input:eq(" + newIndex + ")").focus();

    });
});

function collectionNumOnSelect(rec) {
    if (rec.value == "0.5-1U") {
        $('#Fee').numberbox('setValue', 150.00);
    } else if (rec.value == "1.5U") {
        $('#Fee').numberbox('setValue', 200.00);
    } else {
        $('#Fee').numberbox('setValue', 220.00);
    }
}

//保存
function saveData() {
    $(this).attr("disabled", "disabled");
    var validate = $("#formAdd").form('validate');
    debugger;
    if (!validate) {
        //window.top.$.messager.alert("提示", "数据没有填写完整，请检查！", "info");
        var isPass = $('input[name="IsPass"]').filter(':checked').val();
        if (typeof (isPass) == "undefined") {
            window.top.$.messager.alert("提示", "没有选择'是否通过'值！", "info");
        } 
        return false;
    }
    var isPass = $('input[name="IsPass"]').filter(':checked').val();
    if (isPass == "是") {
        var collectionNum = $("#CollectionNum").combobox('getValue');
        if (collectionNum == "") {
            window.top.$.messager.alert("提示", "采集血量不能为空", "info");
            return false;
        }
    } else {
        var outReason = $('input[name="OutReason"]').filter(':checked').val();
        if (outReason == undefined) {
            window.top.$.messager.alert("提示", "淘汰方式不能为空", "info");
            return false;
        }
    }
    var data = $("#formAdd").serializeArray();
    window.parent.$(window.frameElement).closest(".window").find("#btnOk").addClass("easyui-linkbutton").linkbutton('disable');
    var loading = frxs.loading("正在加载中，请稍后...");
    $.ajax({
        url: "../TrafficFee/SaveTrafficFee",
        type: "post",
        dataType: "json",
        data: data,
        success: function (result) {
            loading.close();
            window.parent.$(window.frameElement).closest(".window").find("#btnOk").addClass("easyui-linkbutton").linkbutton('enable');
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    window.top.$.messager.alert("提示", "操作成功", "info", function () {
                        window.frameElement.wapi.$("#grid").datagrid("reload");
                        window.frameElement.wapi.focus();
                        frxs.pageClose();
                    });
                } else {
                    parent.$("#btnOk").linkbutton('enable');
                    window.top.$.messager.alert("提示", result.Info, "info");
                }
            }
        },
        error: function (request, textStatus, errThrown) {
            loading.close();
            window.parent.$(window.frameElement).closest(".window").find("#btnOk").addClass("easyui-linkbutton").linkbutton('enable');
            parent.$("#btnPackOk").linkbutton('enable');
            if (textStatus) {
                window.top.$.messager.alert("提示", textStatus, "info");
            } else if (errThrown) {
                window.top.$.messager.alert("提示", errThrown, "info");
            } else {
                window.top.$.messager.alert("提示", "出现错误", "info");
            }
        }
    });

    $(this).removeAttr("disabled");
}

var aCity = { 11: "北京", 12: "天津", 13: "河北", 14: "山西", 15: "内蒙古", 21: "辽宁", 22: "吉林", 23: "黑龙江", 31: "上海", 32: "江苏", 33: "浙江", 34: "安徽", 35: "福建", 36: "江西", 37: "山东", 41: "河南", 42: "湖北", 43: "湖南", 44: "广东", 45: "广西", 46: "海南", 50: "重庆", 51: "四川", 52: "贵州", 53: "云南", 54: "西藏", 61: "陕西", 62: "甘肃", 63: "青海", 64: "宁夏", 65: "新疆", 71: "台湾", 81: "香港", 82: "澳门", 91: "国外" }

function isCardID(sId) {
    var iSum = 0;
    var info = "";
    if (!/^\d{17}(\d|x)$/i.test(sId)) return "你输入的身份证长度或格式错误";
    sId = sId.replace(/x$/i, "a");
    if (aCity[parseInt(sId.substr(0, 2))] == null) return "你的身份证地区非法";
    sBirthday = sId.substr(6, 4) + "-" + Number(sId.substr(10, 2)) + "-" + Number(sId.substr(12, 2));
    var d = new Date(sBirthday.replace(/-/g, "/"));
    if (sBirthday != (d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate())) return "身份证上的出生日期非法";
    for (var i = 17; i >= 0; i--) iSum += (Math.pow(2, i) % 11) * parseInt(sId.charAt(17 - i), 11);
    if (iSum % 11 != 1) return "你输入的身份证号非法";
    return true;//aCity[parseInt(sId.substr(0,2))]+","+sBirthday+","+(sId.substr(16,1)%2?"男":"女")   
}

function luhmCheck(bancknum) {
    if (bancknum.length < 16 || bancknum.length > 19) {
        //alert("银行卡号长度必须在16到19之间");
        return false;
    }
    var num = /^\d*$/;  //全数字
    if (!num.exec(bancknum)) {
        //alert("请输入有效的银行账号！");
        return false;
    }
    //开头6位
    var strBin = "10,18,30,35,37,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,58,60,62,65,68,69,84,87,88,94,95,98,99";
    if (strBin.indexOf(bancknum.substring(0, 2)) == -1) {
        //$("#banknoInfo").html("银行卡号开头6位不符合规范");
        //alert("请输入有效的银行账号！");
        return false;
    }
    var lastNum = bancknum.substr(bancknum.length - 1, 1);//取出最后一位（与luhm进行比较）

    var first15Num = bancknum.substr(0, bancknum.length - 1);//前15或18位
    var newArr = new Array();
    for (var i = first15Num.length - 1; i > -1; i--) {    //前15或18位倒序存进数组
        newArr.push(first15Num.substr(i, 1));
    }
    var arrJiShu = new Array();  //奇数位*2的积 <9
    var arrJiShu2 = new Array(); //奇数位*2的积 >9

    var arrOuShu = new Array();  //偶数位数组
    for (var j = 0; j < newArr.length; j++) {
        if ((j + 1) % 2 == 1) {//奇数位
            if (parseInt(newArr[j]) * 2 < 9)
                arrJiShu.push(parseInt(newArr[j]) * 2);
            else
                arrJiShu2.push(parseInt(newArr[j]) * 2);
        }
        else //偶数位
            arrOuShu.push(newArr[j]);
    }

    var jishu_child1 = new Array();//奇数位*2 >9 的分割之后的数组个位数
    var jishu_child2 = new Array();//奇数位*2 >9 的分割之后的数组十位数
    for (var h = 0; h < arrJiShu2.length; h++) {
        jishu_child1.push(parseInt(arrJiShu2[h]) % 10);
        jishu_child2.push(parseInt(arrJiShu2[h]) / 10);
    }

    var sumJiShu = 0; //奇数位*2 < 9 的数组之和
    var sumOuShu = 0; //偶数位数组之和
    var sumJiShuChild1 = 0; //奇数位*2 >9 的分割之后的数组个位数之和
    var sumJiShuChild2 = 0; //奇数位*2 >9 的分割之后的数组十位数之和
    var sumTotal = 0;
    for (var m = 0; m < arrJiShu.length; m++) {
        sumJiShu = sumJiShu + parseInt(arrJiShu[m]);
    }

    for (var n = 0; n < arrOuShu.length; n++) {
        sumOuShu = sumOuShu + parseInt(arrOuShu[n]);
    }

    for (var p = 0; p < jishu_child1.length; p++) {
        sumJiShuChild1 = sumJiShuChild1 + parseInt(jishu_child1[p]);
        sumJiShuChild2 = sumJiShuChild2 + parseInt(jishu_child2[p]);
    }
    //计算总和
    sumTotal = parseInt(sumJiShu) + parseInt(sumOuShu) + parseInt(sumJiShuChild1) + parseInt(sumJiShuChild2);

    //计算Luhm值
    var k = parseInt(sumTotal) % 10 == 0 ? 10 : parseInt(sumTotal) % 10;
    var luhm = 10 - k;

    if (lastNum != luhm) {
        //alert("银行卡号必须符合Luhm校验");
        return false;
    } else {
        //alert("Luhm验证通过");
        return true;
    }
}

$.extend($.fn.validatebox.defaults.rules, {
    idcared: {
        validator: function (value, param) {
            var flag = isCardID(value);
            return flag == true ? true : false;
        },
        message: '不是有效的身份证号码'
    },
    bankIdCard: {
        validator: function (value, param) {
            var flag = luhmCheck(value);
            return flag == true ? true : false;
        },
        message: '不是有效的银行卡号'
    },
    phoneRex: {
        validator: function (value) {
            var rex = /^1[3-8]+\d{9}$/;
            //var rex=/^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/;
            //区号：前面一个0，后面跟2-3位数字 ： 0\d{2,3}
            //电话号码：7-8位数字： \d{7,8
            //分机号：一般都是3位数字： \d{3,}
            //这样连接起来就是验证电话的正则表达式了：/^((0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/		 
            var rex2 = /^((0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/;
            if (rex.test(value) || rex2.test(value)) {
                // alert('t'+value);
                return true;
            } else {
                //alert('false '+value);
                return false;
            }
        },
        message: '请输入正确电话或手机格式'
    }
});
