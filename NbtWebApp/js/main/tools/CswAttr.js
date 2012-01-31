/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />

(function ($) { 
    "use strict";
    $.fn.CswAttrDom = function (name, value) {
        /// <summary>
        ///   Gets or sets a DOM attribute
        /// </summary>
        /// <param name="name" type="String">The name of the attribute</param>
        /// <param name="value" type="String">The value of the attribute</param>
        /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 

        var $Dom = $(this);
        var ret = $Dom;
        
        try {
            if (typeof name === "object") {
                for (var prop in name) {
                    doProp($Dom, prop, name[prop]);
                }
            } else {
                ret = doProp($Dom, name, value);
            }
        } catch (e) {
            //We're in IE hell. Do nothing.
        }
        return ret;

    }; // function (options) {

    function doProp($Dom, name, value) {
        var ret = '';

        try {
            if (arguments.length === 2) {
                ret = $Dom.prop(name);
            } else {
                ret = $Dom.prop(name, value);
            }

            // special cases
            if (ret === undefined ||
                name === 'href' ||
                    name === 'cellpadding' ||
                        name === 'cellspacing' ||
                            name === 'rowspan' ||
                                name === 'colspan') {
                if (arguments.length === 2) {
                    ret = $Dom.attr(name);
                } else {
                    ret = $Dom.attr(name, value);
                }
            }
        } catch (e) {
            //We're in IE hell. Do nothing.
        }
         
        return ret;
    }

    $.fn.CswAttrNonDom = function (name, value) {

        /// <summary>
        ///   Gets or sets an Non-Dom attribute
        /// </summary>
        /// <param name="name" type="String">The name of the attribute</param>
        /// <param name="value" type="String">The value of the attribute</param>
        /// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 

        var $this = $(this);
        var ret = $this;
        try {
            if (typeof name === "object") {
                for (var prop in name) {
                    doAttr($this, prop, name[prop]);
                }
            } else {
                ret = doAttr($this, name, value);
            }
            // For proper chaining support
        } catch (e) {
            //We're in IE hell. Do nothing.
        }
        return ret;

    }; // function (options) {

    function doAttr($this, name, value) {
        var ret = $this;

        try {
            switch (arguments.length) {
                case 2:
                    ret = $this.attr(name);
                    break;
                case 3:
                    ret = $this.attr(name, value);
                    break;
            }
        } catch (e) {
            //We're in IE hell. Do nothing.
        }
        // For proper chaining support
        return ret;
    }

})(jQuery);


