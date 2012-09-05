/// <reference path="~app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.tabDiv = Csw.controls.tabDiv ||
        Csw.controls.register('tabDiv', function (cswParent, options) {
            'use strict';
            /// <summary> Create or extend an HTML <div /> and return a Csw.tabdivobject
            ///     &#10;1 - tabdiv(options)
            ///     &#10;2 - tabdiv($jqueryElement)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the div.</para>
            /// <para>options.name: A name for the div.</para>
            /// <para>options.cssclass: CSS class to asign</para>
            /// <para>options.text: Text to display</para>
            /// </param>
            /// <returns type="tabdiv">A tabdiv object</returns>
            var cswPrivate = {
                ID: '',
                name: '',
                cssclass: '',
                text: '',
                title: '',
                align: ''
            };
            var cswPublic = {};

            (function () {
                Csw.extend(cswPrivate, options);
                cswPublic = cswParent.div(cswPrivate);
                //Csw.extend(cswPublic, Csw.literals.div(cswPrivate));
            } ());

            cswPublic.tabs = function () {
                var ret,
                    tryRet = Csw.tryJqExec(cswPublic, 'tabs', arguments);
                if (Csw.isJQuery(tryRet)) {
                    ret = cswPublic.jquery(tryRet);
                } else {
                    ret = tryRet;
                }
                return ret;
            };

            return cswPublic;
        });

} ());

