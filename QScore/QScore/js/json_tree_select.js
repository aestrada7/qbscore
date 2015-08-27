/*
 * JSON Tree Select 1.0
 *
 * Copyright 2013-2014, Tony Estrada (@aestrada7)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 *
 */

(function ($) {

  $.fn.treeSelectJSON = function (o) {
    var currSelection = [0,0,0,0,0,0,0,0,0,0];
    var currObjects = ["","","","","","","","","",""];
    var lastId = "";
    
    if(!o) var o = {};
    if(o.source == undefined ) o.source = '';
    if(o.target == undefined ) o.target = '';
    if(o.defaultText == undefined ) o.defaultText = 'Select';
    if(o.extraHtml == undefined ) o.extraHtml = '';
    if(o.preselected == undefined ) o.preselected = '';
    
    $(this).each( function() {
      function buildSelect(who, treeRoute)
      {
        var htmlStr = "";
        var id = who.prop('id') + "_l" + treeRoute;
        lastId = id;
        var obj;
        var treeRouteArr = [];
        htmlStr = "<select id='" + id + "' " + o.extraHtml + ">";
        htmlStr += "<option value='0'>" + o.defaultText + "</input>";
        if(treeRoute == "")
        {
          obj = o.source.children;
        }
        else
        {
          obj = o.source.children;
          treeRouteArr = treeRoute.split("_");
          for(var k = 0; k < treeRouteArr.length; k++)
          {
            for(var j = 0; j < obj.length; j++)
            {
              if(obj[j].id == treeRouteArr[k])
              {
                obj = obj[j].children;
                break;
              }
            }
          }
        }
        if(obj != undefined)
        {
          for(var i = 0; i < obj.length; i++)
          {
            htmlStr += "<option value='" + obj[i].id + "'>" + obj[i].value + "</input>";
          }
        }
        htmlStr += "</select>";
        htmlStr += "<br id='" + id + "_br' />";
        currObjects[treeRouteArr.length] = id;
        if(obj != undefined) who.append(htmlStr);
        $("#" + id).change(function() {
          currSelection[treeRouteArr.length] = $(this).val();
          var target = $("#" + o.target);
          target.val("");
          for(var i = 0; i < currSelection.length; i++)
          {
            if(i > treeRouteArr.length)
            {
              currSelection[i] = 0;
            }
            if(target.val() != "")
            {
              if(currSelection[i] != 0)
              {
                target.val(target.val() + "_" + currSelection[i]);
              }
            }
            else
            {
              if(currSelection[i] != 0)
              {
                target.val(currSelection[i]);
              }
            }
          }
          var currentTargets = target.val().split("_");
          for(var k = 0; k < currObjects.length; k++)
          {
            if(k >= currentTargets.length)
            {
              $("#" + currObjects[k]).remove();
              $("#" + currObjects[k] + "_br").remove();
              currObjects[k] = "";
            }
          }
          if(target.val() != "")
          {
            buildSelect(who, target.val());
          }
        });
      }
      
      buildSelect($(this), "");
      if(o.preselected != "")
      {
        var preselArr = o.preselected.split("_");
        for(var i = 0; i < preselArr.length; i++)
        {
          $("#" + lastId).val(preselArr[i]);
          $("#" + lastId).trigger("change");
        }
      }
    });
    
  }
  
})(jQuery);