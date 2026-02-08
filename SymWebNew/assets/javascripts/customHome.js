$(function () {
 
    //InitAccordions();
    goHome();

})
 
function goHome() {
   
    $(".selectDropdown").select2();
    $(".selectDropdown").trigger('change');
    $(".selectDropdown").removeClass('form-control');
    function goBack() {
        window.history.back();
    }
    var url = "";
    $(".dialog-alert").dialog({
        autoOpen: false,
        resizable: false,
        //height: 170,
        title: 'Shampan ERP',
        width: 350,
        show: {
            effect: 'drop', direction: "up", effect: "blind",
            duration: 300
        },
        hide: {
            effect: "explode",
            duration: 300
        },
        modal: true,
        draggable: true,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close").hide();
            var msg = '@Session["result"]';
            msg = msg.split("~");
            $(".ui-dialog").addClass('' + msg[0]);
            $(".msgg").html("" + msg[1]);
        },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
                window.location.reload(true);

            },
            "Cancel": function () {
                $(this).dialog("close");
                window.location.reload(true);
            }
        }
    });



    $(".dialog-create").dialog({
        title: 'Create',
        autoOpen: false,
        resizable: false,
        width: 400,
        show: {
            effect: 'drop', direction: "up", effect: "blind",
            duration: 300
        },
        hide: {
            effect: "explode",
            duration: 300
        },
        modal: true,
        draggable: true,
        open: function (event, ui) {

            $(".ui-dialog-titlebar-close").hide();
            $(".ui-dialog").removeClass('Success');
            $(".ui-dialog").removeClass('Fail');
            $(this).load(url);
        }
    });
    $(".dialog-edit").dialog({
        title: 'Update',
        autoOpen: false,
        resizable: false,
        width: 400,
        show: {
            effect: 'drop', direction: "up", effect: "blind",
            duration: 300
        },
        hide: {
            effect: "explode",
            duration: 300
        },
        modal: true,
        draggable: true,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close").hide();
            $(".ui-dialog").removeClass('Success');
            $(".ui-dialog").removeClass('Fail');
            $(this).load(url);
        }
    });
    $(".dialog-confirm").dialog({
        autoOpen: false,
        resizable: false,
        title: 'Shampan ERP',
        height: 170,
        width: 350,
        show: {
            effect: 'drop', direction: "up", effect: "blind",
            duration: 300
        },
        hide: {
            effect: "explode",
            duration: 300
        },
        modal: true,
        draggable: true,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close").hide();
            $(".ui-dialog").removeClass('Success');
            $(".ui-dialog").removeClass('Fail');

        },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
                window.location.href = url;
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });
    $(".dialog-detail").dialog({
        title: 'View User',
        autoOpen: false,
        resizable: false,
        width: 400,
        show: {
            effect: 'drop', direction: "up", effect: "blind",
            duration: 300
        },
        hide: {
            effect: "explode",
            duration: 300
        },
        modal: true,
        draggable: true,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close").hide();
            $(this).load(url);
        },
        buttons: {
            "Close": function () {
                $(this).dialog("close");
            }
        }
    });
    $(".dialog-report").dialog({
        
        title: 'Report',
        autoOpen: false,
        resizable: false,
        width: 400,
        show: {
            effect: 'drop', direction: "up", effect: "blind",
            duration: 300
        },
        hide: {
            effect: "explode",
            duration: 300
        },
        modal: true,
        draggable: true,
        open: function (event, ui) {

            $(".ui-dialog-titlebar-close").hide();
            $(".ui-dialog").removeClass('Success');
            $(".ui-dialog").removeClass('Fail');
            $(this).load(url);
        }
    });

    $(".lnkCreate").live("click", function (e) {
        url = $(this).attr('href');
        $(".dialog-create").dialog('open');

        return false;
    });
    $(".btnCreate").live("click", function (e) {
        url = $(this).attr('data-url');
        $(".dialog-create").dialog('open');

        return false;
    });
    $(".lnkEdit").live("click", function (e) {
        url = $(this).attr('href');
        $(".dialog-edit").dialog('open');

        return false;
    });

    $(".lnkDelete").live("click", function (e) {
        url = $(this).attr('href');
        $(".dialog-confirm").dialog('open');

        return false;
    });

    $(".lnkDetail").live("click", function (e) {
        url = $(this).attr('href');
        $(".dialog-detail").dialog('open');

        return false;
    });
    $(".btncancel").live("click", function (e) {
        $(".dialog-edit").dialog("close");
        $(".dialog-create").dialog("close");
        return false;
    });

    $(".btnReport").live("click", function (e) {
        url = $(this).attr('data-url');
        $(".dialog-report").dialog('open');
        return false;
    });
    $(".loading").fadeOut(200).hide("slow")
}
