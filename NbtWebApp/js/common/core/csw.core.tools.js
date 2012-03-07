/// <reference path="~/js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswTools() {
    'use strict';

/*    
    var loadResource = function (filename, filetype, useJquery) {
        var fileref, type = filetype || 'js';
        switch (type) {
            case 'js':
                if (jQuery && (($.browser.msie && $.browser.version <= 8) || useJquery)) {
                    $.ajax({
                        url: '/NbtWebApp/' + filename,
                        dataType: 'script'
                    });
                } else {
                    fileref = document.createElement('script');
                    fileref.setAttribute("type", "text/javascript");
                    fileref.setAttribute("src", filename);
                }
                break;
            case 'css':
                fileref = document.createElement("link");
                fileref.setAttribute("rel", "stylesheet");
                fileref.setAttribute("type", "text/css");
                fileref.setAttribute("href", filename);
                break;
        }
        if (fileref) {
            document.getElementsByTagName("head")[0].appendChild(fileref);
        }
    };
    Csw.register('loadResource', loadResource);
    Csw.loadResource = Csw.loadResource || loadResource;
*/
    
    function hasWebStorage() {
        var ret = (window.Modernizr.localstorage || window.Modernizr.sessionstorage);
        return ret;
    }
    Csw.register('hasWebStorage', hasWebStorage);
    Csw.hasWebStorage = Csw.hasWebStorage || hasWebStorage;

}());
