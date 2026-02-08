$(function () {
    //valiDation();
    //InitDatePickers();
    //FormatDate();
    InitAccordions();
    //InitTabs();

    //InitButtons();
    InitDropDowns();
    //InitCustomDialog();

    //InitDialogs();
    //InitThemes();
    InitRequired();
    //InitHelp();
    InitDateTimePickers();
    InitDatePickerRange();
    InitDateTimePickersChangeable();
    DatePickerEdit();
    //sessionDateTime()
    NumberFormatCheck();
    NumberFormatCheckText();

})
$(document).ready(function () {
    $(":disabled, [readonly=readonly]").css("border-color", "orange").attr("title", "This field is readonly");
    //$(":disabled, [readonly=readonly]").removeClass("required");
});

function goBack() {
    window.history.back();
}

//////$("textarea").each(function () {
//////    textAreaAdjust(this);
//////});

//////function textAreaAdjust(o) {
//////    o.style.height = "1px";
//////    o.style.height = (25 + o.scrollHeight) + "px";
//////}

$(function () {
    $(".hasDatepicker").datepicker({
        changeMonth: true,
        changeYear: true

    });
    $(".date_range_filter").datepicker({
        changeMonth: true,
        changeYear: true
    });
});



//////////$("img").on("error", function () {
//////////    $(this).attr('src', '~/Files/EmployeeInfo/0-mini.jpg');
//////////});

function MyCheckBox(senderValue, targetId, outputId) {
    if (senderValue == "True") {
        $('#' + targetId).attr('checked', true);
    }

    $('#' + targetId).checkboxpicker({
        html: true,
        offLabel: '<span class="glyphicon glyphicon-remove NotActive">',
        onLabel: '<span class="glyphicon glyphicon-ok IsActive">'
    });

    $('.NotActive').parent().click(function () {
        $('#' + outputId).val("False");
    });
    $('.IsActive').parent().click(function () {
        $('#' + outputId).val("True");

    });
}


function InitAccordions() {
    $(".Accordion").accordion({
        collapsible: true, active: true
    });
    $(".OpenAccordion").accordion({
        collapsible: false, active: 0, heightStyle: "content"
    });
    $(".CloseAccordion").accordion({
        collapsible: true, active: false, heightStyle: "content"
    });
}
function tablelength() {
    var len = [[5, 10, 25, 50, 100, 150, 200, 500 - 1], [5, 10, 25, 50, 100, 150, 200, 500, "All"]];
    return len;
}
function CodeCheck(sender, T) {
    if ($(sender).val() == "") {
        return;
    }
    $(sender).parent().find('.NotMatch').remove();
    var url = "/Common/Common/EnumAlreadyExist?tableName=" + T + "&fieldName=Name&value=" + $(sender).val();
    $.ajax({
        type: "GET",
        url: url,
        error: function (xhr, status, error) {
            //"test"
        },
        success: function (response) {
            if (!response) {
                $(sender).val('');
                if (!$(sender).parent().find('.NotMatch').hasClass('NotMatch')) {
                    $(sender).parent().append('<p class="NotMatch" style="color:red;" >Value is not Matched</p>');
                }
            }
        }
    });
}
function customToollip() {
    //$('.country').attr('title', 'write here or Double click for sample');
    $(' .department,.designation,.section,.country, .division,.district, .bloodGroup, .degree, .employmentStatus, .employmentType, .gender, .immigrationType, .leaveApproveStatus, .leaveType, .leftType, .meritalStatus, .nationality, .religion, .salaryPayMode, .salutation, .skillQuality, .trainingCourse, .trainingPlace, .trainingStatus, .travelType, .languageE, .languageCompetency, .languageFluency').attr('title', 'write here or Double click for sample');
    $(' .department,.designation,.section,.country, .division,.district, .bloodGroup, .degree, .employmentStatus, .employmentType, .gender, .immigrationType, .leaveApproveStatus, .leaveType, .leftType, .meritalStatus, .nationality, .religion, .salaryPayMode, .salutation, .skillQuality, .trainingCourse, .trainingPlace, .trainingStatus, .travelType, .languageE, .languageCompetency, .languageFluency').attr('placeholder', 'write here');
    $('.customDatePicker,.customTimePicker').attr('placeholder', 'click here');
}

function DatePickerEdit() {
    $(".cDatePickerEdit").datepicker({
        changeYear: true, changeMonth: true, yearRange: '-90:+9', maxYear: '100',
        dateFormat: 'dd-M-yy'

    });
}


function InitDateTimePickers() {
    $('.customTimePicker')//.prop("readonly", "readonly");
    $('.customDatePicker').prop("readonly", "readonly");
    $(".customDatePicker").datepicker({
        changeYear: true, changeMonth: true, yearRange: '-90:+9', maxYear: '100',
        dateFormat: 'dd-M-yy'

    });
}
function InitDatePickerRange() {

    var currentDate = "";

    //$.ajax({
    //    url: '/Sage/Home/SelectServerDateTime',
    //    type: 'GET',
    //    cache: false,

    //    success: function (result) {

            currentDate = Date.now();
            //currentDate = result;

            var newDate = new Date(currentDate);
            var rMin = new Date(newDate.getFullYear(), newDate.getMonth(), newDate.getDate()); // Min Date = Selected - 365d
            var rMax = new Date(newDate.getFullYear(), newDate.getMonth(), newDate.getDate() + 7); // Max Date = Selected + 7d
            $(".datepickerRange").datepicker({
                minDate: rMin
                , maxDate: rMax
                ,dateFormat: 'dd-M-yy'
                , yearRange: '-90:+9'

            });

    //    }
    //});
    $('.datepickerRange').prop("readonly", "readonly");





    //$(".datepickerRange").datepicker({ minDate: -20, maxDate: "+1M +10D" });


    ////var date = $(".datepickerRange").val();
    ////var today = FormatDate(new Date());
    ////if (date == "") {
    ////    $(".datepickerRange").val(today);
    ////}

}

