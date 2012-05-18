/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />


(function () {
    // adapted from 
    // http://stackoverflow.com/questions/901115/get-querystring-values-in-javascript/2880929#2880929

    Csw.queryString = Csw.queryString ||
        Csw.register('queryString', function () {
            /// <summary>
            ///     usage:    page.html?param=value&param2=value2
            ///     &#10;1 - var qs = Csw.queryString();
            ///     &#10;2 - qs.param   -->  'value'
            ///     &#10;3 - qs.param2  -->  'value2'
            /// </summary>
            /// <returns type="Object" />
            'use strict';
            var cswPublicRet = {};
            var cswPrivateVar = {
                e: '',
                a: /\+/g, // Regex for replacing addition symbol with a space
                r: /([^&=]+)=?([^&]*)/g,
                d: function (s) { return decodeURIComponent(s.replace(cswPrivateVar.a, " ")); },
                q: Csw.window.location().search.substring(1)
            };

            while (cswPrivateVar.e = cswPrivateVar.r.exec(cswPrivateVar.q)) {
                cswPublicRet[cswPrivateVar.d(cswPrivateVar.e[1])] = cswPrivateVar.d(cswPrivateVar.e[2]);
            }

            return cswPublicRet;

        });
} ());
