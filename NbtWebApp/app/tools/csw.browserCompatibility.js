/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
        Csw.browserCompatibility.register('usingIE10', function () {
            /// <summary>Attempt to detect if browser is Internet Explorer 10.</summary>
            /// <ret>Boolean: whether or not browser is IE10</ret>
            return navigator.userAgent.contains("MSIE 10.0");
        });
}());