function InitDateTimePickersChangeable() {
    $('.customTimePicker')//.prop("readonly", "readonly");
    //$('.customDatePicker').prop("readonly", "readonly");



    $(".customDatePickerChangeable").datepicker({
        changeYear: true, changeMonth: true, yearRange: '-90:+9', maxYear: '100',
        dateFormat: 'dd-M-yy'
       , minDate: -7, maxDate: 7

    });
}
function InitDateTimePickersCurrent() {
    //alert(1);
    $('.customTimePicker')//.prop("readonly", "readonly");
    $('.customDatePicker').prop("readonly", "readonly");
    $(".customDatePicker").datepicker({
        //changeYear: true, changeMonth: true, yearRange: '-90:+9', maxYear: '100',
        dateFormat: 'dd-M-yy',
        //minDate: new Date()

    });

    var date = $(".customDatePicker").val();
    //var today =new Date();// Session["SessionDate"];
    var today = FormatDate(new Date());
    //alert(today);
    if (date == "") {
        $(".customDatePicker").val(today);
    }
}

function InitDatePickers() {
    //var dt = FaizaSMS.WebUI.ViewModels.IdentityViewModel.SessionDate
    $(".DatePicker").prop("readonly", "readonly");
    $(".DatePicker:not(.OldDates)").datepicker({ changeYear: true, changeMonth: true, dateFormat: 'dd-M-yy', yearRange: '-100:+1' });
    $(".DatePicker.OldDates").datepicker({ changeYear: true, changeMonth: true, dateFormat: 'dd-M-yy', yearRange: '-100:+0', maxDate: '0', maxYear: '100' });
}

function FormatDate(input) {
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var dt = new Date(input);
    return [dt.getDate(), months[dt.getMonth()], dt.getFullYear()].join('-');
}
function ParseDate(input) {
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var parts = input.split('-');
    return new Date(parts[2], months.indexOf(parts[1]), parts[0]);
}

//function valiDation(sender) {
//    if (!$($("#frmEmployeeCreate .required")[0]).next('.RequiredField')) {

//    }
//    $($("#frmEmployeeCreate .required")[0]).next(':not(.RequiredField)')
//    $("#" + sender + " .required").parent().append('<p class="RequiredField" style="color:red;" >Value is Required</p>');
//    $("#" + sender + " .RequiredField").hide();
//    // $("#" + sender + ".required").change(function () {
//    $(".required").change(function () {
//        if ($(this).val() == "") {
//            $(this).parent().find('.RequiredField').show();
//        }
//        else {
//            $(this).parent().find('.RequiredField').hide('slow');
//        }
//    });
//}
function valiDation(sender) {
    //$("textarea").attr('maxlength', '200');
    //$("input:text").attr('data-val', 'number');
    //$(sender).attr('data-val', 'number');

    $("#" + sender + " .required").parent().append('<p class="RequiredField" style="color:red;" >Value is Required</p>');
    $("#" + sender + " .RequiredField").hide();


    $(".required").change(function () {
        //var t = $(".required :selected").text();

        if ($(this).val() == "") {

            $(this).parent().find('.RequiredField').show();
        }
        else if ($(sender).attr('data-val-number')) {
            if (!parseInt($(this).val())) {
                $(this).parent().find('.RequiredField').show();
            }
            else {
                $(this).parent().find('.RequiredField').hide('slow');
            }
        }
        else {

            $(this).parent().find('.RequiredField').hide('slow');
        }
    });
    $(".NumberCheck").change(function () {
        var val = $(this).val().replace(/\./g, '');
        val = val.replace(/\,/g, '');
        if (isNaN(val)) {
            $(this).val(0);
            ShowResult("Fail", "Only numeric allowed!");
        }
    });
}
function pageSubmit(sender) {
    var a = 0;
    
    for (var i = 0; i < $('#' + sender + ' .required').length; i++) {
        if ($($('#' + sender + ' .required')[i]).val() == "") {
            $($('#' + sender + ' .required')[i]).parent().find('.RequiredField').show();
            a++;
        }
    }
    if (a == 0) {
        debugger;
        //$("#" + sender).submit();
        $(".loading").show();
        var ab = $("#" + sender).submit();
        if (ab == "") {

            setTimeout(function () {
                $(".loading").fadeOut(500).hide("slow")
            }, 10000);


        }
        else {
            setTimeout(function () {
                $(".loading").fadeOut(500).hide("slow")
            }, 10000);
        }

    }
}

function BackToList(sender) {
    var url = $(sender).attr('data-url');
    window.location = url;
}
function NumberCheck(sender) {
    var val = $(sender).val().replace(/\./g, '');
    val = val.replace(/\,/g, '');
    if (isNaN(val)) {
        $(sender).val(0);
        ShowResult("Fail", "Only numeric allowed!");
    }
}

$(".TwoDecimal").each(function () {
    this.value = parseFloat(this.value).toFixed(2);
});

$(".TwoDecimalText").each(function () {
    this.innerText = parseFloat(this.innerText).toFixed(2);
});

$(".NoDecimal").each(function () {
    this.value = parseFloat(this.value).toFixed(0);
});
$(".NumberCheckAddDetail").change(function () {
    var val = $(this).val();


    val = val.replace(/\,/g, '');

    if (isNaN(val)) {
        $(this).val(0);
        ShowResult("Fail", "Only numeric allowed!");
    }
    else {
        val = Number(parseFloat(val).toFixed(2)).toLocaleString('en');
        $(this).val(val);
    }
});

$(".NumberCheck").change(function () {
    var val = $(this).val();
    val = val.replace(/\./g, '');
    val = val.replace(/\,/g, '');
    val = val.replace(/\s/g, '');
    if (isNaN(val)) {
        $(this).val(0);
        ShowResult("Fail", "Only numeric allowed!");
    }
});


function TimeFormat(sender) {

    //var time = $(sender).val();
    ////var regexp = new RegExp("/^\d+\.(?:[0-5]\d)$");
    //var regexp = new RegExp("/^\d+\.\d$");
    ////alert("m: " + $(sender).val().match("/^\d+\.(?:[0-5]\d)$"));

    ////alert(regexp);

    ////alert(time.search(regexp));

    ////var correct = (time.search(regexp) >= 0) ? true : false;
    //var correct = regexp.test(time);

    //alert(correct);

    //if (correct == false) {
    //    $(sender).val("0.00")
    //    ShowResult("Fail", time + " Invalid Time Format!");
    //}
}


$(".TimeFormat").change(function () {
    var regexp = "/^\d+\.(?:[0-5]\d)$";
    var correct = ($(this).val().search(regexp) >= 0) ? true : false;

    if (correct == false) {
        ShowResult("Fail", $(this).val() + " Invalid Time Format!");
    }
});

