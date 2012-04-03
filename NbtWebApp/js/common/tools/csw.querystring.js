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
            var external = {};
            var internal = {
                e: '',
                a: /\+/g, // Regex for replacing addition symbol with a space
                r: /([^&=]+)=?([^&]*)/g,
                d: function (s) { return decodeURIComponent(s.replace(internal.a, " ")); },
                q: window.location.search.substring(1)
            };

            while (internal.e = internal.r.exec(internal.q)) {
                external[internal.d(internal.e[1])] = internal.d(internal.e[2]);
            }

            return external;

        });
} ());
