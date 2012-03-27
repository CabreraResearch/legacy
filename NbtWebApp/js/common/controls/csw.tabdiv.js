/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
            var internal = {
                $parent: '',
                ID: '',
                name: '',
                cssclass: '',
                text: '',
                title: '',
                align: ''
            };
            var external = {};

            (function () {
                $.extend(internal, options);
                external = cswParent.div(internal);
                //$.extend(external, Csw.literals.div(internal));
            } ());

            external.tabs = function () {
                var ret,
                    tryRet = Csw.tryJqExec(external, 'tabs', arguments);
                if (Csw.isJQuery(tryRet)) {
                    ret = external.jquery(tryRet);
                } else {
                    ret = tryRet;
                }
                return ret;
            };

            return external;
        });

} ());