function pageSubmitJSON(sender) {
    var a = 0;
    for (var i = 0; i < $('#' + sender + ' .required').length; i++) {
        if ($($('#' + sender + ' .required')[i]).val() == "") {
            $($('#' + sender + ' .required')[i]).parent().find('.RequiredField').show();
            a++;
        }
    }
    return a;
}
function NumberFormatCheck() {
    $(".NumberCheck").each(function () {
        var val = ($(this).val()).replace(/\,/g, '');
        if (val != "") {
            val = Number(parseFloat(val).toFixed(2)).toLocaleString('en');
            $(this).val(val);
        }
    });
}

function NumberFormatCheckText() {
    $(".NumberCheck").each(function () {
        var val = ($(this).text()).replace(/\,/g, '');
        if (val != "") {
            val = Number(parseFloat(val).toFixed(2)).toLocaleString('en');
            $(this).text(val);
        }
    });
}

function Division() {
    $("select.country").change(function () {
        var a = $(this);
        $(a).closest('div').parent().closest('div').parent('div').find('.district').html("<option></option>")
        var dropdownElements = a.closest('div').parent().next().find('.division');
        var url = a.closest('div').parent().next().find('.division').attr('data-url') + "?country=" + $(this).val();
        $.getJSON(url, function (item, textStatus, jqXHR) {
            var Listitems = '<option value="">Select</option>';
            $.each(item, function (i, data) {
                if (true) {

                }
                Listitems += "<option value='" + data.Value + "'>" + data.Text + "</option>";
            });
            dropdownElements.html(Listitems).addClass("DropdownInited");
        });
    });
}
function District() {
    $("select.division").change(function () {
        var a = $(this);
        var dropdownElements = a.closest('div').parent().next().find('.district');
        var url = a.closest('div').parent().next().find('.district').attr('data-url') + "?division=" + $(this).val();
        $.getJSON(url, function (item, textStatus, jqXHR) {
            var Listitems = '<option value="">Select</option>';
            $.each(item, function (i, data) {
                if (true) {

                }
                Listitems += "<option value='" + data.Value + "'>" + data.Text + "</option>";
            });
            dropdownElements.html(Listitems).addClass("DropdownInited");
        });
    });
}
function Country() {
    var county = $("select.country");
    $.each(county, function (a, b) {
        var selectedValue = $(b).attr('data-val');
        var url = $(b).attr('data-url');
        $.getJSON(url, function (item, textStatus, jqXHR) {
            var Listitems = '<option value="">Select</option>';
            $.each(item, function (i, data) {
                if (selectedValue == data.Value) {
                    Listitems += "<option selected='selected' value='" + data.Value + "'>" + data.Text + "</option>";
                    //
                    var selectedValue2 = $(b).closest('div').parent().next().find('.division').attr('data-val');
                    var url2 = $(b).closest('div').parent().next().find('.division').attr('data-url') + "?country=" + selectedValue;
                    $.getJSON(url2, function (item2, textStatus, jqXHR) {
                        var dListitems = '<option value="">Select</option>';
                        $.each(item2, function (i, division) {
                            if (selectedValue2 == division.Value) {
                                dListitems += "<option selected='selected' value='" + division.Value + "'>" + division.Text + "</option>";
                                //
                                var selectedValue3 = $(b).closest('div').parent().closest('div').parent('div').find('.district').attr('data-val');
                                var url3 = $(b).closest('div').parent().closest('div').parent('div').find('.district').attr('data-url') + "?division=" + selectedValue2;
                                $.getJSON(url3, function (item3, textStatus, jqXHR) {
                                    var dListitem3 = '<option value="">Select</option>';
                                    $.each(item3, function (i, disct) {
                                        if (selectedValue3 == disct.Value) {
                                            dListitem3 += "<option selected='selected' value='" + disct.Value + "'>" + disct.Text + "</option>";

                                        }
                                        else {
                                            dListitem3 += "<option value='" + disct.Value + "'>" + disct.Text + "</option>";
                                        }
                                    });
                                    $(b).closest('div').parent().closest('div').parent('div').find('.district').html(dListitem3).addClass("DropdownInited");
                                });
                                //
                            }
                            else {
                                dListitems += "<option value='" + division.Value + "'>" + division.Text + "</option>";
                            }
                        });
                        $(b).closest('div').parent().next().find('.division').html(dListitems).addClass("DropdownInited");
                    });
                }
                else {
                    Listitems += "<option value='" + data.Value + "'>" + data.Text + "</option>";
                }
            });
            $(b).html(Listitems).addClass("DropdownInited");
        });
    });
}

function InitDropDowns() {
    var dropdownElements = $('select.Dropdown:not(.DropdownInited)');
    $.each(dropdownElements, function (index, element) {
        var dropdownEl = $(element);
        var url = dropdownEl.attr('data-url');
        if (!url) {
            alert("no url");
            return;
        }

        var selected = dropdownEl.attr('data-selected');
        var dataCache = dropdownEl.attr('data-cache') ? true : false;

        $.ajax({
            url: url,
            type: 'GET',
            cache: dataCache,
            success: function (jsonData, textStatus, XMLHttpRequest) {
                var Listitems = '<option value="">Select</option>';

                $.each(jsonData, function (i, item) {

                    if (selected && selected == item.Value) {
                        Listitems += "<option selected='selected' value='" + item.Value + "'>" + item.Text + "</option>";
                    }
                    else {
                        Listitems += "<option value='" + item.Value + "'>" + item.Text + "</option>";
                    }
                });
                dropdownEl.html(Listitems).addClass("DropdownInited");
            }
        });
    });
}

