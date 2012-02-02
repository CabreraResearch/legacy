/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswBoolean() {
    'use strict';

    function bool(str, isTrueIfNull) {

        var retBool;
        function isTrue () {
            /// <summary>
            ///   Returns true if the input is true, 'true', '1' or 1.
            ///   &#10;1 Returns false if the input is false, 'false', '0' or 0.
            ///   &#10;2 Otherwise returns false and (if debug) writes an error to the log.
            /// </summary>
            /// <param name="str" type="Object">
            ///     String or object to test
            /// </param>
            /// <returns type="Bool" />
            var ret;
            var truthy = Csw.string(str).toLowerCase().trim();
            if (truthy === 'true' || truthy === '1') {
                ret = true;
            } else if (truthy === 'false' || truthy === '0') {
                ret = false;
            } else if (isTrueIfNull && Csw.isNullOrEmpty(str)) {
                ret = true;
            } else {
                ret = false;
            }
            return ret;
        }

        retBool = isTrue();
        
        return retBool;
    }

    Csw.register('bool', bool);
    Csw.bool = Csw.bool || bool;

}());