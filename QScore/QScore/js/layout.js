var currentTab;
var blockerFrozen = false;
var blocker;

$(function () {
    $.datepicker.setDefaults($.datepicker.regional["mx"]);
    layoutElements();
    $("#dropdownmenu").sticky({ topSpacing: 0 });
    //$("#pageTitle").sticky({ topSpacing: 39 });
    $("#tabNav").tabs({
        selected: currentTab,
        select: function (event, ui) {
            var active = ui.index;
            currentTab = active;
        }
    });
    $('#loading').ajaxStart(function () {
        if (!blockerFrozen) {
            showDialog(true, $(this).attr('id'), 100);
        }
    }).ajaxStop(function () {
        $("#" + $(this).attr('id')).dialog("destroy");
    });
    $(".tile").tile({ width: 148 });
});

$(window).on('beforeunload', function () {
    //$.ui.dialog.overlay.create();
    if (!blockerFrozen) {
        showDialog(true, "loading", 100);
    }
});

function freezeBlocker() {
    blockerFrozen = true;
    setTimeout("thawBlocker()", 1000);
}

function thawBlocker() {
    blockerFrozen = false;
}

function layoutElements() {
    $(":button").button();
    $(":submit").button();
    $(".datePicker").attr("readonly", true);
    $(".datePicker").datepicker();
}

function showMessage(isModal, text, width) {
    var crc = parseInt(Math.random() * 1000);
    $("body").append("<div id='dialog_" + crc + "'>" + text + "</div>");
    var elementId = "dialog_" + crc;
    showDialog(isModal, elementId, width);
    return elementId;
}

function showDialog(isModal, elementId, widthVal, dialogTitle) {
    $("#" + elementId).dialog({
        modal: isModal,
        show: {
            effect: "fade",
            duration: 350
        },
        hide: {
            effect: "fade",
            duration: 200
        },
        width: widthVal != "" ? widthVal : 300,
        title: dialogTitle ? dialogTitle : ""
    });
}

function showDialogAjax(isModal, url, dataToSend, widthVal, callback, dialogTitle) {
    var tag = $("<div id='dialog_new'></div>");
    $("body").append(tag);
    $("#dialog_new").load(url, dataToSend, function () {
        $("#dialog_new").dialog({
            modal: isModal,
            show: {
                effect: "fade",
                duration: 350
            },
            hide: {
                effect: "fade",
                duration: 200
            },
            width: widthVal != "" ? widthVal : 300,
            title: dialogTitle ? dialogTitle : ""
        });
        if(callback) callback();
    });
}

function killAllMessages() {
    $("div[id^='dialog_']").dialog("close");
}

function inOver(element) { }

function inOut(element) { }

function inFocus(element) { }

function inBlur(element) { }

function parseAndNavToURL(url) {
    var qsIdx = url.indexOf("?");
    var tmpArr = new Array();
    var parsed, lastIdx, arg, val, sep, limit, pageName;
    if (qsIdx != -1) {
        var qs = url.substr(qsIdx, url.length - qsIdx);
        parsed = false;
        lastIdx = 0;
        pageName = url.substr(0, qsIdx);
        while (!parsed) {
            sep = qs.indexOf("=", lastIdx);
            limit = qs.indexOf("&", sep);
            arg = qs.substr(lastIdx + 1, (sep - lastIdx - 1));
            if (limit == -1) {
                limit = qs.length;
                parsed = true;
            }
            val = qs.substr(sep + 1, limit - (sep + 1));
            if (val.indexOf("?") != -1) {
                val = qs.substr(sep + 1, qs.length - sep);
                parsed = true;
            }
            tmpArr.push({ param: arg, value: val });
            lastIdx = limit;
        }
        navigateToURL_Arr(pageName, tmpArr);
    } else {
        location.href = url;
    }
}

function navigateToURL_Arr(url, arr) {
    var i; var items; var target; var winparams; var isiframe;
    target = "_self";
    items = "";
    winparams = "";
    isiframe = false;
    if (url == null || url == '') return false;
    for (i = 0; i < arr.length; i++) {
        if (arr[i].param == '_winparams') {
            winparams = arr[i].value;
        } else if (arr[i].param == '_target') {
            target = arr[i].value;
        } else if (arr[i].param == '_isiframe') {
            isiframe = true;
        } else {
            var s = '' + arr[i].value;
            items += '<input type="hidden" name="' + arr[i].param + '" value="' + s.replace(/"/gi, '&quot;') + '" />';
        }
    }
    return buildNavForm(url, target, items, winparams, isiframe);
}

function navigateToURL(url) {
    var i; var items; var target; var winparams; var isiframe;
    target = "_self";
    items = "";
    winparams = "";
    isiframe = false;
    if (url == null || url == '') return false;
    for (i = 1; i < navigateToURL.arguments.length; i += 2) {
        if (navigateToURL.arguments[i] == '_winparams') {
            winparams = navigateToURL.arguments[i + 1];
        } else if (navigateToURL.arguments[i] == '_target') {
            target = navigateToURL.arguments[i + 1];
        } else if (navigateToURL.arguments[i] == '_isiframe') {
            isiframe = true;
        } else if (navigateToURL.arguments[i + 1] != null) {
            var s = '' + navigateToURL.arguments[i + 1];
            items += '<input type="hidden" name="' + navigateToURL.arguments[i] + '" value="' + s.replace(/"/gi, '&quot;') + '" />';
        }
    }
    return buildNavForm(url, target, items, winparams, isiframe);
}

function buildNavForm(url, target, items, winparams, isiframe) {
    var formHTML;
    var w = null;
    formHTML = "<form id='navToURLForm' name='navToURLForm' method='POST' action='" + url + "' target='" + target + "'>" + items + "</form>";
    var container = document.getElementById('navToURLContainer');
    if (container == null) {
        container = document.createElement("div");
        container.id = "navToURLContainer";
        document.body.appendChild(container);
    }
    container.innerHTML = formHTML;
    if (target != "_self") {
        if (!isiframe) {
            w = window.open("", target, winparams);
            if (w) {
                w.opener = self;
                w.focus();
            }
        }
    }
    document.getElementById('navToURLForm').submit();
    var temp = document.getElementById('navToURLContainer');
    document.body.removeChild(temp);
    return w;
}