function InitCascadingDropDowns() {
    var dependentElements = $('select.Cascading:not(.DropdownInited)');
    $.each(dependentElements, function (index, element) {
        var dependentEl = $(element);
        var parentEl = $('#' + dependentEl.attr('data-parent')); // element.dataset.parent;
        var url = dependentEl.attr('data-url');
        var selected = dependentEl.attr('data-selected');
        var dataCache = dependentEl.attr('data-cache') ? true : false;
        var loadDropDownItems = function () {
            if (!parentEl.val()) {
                if (selected) {
                    setTimeout(loadDropDownItems, 300);
                }
                return;
            }
            $.ajax({
                url: url + parentEl.val(),
                type: 'GET',
                cache: dataCache,
                success: function (jsonData, textStatus, XMLHttpRequest) {
                    var Listitems = '<option></option>';
                    $.each(jsonData, function (i, item) {
                        if (selected && selected == item.Value) {
                            Listitems += "<option selected='selected' value='" + item.Value + "'>" + item.Text + "</option>";
                        }
                        else {
                            Listitems += "<option value='" + item.Value + "'>" + item.Text + "</option>";
                        }
                    });
                    dependentEl.html(Listitems).addClass("DropdownInited");
                }
            });
        };
        parentEl.change(loadDropDownItems);
        if (selected) {
            loadDropDownItems();
        }
    });
}
function fnUpdate(submitBtn) {
    var $form = $(submitBtn).parents('form');
    var tt = "";
    $.ajax({
        type: "POST",
        url: $form.attr('action'),
        data: $form.serialize(),
        error: function (xhr, status, error) {
            "test"
        },
        success: function (response) {
            // $(".dialog-alert").dialog('open');
            $("#dialog-msg").append('' + response[1]);
            $(".ui-dialog").addClass('' + response[0]);

        }
    });
    return false;// if it's a link to prevent post
}
function InitRequired() {
    //$("[data-val-required]").css({
    //    $(".required").css({
    //    borderRight: "4px solid #f20"
    //});
}
var url = "";
function fromWriteColor(value) {
    $("." + value + " input:text,." + value + " textarea,." + value + " input:file,." + value + " select").addClass("wrightColor");
}
function fromReadColor(value) {
    $("." + value + " input:text,." + value + " textarea,." + value + " input:file,." + value + " select").addClass("readOnlyColor");
}
function fnReadOnly(value) {
    if ($("." + value).hasClass("readOnly")) {
        $("." + value).removeClass("readOnly");
        $("." + value + " input:text,." + value + " textarea").attr("readOnly", false);
        $("." + value + " input:file,." + value + " select,." + value + "  input:Checkbox,." + value + " .customDatePicker,." + value + " .customTimePicker").attr("disabled", false);
        $("." + value + " input:text,." + value + " textarea,." + value + " input:file,." + value + " select").addClass("wrightColor");
        $("." + value + " input:text,." + value + " textarea,." + value + " input:file,." + value + " select").removeClass("readOnlyColor");
        //$("." + value).attr("readOnly", false);
        //$("." + value).addClass("wrightColor");
        //$("." + value).removeClass("readOnlyColor");
        $("." + value + " .customTimePicker").attr('style', "cursor:pointer !important");

    }
    else {
        $("." + value + " input:file,." + value + " select,." + value + " input:Checkbox,." + value + " .customDatePicker,." + value + " .customTimePicker").attr("disabled", true);
        $("." + value).addClass("readOnly");
        $("." + value + " input:text,." + value + " textarea").attr("readOnly", true)
        $("." + value + " input:text,." + value + " textarea,." + value + " input:file,." + value + " select").removeClass("wrightColor");
        $("." + value + " input:text,." + value + " textarea,." + value + " input:file,." + value + " select").addClass("readOnlyColor");
        $("." + value + " .customTimePicker").attr('style', "cursor:not-allowed !important");
    }
    $('.customTimePicker').prop("readonly", "readonly");
    $('.customDatePicker').prop("readonly", "readonly");

}

function SelectAllForDelete() {
    $(".chkAll").on("click", function () {
        if ($(this).is(":checked")) {
            $(this).closest("table").find("tbody input:Checkbox").attr("checked", true);
        }
        else {
            $(this).closest("table").find("tbody input:Checkbox").attr("checked", false);
        }
    });
    $(".paginate_enabled_next,.paginate_enabled_previous,.paginate_disabled_next,.paginate_disabled_previous").on("click", function () {
        $(this).parents().find("table th .chkAll").attr("checked", false);
    });
}


function deletedOne(sender) {
    var url = $(sender).attr("data-url") + "~";
    //alert(url);
    Ask("Are you sure to Delete!", function () {
        $.getJSON(url, function (item, textStatus, jqXHR) {
            if (result.indexOf("~") > -1) {
                ShowResult(result.split("~")[0], result.split("~")[1]);

                if (result.split("~")[0] == "Fail") {
                    return;
                }

            }
            else {
                ShowResult("Success", result);
            }

            setTimeout(function () {
                location.reload();
            }, 2000);
        });
    }, function () { });
}

function Synchronize(sender) {
    var url = $(sender).attr("data-url");

    Ask("Are you sure to Synchronize!", function () {
        $(".loading").show();

        $.getJSON(url, function (result, textStatus, jqXHR) {

            if (result.indexOf("~") > -1) {
                ShowResult(result.split("~")[0], result.split("~")[1]);

                if (result.split("~")[0] == "Fail") {
                    return;
                }

            }
            else {
                ShowResult("Success", result);
            }

            $(".loading").fadeOut(200).hide("slow")

            setTimeout(function () {
                location.reload();
            }, 2000);
        });
    }, function () { })
}


function deletedData(sender, checkboxId, id) {
    var deletedIds = "";
    if (typeof id === 'undefined') {
        var length = $("#" + checkboxId + " tbody input:checkbox").length;
        for (var i = 0; i < length; i++) {
            if ($($("#" + checkboxId + " tbody input:checkbox")[i]).is(":checked")) {
                deletedIds += $($("#" + checkboxId + " tbody input:checkbox")[i]).attr("data-Id") + "~";
            }
        }
    }
    else {
        deletedIds = id + "~";
    }
    debugger;


    var url = $(sender).attr("data-url") + "?ids=" + deletedIds;
    if (deletedIds == "") {
        ShowResult("Fail", "Select first to Delete!");
        return;
    }
    Ask("Are you sure to Delete!", function () {
        
        $.getJSON(url, function (result, textStatus, jqXHR) {

            debugger;

            if (result.indexOf("~") > -1) {
                ShowResult(result.split("~")[0], result.split("~")[1]);
                debugger;
                if (result.split("~")[0] == "Fail") {
                    return;
                }

            }
            else {
                debugger;
                ShowResult("Success", result);
            }

            setTimeout(function () {
                debugger;
                location.reload();
            }, 2000);
        });
    }, function () { })
}
function journalData(sender, checkboxId, id) {
    var deletedIds = "";
    if (typeof id === 'undefined') {
        var length = $("#" + checkboxId + " tbody input:checkbox").length;
        for (var i = 0; i < length; i++) {
            if ($($("#" + checkboxId + " tbody input:checkbox")[i]).is(":checked")) {
                deletedIds += $($("#" + checkboxId + " tbody input:checkbox")[i]).attr("data-Id") + "~";
            }
        }
    }
    else {
        deletedIds = id + "~";
    }

    if (deletedIds == "") {
        ShowResult("Fail", "Select first to Journal!");
        return;
    }
    var tType = $(sender).attr("data-tType");
    var date = $(sender).attr("data-date");

    var url = $(sender).attr("data-url") + "?ids=" + deletedIds + "&tType=" + tType + "&date=" + date;

    Ask("Are you sure to Journal!", function () {
        $.getJSON(url, function (result, textStatus, jqXHR) {
            var FinalLocation = result;
            if (result.indexOf("~") > -1) {
                //////////ShowResult(result.split("~")[0], result.split("~")[1]);
                FinalLocation = result.split("~")[1];
                if (result.split("~")[0] == "Fail") {
                    ShowResult("Fail", "Journal Not Created!");
                    return;
                }
                else {
                    ShowResult("Success", "Journal Succesfully Created!");
                }

            }
            else {
                ShowResult("Success", "Journal Succesfully Created!");
            }
            setTimeout(function () {
                window.location = FinalLocation;
                ////////location.reload();
            }, 2000);
        });
    }, function () { })
}

