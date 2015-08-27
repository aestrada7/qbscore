/**********************************************************************************
 **
 **              JavaScript Tristate Checkbox Plugin
 **              version: 1.00
 **
 **              Dual licensed under the MIT and GPL licenses:
 **              http://www.opensource.org/licenses/mit-license.php
 **              http://www.gnu.org/licenses/gpl.html
 **
 **              author: Antonio Estrada
 **              creation date: 12.04.2013
 **              dependencies: jQuery v1.7 or higher
 **
 **              This file contains the functionality for implementing 3 state checkboxes.
 **
 **********************************************************************************/
function Role(_idModule, _idModuleParent) {
    this.idModule = _idModule;
    this.idModuleParent = _idModuleParent;
}

function treePropagate(prefix, status, elementAt) {
    var i;
    for (i = 0; i < roles.length; i++) {
        if (roles[i].idModuleParent == elementAt) {
            if (status == 1) //checked
            {
                $("#" + prefix + roles[i].idModule).prop("checked", true);
            }
            else if (status == 0) //unchecked
            {
                $("#" + prefix + roles[i].idModule).prop("checked", false);
            }
            $("#" + prefix + roles[i].idModule).prop("indeterminate", false);
            treePropagate(prefix, status, roles[i].idModule);
        }
    }
}

function checkTree(prefix, status, elementAt, startedBy) {
    var i;
    var totalChildren = 0;
    var totalChecked = 0;
    var totalUnchecked = 0;
    var parent = 0;
    //Handle the click and send it down
    if (elementAt == startedBy) treePropagate(prefix, status, elementAt);
    //Go down the hierarchy (depending on what they clicked)
    for (i = 0; i < roles.length; i++) {
        if (roles[i].idModuleParent == elementAt) {
            totalChildren++;
            if ($("#" + prefix + roles[i].idModule).prop("checked")) {
                totalChecked++;
            }
            else {
                totalUnchecked++;
            }
        }
    }
    if (totalChildren > 0) {
        if (totalChildren == totalChecked) {
            $("#" + prefix + elementAt).prop("checked", true);
            $("#" + prefix + elementAt).prop("indeterminate", false);
        }
        else {
            if (totalChildren == totalUnchecked) {
                $("#" + prefix + elementAt).prop("checked", false);
                $("#" + prefix + elementAt).prop("indeterminate", false);
            }
            else {
                $("#" + prefix + elementAt).prop("checked", false);
                $("#" + prefix + elementAt).prop("indeterminate", true);
            }
        }
    }
    //Go up the hierarchy
    for (i = 0; i < roles.length; i++) {
        if (roles[i].idModule == elementAt) {
            parent = roles[i].idModuleParent;
            if (status == 1) //checked
            {
                $("#" + prefix + roles[i].idModuleParent).prop("checked", true);
            }
            else if (status == 0) //unchecked
            {
                $("#" + prefix + roles[i].idModuleParent).prop("checked", false);
            }
            if (roles[i].idModuleParent != 0) {
                checkTree(prefix, status, roles[i].idModuleParent);
            }
        }
    }
}
