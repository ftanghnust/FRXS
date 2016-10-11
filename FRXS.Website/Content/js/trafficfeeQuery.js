
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
        title: '机采交通费信息列表',                      //标题
        iconCls: 'icon-view',               //icon
        methord: 'post',                    //提交方式
        url: '../TrafficFee/GetList',          //Aajx地址
        sortName: 'CreateTime',                 //排序字段
        sortOrder: 'desc',                  //排序方式
        idField: 'ID',                  //主键
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
        onClickRow: function (rowIndex) {
            $('#grid').datagrid('unselectAll');
            $('#grid').datagrid('selectRow', rowIndex);
        },
        // onClickCell: onClickCell,
        onLoadSuccess: function () {
            //$('#grid').datagrid('keyCtr');
        },
        queryParams: {
            txtName: $.trim($("#txtName").val()),
            txtIDCard: $.trim($("#txtIDCard").val()),
            OutReason: $("#OutReason").combobox("getValue"),
            CollectionNum: $("#CollectionNum").combobox("getValue"),
            StartDate: $.trim($("#StartDate").val()),
            EndDate: $.trim($("#EndDate").val()),
            BZ1: $.trim($("#BZ1").val()),
            Type: 'Query'
        },

        frozenColumns: [[
            //冻结列
            { field: 'ck', checkbox: true }, //选择
            { title: '身份证号码', field: 'IDCard', width: 160 },
            { title: '献血者姓名', field: 'Name', width: 95, align: 'left' }
        ]],
        columns: [[
            { title: '手机号码', field: 'BZ1', width: 100, align: 'center' },
            { title: '淘汰方式', field: 'OutReason', width: 65, align: 'center' },
            { title: '采集血量', field: 'CollectionNum', width: 80, align: 'right' },
            {
                title: '应付金额（元）', field: 'Fee', width: 120, align: 'right',
                formatter: function (value, rec) {
                    return parseFloat(value).toFixed(2);
                }

            },
            { title: '户名', field: 'AccountName', width: 80 },
            { title: '银行帐号', field: 'BankAccount', width: 180 },
            { title: '开户行', field: 'BankName', width: 180 },
            { title: '工作人员签名', field: 'WorkMan', width: 105 },
            { title: '创建时间', field: 'CreateTime', width: 150, align: 'center' }
        ]],
        toolbar: [{ id: 'btnExport', text: '导出', iconCls: 'icon-export', handler: Export }]
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
    $('#grid').datagrid('resize', {
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


//清空查询表单
function reset() {
    $("#searchForm").form("clear");
    $('#OutReason').combobox('setValue', '');
    $('#CollectionNum').combobox('setValue', '');
}


//导出事件
function Export() {
    location.href = "../TrafficFee/DataExport?txtName=" + $.trim($("#txtName").val()) + "&txtIDCard=" + $.trim($("#txtIDCard").val()) + "&OutReason=" + $("#OutReason").combobox("getValue") + "&CollectionNum=" + $("#CollectionNum").combobox("getValue") + "&StartDate=" + $.trim($("#StartDate").val()) + "&EndDate=" + $.trim($("#EndDate").val()) + "&BZ1=" + $.trim($("#BZ1").val());
}


//function Export() {
//    var loading = window.top.frxs.loading("正在导出数据...");

//    //获取全部数据后导出到Excel
//    $.ajax({
//        url: '../TrafficFee/GetList',          //Aajx地址
//        type: "post",
//        dataType: "json",
//        data: {
//            //查询条件
//            txtName: $.trim($("#txtName").val()),
//            txtIDCard: $.trim($("#txtIDCard").val()),
//            OutReason: $("#OutReason").combobox("getValue"),
//            CollectionNum: $("#CollectionNum").combobox("getValue"),
//            StartDate: $.trim($("#StartDate").val()),
//            EndDate: $.trim($("#EndDate").val()),
//            BZ1: $.trim($("#BZ1").val()),
//            Type: 'Query',
//            page: 1,
//            sort: 'CreateTime',
//            order: 'desc',
//            rows: 100000000//页数
//        },
//        success: function (result) {
//            if (result != undefined && result.rows != undefined) {
//                var rows = result.rows;
//                if (rows.length <= 0) {
//                    $.messager.alert("提示", "没有查询到数据。", "info");
//                    return false;
//                }

//                //标题行
//                var trtdCode = "<tr>";
//                trtdCode += "<td style='height:24px'>序号</td>";
//                trtdCode += "<td>身份证号码</td>";
//                trtdCode += "<td>献血者姓名</td>";
//                trtdCode += "<td>手机号码</td>";
//                trtdCode += "<td>淘汰方式</td>";
//                trtdCode += "<td>采集血量</td>";
//                trtdCode += "<td>应付金额（元）</td>";
//                trtdCode += "<td>户名</td>";
//                trtdCode += "<td>银行帐号</td>";
//                trtdCode += "<td>开户行</td>";
//                trtdCode += "<td>工作人员签名</td>";
//                trtdCode += "<td>创建时间</td>";
//                trtdCode += "</tr>";

//                //装入数据
//                for (var i = 0; i < rows.length; i++) {
//                    trtdCode += "<tr>";
//                    trtdCode += "<td style=\"mso-number-format:'\@';\">" + (i + 1) + "</td>";
//                    trtdCode += "<td style='height:20px' x:str=\"'" + rows[i].IDCard + "\">" + rows[i].IDCard + "</td>";
//                    trtdCode += "<td>" + frxs.replaceCode(rows[i].Name) + "</td>";
//                    trtdCode += "<td>" + frxs.replaceCode(rows[i].BZ1) + "</td>";
//                    trtdCode += "<td>" + frxs.replaceCode(rows[i].OutReason) + "</td>";
//                    trtdCode += "<td>" + frxs.replaceCode(rows[i].CollectionNum) + "</td>";
//                    trtdCode += "<td style='mso-number-format:\"#,##0.00\";'>" + rows[i].Fee + "</td>";
//                    trtdCode += "<td>" + rows[i].AccountName + "</td>";
//                    trtdCode += "<td style=\"mso-number-format:'\@';\">" + rows[i].BankAccount + "</td>";
//                    trtdCode += "<td>" + rows[i].BankName + "</td>";
//                    trtdCode += "<td>" + rows[i].WorkMan + "</td>";
//                    trtdCode += "<td style=\"mso-number-format:'\@';\">" + rows[i].CreateTime + "</td>";
//                    trtdCode += "</tr>";
//                }
//                debugger;
//                //文件流
//                var dataCode = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>export</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table border="1">{table}</table></body></html>';
//                dataCode = dataCode.replace("{table}", trtdCode);

//                if (dataCode) {
//                    event.preventDefault();
//                    var bb = self.Blob;
//                    saveAs(
//                        new bb(
//                            ["\ufeff" + dataCode] //\ufeff防止utf8 bom防止中文乱码
//                            , { type: "html/plain;charset=utf8" }
//                        ), "机采交通费导出_" + frxs.nowDateTime("yyyyMMdd") + ".xls"
//                    );
//                }
//            }
//            loading.close();
//        },
//        error: function (request, textStatus, errThrown) {
//            if (textStatus) {
//                $.messager.alert("提示", textStatus, "info");
//            } else if (errThrown) {
//                $.messager.alert("提示", errThrown, "info");
//            } else {
//                $.messager.alert("提示", "出现错误", "info");
//            }
//            loading.close();
//        }
//    });

//}