function postedData(sender, checkboxId, id) {
    var postedIds = "";
    if (typeof id === 'undefined') {
        var length = $("#" + checkboxId + " tbody input:checkbox").length;
        for (var i = 0; i < length; i++) {
            if ($($("#" + checkboxId + " tbody input:checkbox")[i]).is(":checked")) {
                postedIds += $($("#" + checkboxId + " tbody input:checkbox")[i]).attr("data-Id") + "~";
            }
        }
    }
    else {
        postedIds = id + "~";
    }

    var url = $(sender).attr("data-url") + "?ids=" + postedIds;
    if (postedIds == "") {
        ShowResult("Fail", "Select First to Post!");
        return;
    }
    Ask("Are you sure to Post!", function () {
        $.getJSON(url, function (result, textStatus, jqXHR) {
            debugger;
            //if (result.indexOf("~") > -1) {
            //    ShowResult(result.split("~")[0], result.split("~")[1]);
            //    if (result.split("~")[0] == "Fail") {        
            //        return;
            //    }
            //}

            if (result[0]  == "Fail") {
                ShowResult("Fail", "Data already Posted");
            }

            else {
                ShowResult("Success", result);
            }

            setTimeout(function () {
                location.reload();
            }, 2000);
        });
    }, function () { })
}

function AuditData(sender, checkboxId, id) {
    operationMultiData(sender, checkboxId, "Audit", id);
}

function ApproveData(sender, checkboxId, id) {
    operationMultiData(sender, checkboxId, "Approve", id);
}

function RejectData(sender, checkboxId, id) {
    operationMultiData(sender, checkboxId, "Reject", id);
}

function ReceiveData(sender, checkboxId, id) {
    operationMultiData(sender, checkboxId, "Receive", id);
}


//function ReportData(sender, checkboxId, id) {
//    operationMultiData(sender, checkboxId, "Report", id);
//}

function operationMultiData(sender, checkboxId, operationName, id) {
    var ids = "";
    if (typeof id === 'undefined') {
        var length = $("#" + checkboxId + " tbody input:checkbox").length;
        for (var i = 0; i < length; i++) {
            if ($($("#" + checkboxId + " tbody input:checkbox")[i]).is(":checked")) {
                ids += $($("#" + checkboxId + " tbody input:checkbox")[i]).attr("data-Id") + "~";
            }
        }
    }
    else {
        ids = id + "~";
    }

    var url = $(sender).attr("data-url") + "?ids=" + ids;
    if (ids == "") {
        ShowResult("Fail", "Select First to " + operationName + "!");
        return;
    }
    Ask("Are you sure to " + operationName + "!", function () {
        $.getJSON(url, function (result, textStatus, jqXHR) {
            if (result.indexOf("~") > -1) {
                ShowResult(result.split("~")[0], result.split("~")[1]);

                if (result.split("~")[0] == "Fail") {
                    return;
                }

            }
            else {
                ShowResult("Success", result);
            }
            setTimeout(function () {
                location.reload();
            }, 2000);
        });
    }, function () { })
}

function NewTab(sender) {
    var url = $(sender).attr("data-url");
    var win = window.open(url, '_blank');
}

function GoTo(sender) {

    window.location = $(sender).attr("data-url");
}
function CheckAll(sender) {

    var unchecked = $(sender).closest("tbody").find('input:checkbox:not(":checked")').length;
    if (unchecked == 0) {
        $(sender).closest("table").find(" thead input:checkbox").attr('checked', true);
    }
    else {
        $(sender).closest("table").find(" thead input:checkbox").attr('checked', false);
    }
}
function CheckPromotionDate(sender) {
    var url = "/HRM/Home/CheckPromotionDate?employeeId=" + $("#promotionVM_EmployeeId").val() + "&date=" + $(sender).val();
    $.ajax({
        type: "GET",
        url: url,
        error: function (xhr, status, error) {
            //"test"
        },
        success: function (response) {
            if (!response) {
                $(sender).val('');
                ShowResult("Fail", "Promotion date can't be perior to join date/  last promotion date");
            }
        }
    });
}
function CheckTransferDate(sender) {
    var url = "/HRM/Home/CheckTransferDate?employeeId=" + $("#transferVM_EmployeeId").val() + "&date=" + $(sender).val();
    $.ajax({
        type: "GET",
        url: url,
        error: function (xhr, status, error) {
            //"test"
        },
        success: function (response) {
            if (!response) {
                $(sender).val('');
                ShowResult("Fail", "Transfer date can't be perior to join date/  last transfer  date");
            }
        }
    });
}
function ShowResult(status, msg, dataAction, dataUrl) {
    if (status == "Success") {
        toastr.success(msg, 'Shampan ERP')
    }
    else if (status == "Fail") {
        toastr.error(msg, 'Shampan ERP')
    }
    else if (status == "Info") {
        toastr.info(msg, 'Shampan ERP')
    }
    else if (status == "Warning") {
        toastr.warning(msg, 'Shampan ERP')
    }
    else {
        toastr.info(status, 'Shampan ERP')
    }
}

function ShowResultProcess(status, msg, dataAction, dataUrl) {

    var html = '' +
    '<div class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">' +
        '<div class="modal-dialog" style="margin-top:150px;width:400px;">' +
            '<div class="modal-content">' +
                '<div class="modal-header">' +
                    '<button type="button" class="close"  onclick="CloseModal(this)" aria-hidden="true">&times;</button>' +
                    '<h2 class="modal-title" id="myModalLabel">Shampan ERP</h2>' +
                 '</div>' +
                 '<div class="modal-body ' + status + '">' +
                    msg +
                 '</div>' +
                 '<div class="modal-footer">' +
                    '<input type="button" value="Ok" class="btn btn-info " onclick="CloseModal(this,\'' + dataAction + '\',\'' + dataUrl + '\')"/>' +
                '</div>' +
            '</div>' +
        '</div>' +
    '</div>';

    var dialogWindow = $(html).appendTo('body');
    dialogWindow.modal({ backdrop: 'static' });
}

