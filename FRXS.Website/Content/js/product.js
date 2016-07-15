
//全局变量
var editId;

$(function () {
     var parm = getUrlParam("parm");
    alert("得到参数"+parm);
    //加载数据
    loadgrid();
    //高度自适应
    gridresize();
});


//获取url中的参数
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}

//实现对DataGird控件的绑定操作
function loadgrid() {
    $('#grid').datagrid({
        title: '',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../Product/GetList',          //Aajx地址
        sortName: 'Id',                 //排序字段
        sortOrder: 'desc',                  //排序方式
        idField: 'Id',                  //主键
        pageSize: 30,                       //每页条数
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: true,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('unselectAll');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        queryParams: {
            txtProductName: $("#txtProductName").val(),

        },
        onDblClickCell:function () {
            var selected = $('#grid').datagrid('getSelected');
            alert("得到选中数据行" + selected.ProductName);
        },
        columns: [[
              { field: 'ck', checkbox: true },   //选择
              { title: 'ProductName', field: 'ProductName', width: 80 },

              {
                field: 'opt', title: '操作', align: 'center', width: 100,
                formatter: function (value, rec) {
                    var str = "";
                    str += "<a class='rowbtn' onclick='edit(" + rec.Id + ")'>修改</a>";
                    str += "<a class='rowbtn' onclick='del(" + rec.Id + ")'>删除</a>";
                    return str;
                }
            }
        ]],
        toolbar: [{
            text: '添加',
            iconCls: 'icon-add',
            handler: add
        }, '-', {
            text: '删除',
            iconCls: 'icon-remove',
            handler: del
        }, '-', {
            text: '查找',
            iconCls: 'icon-search',
            handler: search
        }, '-', {
            id: 'btnReload',
            text: '刷新',
            iconCls: 'icon-reload',
            handler: function () {
                //实现刷新栏目中的数据
                $("#grid").datagrid("reload");
            }
        }]
    });
    
};

//窗口大小改变
$(window).resize(function () {
    gridresize();
});

//grid高度改变
function gridresize() {
    var h = ($(window).height() - $("#searchPlan").height() - 19);
    if ($("#searchPlan").is(":hidden")) {
        h = $(window).height() - 2;
    }
    $('#grid').datagrid('resize',{
        width: ($(window).width() - 2),
        height: h
    });
}

//查询
function search() {
    if ($("#searchPlan").is(":hidden")) {
        $('#grid').datagrid('resize', {
            height: ($(window).height() - $("#searchPlan").height() - 19)
        });
        $("#searchPlan").show();
    } else {
        $('#grid').datagrid('resize', {
            height: ($(window).height() - 2)
        });
        $("#searchPlan").hide();
    }
}

//添加
function add() {
    $("#formAdd").form("clear");
    $("#dlgAdd").dialog({
        title: '添加',
        iconCls: "icon-add",
        collapsible: true,
        minimizable: true,
        maximizable: true,
        resizable: true,
        width: $(window).width() * 0.5,
        height: $(window).height() * 0.5,
        modal: true,
        onClose: function () {
            
        },
        buttons: [{
            text: '保存',
            iconCls: 'icon-ok',
            handler: save
        }, {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                $('#dlgAdd').dialog('close');
            }
        }]
    });
}

//删除
function del(id) {
    var ids = "";
    if (!isNaN(id)) {
        ids = id;
    } else {
        var rows = $("#grid").datagrid("getSelections");
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].Id + ",";
        }
        ids = ids.substring(0, ids.length - 1);
    }
    if (ids != "") {
        //然后确认发送异步请求的信息到后台删除数据
        $.messager.confirm("提示", "您确认删除？", function(res) {
            if (res) {
                $.ajax({
                    url: '../Product/DeletebyIds',
                    type: 'post',
                    data: { ids: ids },
                    success: function(result) {
                        if (result=="success") {
                            //当删除完成之后清除信息 
                            $("#grid").datagrid("clearSelections");
                            $("#grid").datagrid("reload");
                        }
                    }
                });
            }
        });
    } else {
        $.messager.alert("提示", "请选择你要删除的数据");
    }
}
//清空查询表单
function reset() {
    $("#searchForm").form("clear");
}

//保存
function save() {
    var validate = $("#formAdd").form('validate');
    if (validate == false) {
        return false;
    }
    var data = $("#formAdd").serializeArray();
    $.ajax({
        url: "../Product/SaveProduct",
        type: "post",
        data: data,
        success: function (result) {
            if(result=="success") {
                $.messager.alert("提示", "操作成功", "info", function () {
                    $("#dlgAdd").dialog("close");
                    $("#grid").datagrid("reload");
                });
            }
        }
    });
}

//编辑
function edit(id) {
    $("#formAdd").form("clear");
    
    $.ajax({
        url: "../Product/GetProduct",
        type: "post",
        data: { id: id },
        dataType: 'json',
        success: function (obj) {
            $("#Id").val(obj.Id);
            $("#ProductName").val(obj.ProductName);

        }
    });


    $("#dlgAdd").dialog({
        title: '编辑',
        iconCls: "icon-edit",
        collapsible: true,
        minimizable: true,
        maximizable: true,
        resizable: true,
        width: $(window).width() * 0.5,
        height: $(window).height() * 0.5,
        modal: true,
        onClose: function () {

        },
        buttons: [{
            text: '保存',
            iconCls: 'icon-ok',
            handler: save
        }, {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                $('#dlgAdd').dialog('close');
            }
        }]
    });
}
