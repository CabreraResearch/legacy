/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    
        Csw.browserCompatibility.register('usingIE10', function () {
            /// <summary>Attempt to detect if browser is Internet Explorer 10.</summary>
            /// <ret>Boolean: whether or not browser is IE10</ret>
            'use strict';
            return (window.CswIeVersion && window.CswIeVersion > 9) || navigator.userAgent.contains("MSIE 10.0");
        });
}());