function CloseModal(el, dataAction, dataUrl) {
    //if (dataAction == "formsubmit") {
    //    $("#" + dataUrl).submit();
    //}
    //else if (dataAction == "refreshparent") {
    //    window.parent.location = window.parent.location;
    //}
    //else if (dataAction == "refreshself") {
    //    window.location = window.location;
    //}
    //else if (dataAction == "redirect") {
    //    window.location = dataUrl;
    //}
    //else if (dataAction == "function") {
    //    eval(dataUrl);
    //}

    if (dataAction == "refreshparent") {
        window.parent.location = window.parent.location;
    }
    else if (dataAction == "redirect") {
        window.location = dataUrl;
    }
    var win = $(el).closest(".modal");
    win.modal("hide");
    setTimeout(function () {
        win.next(".modal-backdrop").remove();
        win.remove();
        $("body").removeClass("modal-open");
    }, 500);

}
function Ask(msg, yesCallback, noCallback) {
    var html = '' +
    '<div class="modal fade ask" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">' +
        '<div class="modal-dialog" style="margin-top:150px;width:400px;">' +
            '<div class="modal-content">' +
                '<div class="modal-header">' +
                    '<button type="button" class="close" onclick="CloseModal(this);" aria-hidden="true">&times;</button>' +
                    '<h2 class="modal-title" id="myModalLabel">Shampan ERP</h2>' +
                 '</div>' +
                 '<div class="modal-body Success">' +
                    msg +
                 '</div>' +
                 '<div class="modal-footer">' +
                  '<input type="button" id="btnAskYes" value="Yes" class="btn btn-success" />' +
                 '<input type="button" id="btnAskNo" value="No" class="btn btn-success" />' +
                   
                    
                '</div>' +
            '</div>' +
        '</div>' +
    '</div>';

    var dialogWindow = $(html).appendTo('body');
    dialogWindow.modal({ backdrop: 'static' });

    $("#btnAskYes").on("click", function () {
        var win = $(this).closest(".modal");
        win.modal("hide");
        setTimeout(function () {
            win.next(".modal-backdrop").remove();
            win.remove();
            $("body").removeClass("modal-open");
        }, 500);
        setTimeout(yesCallback, 600);
    });
    $("#btnAskNo").on("click", function () {
        var win = $(this).closest(".modal");
        win.modal("hide");
        setTimeout(function () {
            win.next(".modal-backdrop").remove();
            win.remove();
            $("body").removeClass("modal-open");
        }, 500);
        setTimeout(noCallback, 600);
    });
}
function InitButtons() {
    $('.AddRow:not(.AddRowInited)').on("click", function () {
        var url = $(this).attr('data-url');
        var container = $(this).attr('data-container');
        $.ajax({
            url: url,
            type: 'POST',
            cache: false,
            success: function (html) {
                $("#" + container).append(html);
            }
        });
        return false;
    }).addClass("AddRowInited");

    $('.RemoveRow:not(.RemoveRowInited)').on("click", function () {
        $(this).parents("div.row:first").remove();
        return false;
    }).addClass("RemoveRowInited");
}

function CheckImageSize(sender) {
    if (sender.files[0].size > (1024 * 100)) {
        ShowResult('Fail', 'Image size can be maximum 100kb');
        $(sender).val('');
    }
}
function CheckFileSize(sender) {
    if (sender.files[0].size > (1024 * 512)) {
        ShowResult('Fail', 'File size can be maximum 512kb');
        $(sender).val('');
    }
}
function FileDelete(sender) {
    Ask("Are you sure to delete this file!", function () {
        var url = "/Config/DropDown/FileDelete?filepath=" + $(sender).attr('data-url') + "&table=" + $(sender).attr('data-table') + "&field=" + $(sender).attr('data-field') + "&id=" + $(sender).attr('data-id');
        $.ajax({
            type: "GET",
            url: url,
            error: function (xhr, status, error) {
                //"test"
            },
            success: function (response) {
                if (response) {
                    $(sender).closest("div").parent().remove();
                    ShowResult("Success", "File deleted!");
                }
                else {
                    ShowResult("Fail", "File deleted failed!");
                }
            }
        })
    }, function () { })

}
//// salary structure start
function FixedCheck(sender) {
    if ($(sender).is(":checked")) {
        $(sender).closest('.row').find(".PortionSalaryTypeId option:selected").prop("selected", false);
        $(sender).closest('.row').find(".PortionSalaryTypeId").attr('disabled', true);
        $(sender).closest('.row').find("label.fr").text("Fixed")

    }
    else {
        $(sender).closest('.row').find(".PortionSalaryTypeId").attr('disabled', false);
        $(sender).closest('.row').find(".Portion").trigger('change');
        $(sender).closest('.row').find("label.fr").text("Rate(% of Basic)")

    }
}

function SalaryTypePortion(sender) {
    $(sender).closest('div').find('lable').hide();
    if (!isNaN($(sender).val())) {
        if (!$(sender).closest('.row').find(".IsFixed").is(":checked")) {
            if (parseFloat($(sender).val()) > 100) {
                $(sender).val("100.00");
            }
            else if (parseFloat($(sender).val()) < 0) {
                $(sender).val("0.00");
            }
        }

    }
    else {
        $(sender).val("0.00");
        $(sender).closest('div').append('<lable class="" style="color:red">Only numecir value is allowed!<lable>');

    }
}

function TaxPortion(sender) {
    if (!isNaN($(sender).val())) {

        //$(sender).parents("div.row:first").find("input.IsFixed");
        if (!$(sender).parents("div.first").find("input.IsFixed").is(":checked")) {
            //if (!$(sender).parents("div.row:first").find("input.IsFixed")) {
            alert("check");

            if (parseFloat($(sender).val()) > 100) {
                $(sender).val("100.00");
            }
            else if (parseFloat($(sender).val()) < 0) {
                $(sender).val("0.00");
            }
        }
        else {
            alert("not check");

        }
    }
    else {
        $(sender).val("0.00");
        $(sender).closest('div').append('<lable class="" style="color:red">Only numecir value is allowed!<lable>');

    }
}

