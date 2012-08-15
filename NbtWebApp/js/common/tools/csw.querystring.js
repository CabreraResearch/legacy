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
            var cswPublic = {};
            var cswPrivate = {
                e: '',
                a: /\+/g, // Regex for replacing addition symbol with a space
                r: /([^&=]+)=?([^&]*)/g,
                d: function (s) { return decodeURIComponent(s.replace(cswPrivate.a, " ")); },
                q: Csw.window.location().search.substring(1)
            };

            while (cswPrivate.e = cswPrivate.r.exec(cswPrivate.q)) {
                cswPublic[cswPrivate.d(cswPrivate.e[1])] = cswPrivate.d(cswPrivate.e[2]);
            }
            if(false === Csw.contains(cswPublic, 'pageName')) {
                cswPublic.pageName = window.location.pathname.substring(window.location.pathname.lastIndexOf('/') + 1);
            }
            return cswPublic;

        });
} ());
