/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';

    Csw.log = Csw.log ||
        Csw.register('log', function (s, includeCallStack) {
            /// <summary>Outputs a message to the console log(Webkit,FF) or an alert(IE)</summary>
            /// <param name="s" type="String"> String to output </param>
            /// <param name="includeCallStack" type="Boolean"> If true, include the callStack </param>
            var extendedLog = '';
            if (Csw.bool(includeCallStack)) {
                extendedLog = console.trace();
            }
            try {
                if (false === Csw.isNullOrEmpty(extendedLog)) {
                    console.log(s, extendedLog);
                } else {
                    console.log(s);
                }
            } catch (e) {
                // because IE 8 doesn't support console.log unless the console is open (*duh*)
//                alert(s);
//                if (false === Csw.isNullOrEmpty(extendedLog)) {
//                    alert(extendedLog);
//                }
            }
        });

} ());