function CheckNumeric(value) {
    var a = $(value).val();
    if (!$.isNumeric(a)) {
        $(value).val(0)
        ShowResult("Fail", "Please Enter Numeric data");

        return;
    }
}
$('.ccProject').click(function () {
    var departments = "";
    $('.department').html("");


    var oParam = { "projectId": "" };
    oParam.projectId = $('.ccProject').val();

    var url1 = "/Config/DropDown/DerparmentByProject";
    departments += "<option value='0'>Select</option>";

    $.getJSON(url1, oParam, function (data) {
        $.each(data, function (i, state) {
            departments += "<option value='" + state.value + "'>" + state.text + "</option>";
        });
        $('.ccDepartment').html(departments);
    });
});


////Date Comparison

function comparedate(first, second, firstName, secondName) {
    if (first > second) {
        ShowResult("Fail!", "" + firstName + " date can't be prior to " + secondName + " date");
        return false;
    }
}

function minmax(value, min, max) {
    $(value).attr({ type: "number", min: "0", step: "0.01" });
    var a = $(value).val();
    if (!$.isNumeric(a)) {
        $(value).val("")
        ShowResult("Fail", "Please Enter Numeric data");
        return;
    }
    else {
        if (parseInt(a) < min || isNaN(parseInt(a))) {
            ShowResult("Fail", "Please Enter more then")
            return;
        }
        else if (parseInt(a) > max) {
            ShowResult("Fail", "Please Enter greater then")
            return;
        }
        else return value;
    }
}
var submit = function (url, mydata) {
    var rrSuccess = false;
    $.ajax({
        type: 'POST',
        data: mydata, // #2
        url: url,
        beforeSend: function () { $(".loading").show(); },
        success: function (result) {
            rrSuccess = true;
            var msg1 = result.split('~')[0];
            var msg2 = result.split('~')[1];
            if (msg1 != "Fail") {

                ShowResult("Success", msg2);
                //location.reload();
                return rrSuccess;
            }
            else {
                ShowResult("Fail", msg2);
                return rrSuccess;
            }

        },
        complete: function () { $(".loading").fadeOut(200).hide("slow") }

    });
    return rrSuccess;

}


var submitProcess = function (url, mydata) {
    var rrSuccess = false;
    $.ajax({
        type: 'POST',
        data: mydata, // #2
        url: url,
        beforeSend: function () { $(".loading").show(); },
        success: function (result) {
            rrSuccess = true;
            var msg1 = result.split('~')[0];
            var msg2 = result.split('~')[1];
            if (msg1 != "Fail") {

                ShowResultProcess("Success", msg2);
                //location.reload();
                return rrSuccess;


            }
            else {
                ShowResultProcess("Fail", msg2);
                return rrSuccess;
            }

        },
        complete: function () { $(".loading").fadeOut(200).hide("slow") }

    });
    return rrSuccess;

}

$(document).keyup(function (e) {
    if (e.keyCode == 27) {
        $(".loading").fadeOut(200).hide("slow");
    }
});

function InitDropdownsCommon() {

    //alert(111);
    var fy = function () {
        $('select.fpDetailsCom').html("");
        var FiscalPeriodDetails = "";
        var fYear = $('.fiscalyearCom').val();

        var url = "/Config/DropDown/DropDownPeriodByFYear/?year=" + fYear;
        FiscalPeriodDetails += "<option value=''>Select</option>";

        $.getJSON(url, function (data) {
            $.each(data, function (i, state) {
                FiscalPeriodDetails += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            });
            $('select.fpDetailsCom').html(FiscalPeriodDetails);
        });
    }
    var fyTo = function () {
        $('select.fpDetailsTo').html("");
        var FiscalPeriodDetails = "";
        var fYear = $('.fiscalyearTo').val();

        var url = "/Config/DropDown/DropDownPeriodByFYear/?year=" + fYear;
        FiscalPeriodDetails += "<option value=''>Select</option>";

        $.getJSON(url, function (data) {
            $.each(data, function (i, state) {
                FiscalPeriodDetails += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            });
            $('select.fpDetailsTo').html(FiscalPeriodDetails);
        });
    }
    function fyscalYearDetailToLoad() {
        $('select.fpDetailsComTo').html("");
        var fId = "";
        var fId = $('select.fpDetailsCom').val();
        var url1 = "/Config/DropDown/DropDownPeriodNext/?currentId=" + fId;
        CodeT = "<option value=0>Select</option>";
        $.getJSON(url1, function (data) {
            $.each(data, function (i, state) {
                CodeT += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            });
            $('select.fpDetailsComTo').html(CodeT).change();;
        });
    }

    $('select.fpDetailsCom').click(function () {
        fyscalYearDetailToLoad();
    })
    //$('select.fpDetailsCom').change(function () {
    //    fyscalYearDetailToLoad();
    //})
    $('.fiscalyearCom').click(function () {
        fy();
    });

    $('.fiscalyearCom').change(function () {
        fy();
    });

    $('.fiscalyearTo').click(function () {
        fyTo();
    });

    $('.fiscalyearTo').change(function () {
        fyTo();
    });
    $('select.departmentsCom').click(function () {
        $('select.projectsCom').html("");
        $('select.sectionsCom').html("");
        var sections = "";
        var did = $('.departmentsCom').val();
        var url1 = "/Config/DropDown/SectionByDepartment/?departmentId=" + did;
        sections = "<option value=''>Select</option>";
        sections += "<option value=0_0>=ALL=</option>";
        $.getJSON(url1, function (data) {
            $.each(data, function (i, state) {
                sections += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            });
            $('select.sectionsCom').html(sections).change();;
        });
    });
    $('select.sectionsCom').click(function () {
        $('select.projectsCom').html("");
        var projects = "";
        var sid = $('select.sectionsCom').val();
        var did = $('select.departmentsCom').val();
        var url1 = "/Config/DropDown/ProjectByDepartment/?departmentId=" + did + "&sectionId=" + sid;
        projects = "<option value=''>Select</option>";
        projects += "<option value=0_0>=ALL=</option>";
        $.getJSON(url1, function (data) {
            $.each(data, function (i, state) {
                projects += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            });
            $('select.projectsCom').html(projects).change();;
        });
    });
    $('select.codeFCom').click(function () {
        $('select.codeTCom').html("");
        var CodeT = "";
        var CodeF = $('select.codeFCom').val();
        var url1 = "/Config/DropDown/EmployeeCodeNext/?currentCode=" + CodeF;
        CodeT = "<option value=''>Select</option>";
        CodeT += "<option value=0_0>=ALL=</option>";
        $.getJSON(url1, function (data) {
            $.each(data, function (i, state) {
                CodeT += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            });
            $('select.codeTCom').html(CodeT).change();;
        });
    })

}

