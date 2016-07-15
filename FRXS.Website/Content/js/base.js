
$(function () {
    InitLeftMenu();
    tabClose();
    tabCloseEven();
    //clockon();
    $('#tabs').tabs('add', {
        title: '欢迎',
        //content: createFrame('muster/welcome')
        content: '<H1>WELCOME</H1>'
    });

    atuoTheme();
});

//初始化左侧
function InitLeftMenu() {

    $("#nav").accordion({ animate: true });

    $.each(_menus.menus, function (i, n) {
        var menulist = '';
        menulist += '<ul>';
        $.each(n.menus, function (j, o) {
            menulist += '<li><div><a ref="' + o.menuid + '" href="#" rel="' + o.url + '" ><span class="icon ' + o.icon + '" >&nbsp;</span><span class="nav">' + o.menuname + '</span></a></div></li> ';
        });
        menulist += '</ul>';

        $('#nav').accordion('add', {
            title: n.menuname,
            content: menulist,
            iconCls: 'icon ' + n.icon
        });

    });

    $('.easyui-accordion li a').click(function () {
        var tabTitle = $(this).children('.nav').text();

        var url = $(this).attr("rel");
        var menuid = $(this).attr("ref");
        var icon = getIcon(menuid, icon);

        addTab(tabTitle, url, icon);
        $('.easyui-accordion li div').removeClass("selected");
        $(this).parent().addClass("selected");
    }).hover(function () {
        $(this).parent().addClass("hover");
    }, function () {
        $(this).parent().removeClass("hover");
    });

    //选中第一个
    var panels = $('#nav').accordion('panels');
    var t = panels[0].panel('options').title;
    $('#nav').accordion('select', t);
}
//获取左侧导航的图标
function getIcon(menuid) {
    var icon = 'icon ';
    $.each(_menus.menus, function (i, n) {
        $.each(n.menus, function (j, o) {
            if (o.menuid == menuid) {
                icon += o.icon;
            }
        });
    });

    return icon;
}

function addTab(subtitle, url, icon) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        $('#tabs').tabs('add', {
            title: subtitle,
            content: createFrame(url),
            closable: true,
            icon: icon
        });
    } else {
        $('#tabs').tabs('select', subtitle);
        $('#mm-tabupdate').click();
    }
    tabClose();
}

function createFrame(url) {
    var s = '<iframe scrolling="auto" frameborder="0" src="' + url + '" style="width:100%;height:99%;"></iframe>';
    return s;
}

function tabClose() {
    /*双击关闭TAB选项卡*/
    $(".tabs-inner").dblclick(function () {
        var subtitle = $(this).children(".tabs-closable").text();
        $('#tabs').tabs('close', subtitle);
    });
    /*为选项卡绑定右键*/
    $(".tabs-inner").bind('contextmenu', function (e) {
        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        var subtitle = $(this).children(".tabs-closable").text();

        $('#mm').data("currtab", subtitle);
        $('#tabs').tabs('select', subtitle);
        return false;
    });
}
//绑定右键菜单事件
function tabCloseEven() {
    //刷新
    $('#mm-tabupdate').click(function () {
        var currTab = $('#tabs').tabs('getSelected');
        var url = $(currTab.panel('options').content).attr('src');
        $('#tabs').tabs('update', {
            tab: currTab,
            options: {
                content: createFrame(url)
            }
        });
    });
    //关闭当前
    $('#mm-tabclose').click(function () {
        var currtabTitle = $('#mm').data("currtab");
        $('#tabs').tabs('close', currtabTitle);
    });
    //全部关闭
    $('#mm-tabcloseall').click(function () {
        $('.tabs-inner span').each(function (i, n) {
            var t = $(n).text();
            $('#tabs').tabs('close', t);
        });
    });
    //关闭除当前之外的TAB
    $('#mm-tabcloseother').click(function () {
        $('#mm-tabcloseright').click();
        $('#mm-tabcloseleft').click();
    });
    //关闭当前右侧的TAB
    $('#mm-tabcloseright').click(function () {
        var nextall = $('.tabs-selected').nextAll();
        if (nextall.length == 0) {
            //msgShow('系统提示','后边没有啦~~','error');
            //alert('后边没有啦~~');
            return false;
        }
        nextall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            $('#tabs').tabs('close', t);
        });
        return false;
    });
    //关闭当前左侧的TAB
    $('#mm-tabcloseleft').click(function () {
        var prevall = $('.tabs-selected').prevAll();
        if (prevall.length == 0) {
            //alert('到头了，前边没有啦~~');
            return false;
        }
        prevall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            $('#tabs').tabs('close', t);
        });
        return false;
    });

    //退出
    $("#mm-exit").click(function () {
        $('#mm').menu('hide');
    });
}
//本地时钟
function clockon() {
    var now = new Date();
    var year = now.getFullYear(); //getFullYear getYear
    var month = now.getMonth();
    var date = now.getDate();
    var day = now.getDay();
    var hour = now.getHours();
    var minu = now.getMinutes();
    var sec = now.getSeconds();
    var week;
    month = month + 1;
    if (month < 10) month = "0" + month;
    if (date < 10) date = "0" + date;
    if (hour < 10) hour = "0" + hour;
    if (minu < 10) minu = "0" + minu;
    if (sec < 10) sec = "0" + sec;
    var arrWeek = new Array("星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六");
    week = arrWeek[day];
    var time = "";
    time = year + "年" + month + "月" + date + "日" + " " + hour + ":" + minu + ":" + sec + " " + week;

    $("#bgclock").html(time);

    var timer = setTimeout("clockon()", 1000);
}



/**************************绑定导航栏目**********************************************/
var _menus = {
    "menus": [
        {
            "menuid": "1",
            "icon": "icon-role",
            "menuname": "机采交通费",
            "menus": [
                { "menuid": "dcfee5be-651e-4aac-8968-ce127e457454", "menuname": "新增", "icon": "icon-add", "url": 'orguser/index' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b1", "menuname": "查询", "icon": "icon-search", "url": 'control/accordion' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b2", "menuname": "combobox", "icon": "icon-set", "url": 'control/combobox' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b3", "menuname": "dialog", "icon": "icon-set", "url": 'control/dialog' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b4", "menuname": "messager", "icon": "icon-set", "url": 'control/messager' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b5", "menuname": "tree", "icon": "icon-set", "url": 'control/tree' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b6", "menuname": "combotree", "icon": "icon-set", "url": 'control/combotree' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e4567b8", "menuname": "slider", "icon": "icon-set", "url": 'control/slider' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e456718", "menuname": "tabs", "icon": "icon-set", "url": 'control/tabs' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e456728", "menuname": "input", "icon": "icon-set", "url": 'control/input' },
                { "menuid": "24ea7f2f-33c3-4e0d-8faa-7a114e456738", "menuname": "treegrid", "icon": "icon-set", "url": 'control/treegrid' }
            ]
        }
        //,{
     //    "menuid": "2", "icon": "icon-set", "menuname": "组织管理",
     //    "menus": [{ "menuid": "21", "menuname": "角色管理", "icon": "icon-log", "url": '' },
     //            { "menuid": "22", "menuname": "角色权限", "icon": "icon-database", "url": '' }
     //    ]
     //}
    ]
};

function loginOut() {
    $.messager.confirm('提示', "确定退出系统！", function (res) {
        if (res) {
            location.href = 'Home/Logout';
        }
    });
}
