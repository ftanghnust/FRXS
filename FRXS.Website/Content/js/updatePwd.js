var id;

$(function () {

});

//保存
function saveData() {
    $(this).attr("disabled", "disabled");
    var validate = $("#formAdd").form('validate');
    if (!validate) {
        //window.top.$.messager.alert("提示", "数据没有填写完整，请检查！", "info");
        return false;
    }
    //var data = $("#formAdd").serializeArray();
    window.parent.$(window.frameElement).closest(".window").find("#btnOk").addClass("easyui-linkbutton").linkbutton('disable');
    var loading = frxs.loading("正在加载中，请稍后...");
    $.ajax({
        url: "../OrgUser/UpdatePwdHandle",
        type: "post",
        dataType: "json",
        data: { OldPwd: $("#OldPwd").val(), NewPwd: $("#NewPwd").val(), NewPwd1: $("#NewPwd1").val() },
        success: function (result) {
            loading.close();
            window.parent.$(window.frameElement).closest(".window").find("#btnOk").addClass("easyui-linkbutton").linkbutton('enable');
            if (result != undefined && result.Info != undefined) {
                if (result.Flag == "SUCCESS") {
                    window.top.$.messager.alert("提示", "修改成功", "info", function () {
                        top.location.href = 'Home/Logout';
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


$.extend($.fn.validatebox.defaults.rules, {
    /*必须和某个字段相等*/
    equalTo: {
        validator: function (value, param) {
            return $(param[0]).val() == value;
        },
        message: '两次输入密码不匹配'
    }

});

