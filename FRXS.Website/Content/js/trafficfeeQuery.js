
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
            EndDate: $.trim($("#EndDate").val())
        },

        frozenColumns: [[
            //冻结列
            { field: 'ck', checkbox: true }, //选择
            { title: '身份证号码', field: 'IDCard', width: 160 },
            { title: '献血者姓名', field: 'Name', width: 95, align: 'left' }
        ]],
        columns: [[
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
        ]]
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