$(window).scroll(function () {
    if ($(this).scrollTop() > 50) {
        $('#back-to-top').fadeIn();
    } else {
        $('#back-to-top').fadeOut();
    }
});
// scroll body to 0px on click
$('#back-to-top').click(function () {
    $('body,html').animate({
        scrollTop: 0
    }, 800);
    return false;
});

$("#downClick").on("click", function () {
    scrolled = scrolled + 300;

    $(".cover").animate({
        scrollTop: scrolled
    });

});


function btnReceive(sender) {
    var IdOperation = $(sender).attr('data-Id');
    var Id = IdOperation.split('~')[0];
    var Operation = IdOperation.split('~')[1];
    if (Operation.toLowerCase() == 'true') {
        ShowResult("Fail", "Data Already Received!");
        return;
    }
    var url = $(sender).attr("data-url") + "?ids=" + Id + '~';
    var questionMSG = "Are you sure to Receive Data!";
    singleOperation(questionMSG, url);
}
function btnApprove(sender) {
    var IdOperation = $(sender).attr('data-Id');
    var Id = IdOperation.split('~')[0];
    var Operation = IdOperation.split('~')[1];
    if (Operation.toLowerCase() == 'true') {
        ShowResult("Fail", "Data Already Approved!");
        return;
    }
    var url = $(sender).attr("data-url") + "?ids=" + Id + '~';
    var questionMSG = "Are you sure to Approve Data!";
    singleOperation(questionMSG, url);
}
function btnPost(sender) {
    var IdPost = $(sender).attr('data-Id');
    var Id = IdPost.split('~')[0];
    var Post = IdPost.split('~')[1];
    if (Post.toLowerCase() == 'true') {
        ShowResult("Fail", "Data Already Posted!");
        return;
    }
    var url = $(sender).attr("data-url") + "?ids=" + Id + '~';
    var questionMSG = "Are you sure to Post Data!";
    singleOperation(questionMSG, url);
}
function btnPostNoSplit(sender) {
    var Id = $(sender).attr('data-Id');
    var Post = $(sender).attr('data-Post');
    if (Post == "Posted") {
        ShowResult("Fail", "Data Already Posted!");
        return;
    }
    var url = $(sender).attr("data-url") + "?ids=" + Id + '~';
    var questionMSG = "Are you sure to Post Data!";
    singleOperation(questionMSG, url);
}

function btnDeleteNoSplit(sender) {
    var Id = $(sender).attr('data-Id');
    var Post = $(sender).attr('data-Post');

    if (Post == "Posted") {
        ShowResult("Fail", "Data Already Posted! Can't Be Deleted!");
        return;
    }
    var url = $(sender).attr("data-url") + "?ids=" + Id + '~';
    var questionMSG = "Are you sure to Delete Data!";
    singleOperation(questionMSG, url);
}


function btnDelete(sender) {
    var IdPost = $(sender).attr('data-Id');
    var Id = IdPost.split('~')[0];
    var Post = IdPost.split('~')[1];
    if (Post.toLowerCase() == 'true') {
        ShowResult("Fail", "Data Already Posted! Can't Be Deleted!");
        return;
    }
    var url = $(sender).attr("data-url") + "?ids=" + Id + '~';
    var questionMSG = "Are you sure to Delete Data!";
    singleOperation(questionMSG, url);
}
function singleOperation(questionMSG, url) {
    Ask(questionMSG, function () {
        $.ajax({
            url: url,
            type: 'Post',
            beforeSend: function () { $(".loading").show(); },
            complete: function () { $(".loading").fadeOut(200).hide("slow") },
            success: function (result) {
                debugger;

                //if (result.indexOf("~") > -1) {
                //    ShowResult(result.split("~")[0], result.split("~")[1]);
                //    if (result.split("~")[0] == "Fail") {
                //        return;
                //    }
                //}

                //NewAdd
                if (result[0] == "Fail") {
                    ShowResult("Fail", "Data already Posted");
                }

                else {
                    ShowResult("Success", result);
                }

                setTimeout(function () {
                    location.reload();
                }, 2000);
            }
        });
    });
}

function singlePostDlete(questionMSG, url) {
    Ask(questionMSG, function () {
        $.ajax({
            url: url,
            type: 'Post',
            beforeSend: function () { $(".loading").show(); },
            complete: function () { $(".loading").fadeOut(200).hide("slow") },
            success: function (result) {
                if (result.indexOf("~") > -1) {
                    ShowResult(result.split("~")[0], result.split("~")[1]);

                    if (result.split("~")[0] == "Fail") {
                        return;
                    }

                }
                else {
                    ShowResult("Success", result);
                }


                setTimeout(function () {
                    location.reload();
                }, 2000);

            }
        });
    });

}

function singleOperationGet(questionMSG, url) {
    Ask(questionMSG, function () {
        $.ajax({
            url: url,
            type: 'Get',
            beforeSend: function () { $(".loading").show(); },
            complete: function () { $(".loading").fadeOut(200).hide("slow") },
            success: function (result) {
                if (result.indexOf("~") > -1) {
                    ShowResult(result.split("~")[0], result.split("~")[1]);

                    if (result.split("~")[0] == "Fail") {
                        return;
                    }

                }
                else {
                    ShowResult("Success", result);
                }


                setTimeout(function () {
                    location.reload();
                }, 2000);
            }
        });
    });
}

function FiscalYearLockCheck(FiscalYearDetailId) {

    console.log("JS");
    console.log(FiscalYearDetailId);

    var url = "/Common/FiscalYear/FiscalPeriodLockCheck?FiscalYearDetailId=" + FiscalYearDetailId;

    var value = false;

    $.ajax({
        url: url

           , type: 'Post'
           , traditional: true
           , beforeSend: function () { $(".loading").show(); }
           , success: function (result) {
               var results = result.split("~");
               if (results[0].toLowerCase() == 'true') {

                   value = results[0];

                   console.log(value);

                   ShowResult("Fail", results[1]);
                   

               }

           }

    });

    return value;

}
$(document).ready(function () {
    $('form').on('click', 'input[value="0"], textarea.NumberCheck', function () {
        $(this).select();
    });
});


