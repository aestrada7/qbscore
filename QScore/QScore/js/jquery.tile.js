/*
 * Metro Tile
 *
 * Copyright 2014, Tony Estrada (@aestrada7)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 *
 */

(function ($) {

    $.fn.tile = function (o) {
        var tileHTML = "";
        var tileBGColor = "";
        var tileImg = "";
        var currentWidth = 0;
        var currentText = 0;
        var tiles = [];

        if(!o) var o = {};
        if (o.width == undefined) o.width = 0;
        if (o.text == undefined) o.text = "";
        if (o.animTime == undefined) o.animTime = 200;
        if (o.noRandom == undefined) o.noRandom = false;

        $(this).each(function () {
            if (o.text == "") {
                currentText = $(this).children("a").attr("alt");
            } else {
                currentText = o.text;
            }
            if (o.width == 0) {
                currentWidth == $(this).width();
            } else {
                currentWidth = o.width;
            }
            tileImg = $("#" + $(this).attr('id') + " img");
            tileBGColor = $(this).css('border-color');
            tileHTML = "<div id='" + $(this).attr('id') + "_txt' style='display: none; position: absolute; z-index: 20; width: " + currentWidth + "; height: auto; top: 0; left: 0; background-color: " + tileBGColor + "'>" + currentText + "</div>";
            $(this).append(tileHTML);
            $(this).mouseenter({ tile: tileImg }, function (obj) {
                $("#" + $(this).attr('id') + "_txt").slideDown(o.animTime);
                obj.data.tile.animate({
                    opacity: .7
                }, o.animTime);
            });
            $(this).mouseleave({ tile: tileImg }, function (obj) {
                $("#" + $(this).attr('id') + "_txt").slideUp(o.animTime);
                obj.data.tile.animate({
                    opacity: 1
                }, o.animTime);
            });
            tiles.push($(this));            
        });

        if (!o.noRandom) {
            setInterval(randomFlip, 4000);
        }

        function randomFlip() {
            if (tiles.length > 0) {
                var tempMenuItem = parseInt(tiles.length * Math.random());
                tiles[tempMenuItem].mouseenter();
                var seconds = 2 + parseInt(8 * Math.random());
                setTimeout(flipBack, seconds * 1000, tempMenuItem);
            }
        }

        function flipBack(idx) {
            tiles[idx].mouseleave();
        }
    }

})(jQuery);
