
//全局变量
var editId;

$(function () {
    //加载数据
    loadgrid();
    //高度自适应
    gridresize();
});


//实现对DataGird控件的绑定操作
function loadgrid() {
    $('#grid').datagrid({
        title: '列表',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../OrgUser/GetList',          //Aajx地址
        sortName: 'UserId',                 //排序字段
        sortOrder: 'desc',                  //排序方式
        idField: 'UserId',                  //主键
        pageSize: 10,                       //每页条数
        pageList: [5, 10, 1000],//可以设置每页记录条数的列表 
        fit: false,                         //分页在最下面
        pagination: true,                   //是否显示分页
        rownumbers: true,                   //显示行编号
        fitColumns: false,                   //列均匀分配
        striped: false,                     //奇偶行是否区分
        //设置点击行为单选，点击行中的复选框为多选
        checkOnSelect: true,
        selectOnCheck: true,
        onClickRow: function(rowIndex) {
            $('#grid').datagrid('unselectAll');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        onClickCell: onClickCell,
        onLoadSuccess:function () {
            //$('#grid').datagrid('keyCtr');
        },
        queryParams: {
            txtUserName: $("#txtUserName").val(),
            txtUserTrueName: $("#txtUserTrueName").val(),
            txtPassword: $("#txtPassword").val(),
        },
        
        frozenColumns: [[
            //冻结列
            { field: 'ck', checkbox: true }, //选择
             { title: 'UserName', field: 'UserName', width: 180, editor: 'text', formatter: frxs.dateFormat },
            { title: 'UserTrueName', field: 'UserTrueName', width: 180, editor: 'numberbox' }
        ]],
        columns: [[
            { title: 'Password', field: 'Password', width: 180, editor: 'datebox', sortable: true },
            { title: 'Sex', field: 'Sex', width: 180, editor: 'combobox' },
            { title: 'Age', field: 'Age', width: 180, editor: 'text' },
            { title: 'Tel', field: 'Tel', width: 180, editor: 'text' },
            { title: 'Phone', field: 'Phone', width: 180, editor: 'text' },
            //{ title: 'Com', field: 'Com', width: 180, editor: 'text' },
            //{ title: 'Address', field: 'Address', width: 180, editor: 'text' },
            //{ title: 'Shen', field: 'Shen', width: 180, editor: 'text' },
            //{ title: 'Shi', field: 'Shi', width: 180, editor: 'text' },
            //{ title: 'Qu', field: 'Qu', width: 180, editor: 'text' },
            //{ title: 'Xian', field: 'Xian', width: 180, editor: 'text' },
            {
                field: 'opt',
                title: '操作',
                align: 'center',
                width: 200,
                formatter: function(value, rec) {
                    var str = "";
                    str += "<a class='rowbtn' onclick='edit(" + rec.UserId + ")'>修改</a>";
                    str += "<a class='rowbtn' onclick='del(" + rec.UserId + ")'>删除</a>";
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
                handler: function() {
                    //实现刷新栏目中的数据
                    $("#grid").datagrid("reload");
                }
            }, '-', {
                id: 'btnReload',
                text: '插入行',
                iconCls: 'icon-reload',
                handler: function () {
                    var selected = $('#grid').datagrid('getSelected');
                    var index = $('#grid').datagrid('getRowIndex', selected);
                    selected.UserName = "UserName";
                    selected.UserTrueName = "121212121212";
                    //插入行
                    $('#grid').datagrid('insertRow', {
                        row: selected
                    });
                }
            }, '-', {
                id: 'btnReload',
                text: '更新行',
                iconCls: 'icon-reload',
                handler: function () {
                    var selected = $('#grid').datagrid('getSelected');
                    var index = $('#grid').datagrid('getRowIndex', selected);
                    selected.UserName = "UserName";
                    selected.UserTrueName = "121212121212";
                    //更新行
                    $('#grid').datagrid('updateRow', {
                        index: index,
                        row: selected
                    });
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
            ids += rows[i].UserId + ",";
        }
        ids = ids.substring(0, ids.length - 1);
    }
    if (ids != "") {
        //然后确认发送异步请求的信息到后台删除数据
        $.messager.confirm("提示", "您确认删除？", function(res) {
            if (res) {
                $.ajax({
                    url: '../OrgUser/DeletebyIds',
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
        url: "../OrgUser/SaveOrgUser",
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
        url: "../OrgUser/GetOrgUser",
        type: "post",
        data: { id: id },
        dataType: 'json',
        success: function (obj) {
            $("#UserId").val(obj.UserId);
            $("#UserName").val(obj.UserName);
            $("#UserTrueName").val(obj.UserTrueName);
            $("#Password").val(obj.Password);

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












$.extend($.fn.datagrid.methods, {
    //编辑单元格事件
    editCell: function(jq,param){
        return jq.each(function(){
            var opts = $(this).datagrid('options');
            var fields = $(this).datagrid('getColumnFields',true).concat($(this).datagrid('getColumnFields'));
            for(var i=0; i<fields.length; i++){
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor1 = col.editor;
                if (fields[i] != param.field){
                    col.editor = null;
                }
            }
            
            $(this).datagrid('beginEdit', param.index);
            
            var ed = $(this).datagrid('getEditor', param);

            if (ed) {
                if ($(ed.target).hasClass('textbox-f')) {
                    var obj = $(ed.target).textbox('textbox');
                    objEvent(obj,ed.field);
                } else {
                    objEvent($(ed.target), ed.field);
                }
            }

            for(var i=0; i<fields.length; i++){
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor = col.editor1;
            }
        });
    }
});

var valueinput = "";

//绑定对象事件
function objEvent(obj,field) {
    obj.select();
    valueinput = $(obj).val();
    obj.bind("keydown", function (e) {
        if (e.keyCode == 13) {
            if (field == "UserName") {
                var control = this;
                if ($(this).val() != valueinput) {

                    showDialog($(this).val());
                    //$.messager.alert("提示", "弹窗,带入参数" + $(this).val());
                } else {
                    nextControl();
                }
            } else {
                nextControl();
            }
        }
    });

    //obj.bind("blur", function () {
    //    $(this).val(valueinput);
    //});
}


function nextControl() {
    var selected = $('#grid').datagrid('getSelected');
    var index = $('#grid').datagrid('getRowIndex', selected);
    var currentField = "UserName";
    var fields = "UserName,UserTrueName,Password,Sex,Age,Tel,Phone".split(',');
    for (var i = 0; i < fields.length; i++) {
        var ciField = fields[i];
        if (ciField == editfield) {
            currentField = fields[i + 1];
            if (currentField == undefined) {
                currentField = "UserName";
                index = index + 1;
                $('#grid').datagrid('unselectAll');
                $('#grid').datagrid('getRowIndex', index);
            }
        }
    }
    
    var len = $("#grid").datagrid("getRows").length;
    if (len == index) {
        //插入行
        $('#grid').datagrid('insertRow', {
            row: {}
        });
        onClickCell(index, currentField);
    } else {
        onClickCell(index, currentField);
    }
}

		
var editIndex = undefined;
var editfield = undefined;
function endEditing() {
    if (editIndex == undefined) {
        return true;
    }
    if ($('#grid').datagrid('validateRow', editIndex)) {
        $('#grid').datagrid('endEdit', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

function onClickCell(index, field) {
    editfield = field;

    if (endEditing()){
        $('#grid').datagrid('selectRow', index)
                .datagrid('editCell', {index:index,field:field});
        editIndex = index;
    }
}


function showDialog(parm) {
    $("#dialog").dialog({
        title: '添加',
        iconCls: "icon-add",
        modal: true,                //是否有遮罩
        collapsible: true,          //是否能收缩
        minimizable: true,          //是否有最小化
        maximizable: true,          //是否有最大化
        resizable: true,            //是否能拉升
        width: $(window).width() * 0.5,
        height: $(window).height() * 0.5,
        //href: "../product/index",
        content: "<iframe scrolling=\"auto\"  frameborder=\"0\" src='../product/index?parm=" + parm + "' style=\"width:100%;height:99%\"></iframe>",
        onClose: function () {
            $.messager.alert("窗口", "你关闭了窗口", "info");
        },
        buttons: [{
            text: '保存',
            iconCls: 'icon-ok',
            handler: function () {
                $.messager.alert("窗口", "你点击了保存", "info");
            }
        }, {
            text: '关闭',
            iconCls: 'icon-cancel',
            handler: function () {
                $('#dialog').dialog('close');
            }
        }]
    });
}
