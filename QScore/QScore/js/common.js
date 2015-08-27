function validateInt(element, event, from, to) {
    var retval = false;
    if (!ignoreKey(event.keyCode)) {
        var current = $(element).val();
        current = current.replace(/\D/g, '');
        $(element).val(current);
        if ((parseInt(current) < from) || (parseInt(current) > to)) {
            markInvalid(element);
        } else {
            markValid(element);
            retval = true;
        }
    }
    return retval;
}

function validateFloat(element, event, from, to) {
    var retval = false;
    if (!ignoreKey(event.keyCode)) {
        var current = $(element).val();
        var regex = /^\d*[\.]?\d*$/;
        if (!regex.test(current)) {
            var totalDots = (current.split(".").length - 1);
            if (totalDots > 1) {
                var index = current.lastIndexOf(".");
                current = current.substr(0, index) + current.substr(index + 1);
            }
            current = current.replace(/[^0-9.]/g, '');
        }
        $(element).val(current);
        if ((parseFloat(current) < from) || (parseFloat(current) > to)) {
            markInvalid(element);
        } else {
            markValid(element);
            retval = true;
        }
    }
    return retval;
}

function validateEmail(element) {
    var retval = false;
    var current = $(element).val();
    var regex = /[-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}/;
    if (!regex.test(current)) {
        markInvalid(element);
    } else {
        markValid(element);
        retval = true;
    }
    $(element).val(current);
    return retval;
}

function markInvalid(element) {
    $(element).addClass("errorItem");
    $(element).qtip({
        position: {
            adjust: { x: 5, y: -35 }
        }
    });
    $(element).qtip('toggle', true);
}

function markValid(element) {
    $(element).removeClass("errorItem");
}

function ignoreKey(keyCode) {
    if ((keyCode == 13) || //enter
        (keyCode == 16) || //shift
        (keyCode == 17) || //ctrl
        (keyCode == 18) || //alt
        (keyCode == 27) || //escape
        (keyCode == 33) || //pageup
        (keyCode == 34) || //pagedown
        (keyCode == 35) || //end
        (keyCode == 36) || //home
        (keyCode == 37) || //left
        (keyCode == 38) || //up
        (keyCode == 39) || //right
        (keyCode == 40) || //down
        (keyCode == 45) || //insert
        (keyCode == 46) || //delete
        (keyCode == 8) || //backspace
        (keyCode == 9)) { //tab
        return true;
    }
    return false;
}

function toggleCheckbox(element) {
    $(element).val(($(element).val() != 1) ? 1 : 0);
}

function strAdd(string1, connector, string2) {
    if (!string1) {
        return string2;
    } else if (!string2) {
        return string1;
    } else {
        return string1 + connector + string2;
    }
}

function viewLog(idLog) {
    freezeBlocker();
    $.post("./viewLog.cshtml", { idLog: idLog })
        .done(function (data) {            
            showMessage(true, data, 600);
        });
}

function firstPage() {
    $('#page').val(1);
    submitForm();
}

function nextPage() {
    $('#page').val(parseInt($('#page').val()) + 1);
    submitForm();
}

function prevPage() {
    $('#page').val(parseInt($('#page').val()) - 1);
    submitForm();
}

function lastPage(last) {
    $('#page').val(last);
    submitForm();
}