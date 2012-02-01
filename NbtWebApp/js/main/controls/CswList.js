/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";
    var pluginName = "CswList";

    var methods = {

        init: function (options) {
            var o = {
                ID: '',
                values: [],
                cssclass: '',
                ordered: false
            };
            if (options) $.extend(o, options);

            var $parent = $(this),
                $list, i, $li, val,
                elementId = Csw.string(o.ID);

            if (Csw.bool(o.ordered)) {
                $list = $('<ol id="' + elementId + '" name="' + elementId + '"></ol>');
            } else {
                $list = $('<ul id="' + elementId + '" name="' + elementId + '"></ul>');
            }

            for (i = 0; i < o.values.length; i += 1) {
                val = o.values[i];
                $li = $('<li></li>').appendTo($list);
                if (Csw.isJQuery(val)) {
                    $li.append(val);
                } else {
                    $li.append('<p>' + val + '</p>');
                }
            }

            if (false === Csw.isNullOrEmpty(o.cssclass)) {
                $list.addClass(o.cssclass);
            }

            $parent.append($list);
            return $list;
        },
        addItem: function (options) {
            var o = {
                value: ''
            };
            if (options) $.extend(o, options);

            var $list = $(this),
                val, $li;

            val = o.value;
            $li = $('<li></li>').appendTo($list);
            if (Csw.isJQuery(val)) {
                $li.append(val);
            } else {
                $li.append('<p>' + val + '</p>');
            }
            return $li;
        }
    };
    // Method calling logic
    $.fn.CswList = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };


})(jQuery);
