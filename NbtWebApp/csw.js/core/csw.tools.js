/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswTools() {
    'use strict';

//    var loadResource = function (filename, filetype, useJquery) {
//        var fileref, type = filetype || 'js';
//        switch (type) {
//            case 'js':
//                if (jQuery && (($.browser.msie && $.browser.version <= 8) || useJquery)) {
//                    $.ajax({
//                        url: '/NbtWebApp/' + filename,
//                        dataType: 'script'
//                    });
//                } else {
//                    fileref = document.createElement('script');
//                    fileref.setAttribute("type", "text/javascript");
//                    fileref.setAttribute("src", filename);
//                }
//                break;
//            case 'css':
//                fileref = document.createElement("link");
//                fileref.setAttribute("rel", "stylesheet");
//                fileref.setAttribute("type", "text/css");
//                fileref.setAttribute("href", filename);
//                break;
//        }
//        if (fileref) {
//            document.getElementsByTagName("head")[0].appendChild(fileref);
//        }
//    };
//    Csw.register('loadResource', loadResource);
//    Csw.loadResource = Csw.loadResource || loadResource;

    function makeId(options, prefix, suffix, delimiter) {
        /// <summary>
        ///   Generates an ID for DOM assignment
        /// </summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.ID: Base ID string
        ///     &#10;2 - options.prefix: String prefix to prepend
        ///     &#10;3 - options.suffix: String suffix to append
        ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
        /// </param>
        /// <returns type="String">A concatenated string of provided values</returns>
        var o = {
            ID: '',
            prefix: Csw.string(prefix),
            suffix: Csw.string(suffix),
            Delimiter: Csw.string(delimiter, '_')
        };
        if (Csw.isPlainObject(options)) {
            $.extend(o, options);
        } else {
            o.ID = Csw.string(options);
        }
        
        var elementId = o.ID;
        if (false === Csw.isNullOrEmpty(o.prefix) && false === Csw.isNullOrEmpty(elementId)) {
            elementId = o.prefix + o.Delimiter + elementId;
        }
        if (false === Csw.isNullOrEmpty(o.suffix) && false === Csw.isNullOrEmpty(elementId)) {
            elementId += o.Delimiter + o.suffix;
        }
        return elementId;
    }
    Csw.register('makeId', makeId);
    Csw.makeId = Csw.makeId || makeId;
    
    function makeSafeId(options, prefix, suffix, delimiter) {
        /// <summary>   Generates a "safe" ID for DOM assignment </summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.ID: Base ID string
        ///     &#10;2 - options.prefix: String prefix to prepend
        ///     &#10;3 - options.suffix: String suffix to append
        ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
        /// </param>
        /// <returns type="String">A concatenated string of provided values</returns>
        var o = {
            ID: '',
            prefix: Csw.string(prefix),
            suffix: Csw.string(suffix),
            Delimiter: Csw.string(delimiter, '_')
        };
        if (Csw.isPlainObject(options)) {
            $.extend(o, options);
        } else {
            o.ID = Csw.string(options);
        }
        
        var elementId = o.ID;
        var toReplace = [ /'/gi , / /gi , /\//g ];
        if (false === Csw.isNullOrEmpty(o.prefix) && false === Csw.isNullOrEmpty(elementId)) {
            elementId = o.prefix + o.Delimiter + elementId;
        }
        if (false === Csw.isNullOrEmpty(o.suffix) && false === Csw.isNullOrEmpty(elementId)) {
            elementId += o.Delimiter + o.suffix;
        }
        for (var i = 0; i < toReplace.length; i++) {
            if (Csw.contains(toReplace, i)) {
                if (false === Csw.isNullOrEmpty(elementId)) {
                    elementId = elementId.replace(toReplace[i], '');
                }
            }
        }
        return elementId;
    }
    Csw.register('makeSafeId', makeSafeId);
    Csw.makeSafeId = Csw.makeSafeId || makeSafeId;

    function hasWebStorage() {
        var ret = (window.Modernizr.localstorage || window.Modernizr.sessionstorage);
        return ret;
    }
    Csw.register('hasWebStorage', hasWebStorage);
    Csw.hasWebStorage = Csw.hasWebStorage || hasWebStorage;
    
    function tryParseElement(elementId, $context) {
        /// <summary>Attempts to fetch an element from the DOM first through jQuery, then through JavaScript.</summary>
        /// <param name="elementId" type="String"> ElementId to find </param>
        /// <param name="$context" type="jQuery"> Optional context to limit the search </param>
        /// <returns type="jQuery">jQuery object, empty if no match found.</returns>
        var $ret = $('');
        var document = Csw.getGlobalProp('document');
        if (false === Csw.isNullOrEmpty(elementId)) {
            if (arguments.length === 2 && false === Csw.isNullOrEmpty($context)) {
                $ret = $('#' + elementId, $context);
            } else {
                $ret = $('#' + elementId);
            }
            if ($ret.length === 0) {
                $ret = $(document.getElementById(elementId));
            }
            if ($ret.length === 0) {
                $ret = $(document.getElementsByName(elementId));
            }
        }
        return $ret;
    }
    Csw.register('tryParseElement', tryParseElement);
    Csw.tryParseElement = Csw.tryParseElement || tryParseElement;
    
}());