
/// <reference path="~/app/CswApp-vsdoc.js" />

(function _cswBoolean() {
    'use strict';

    function bool(str, isTrueIfNull) {
        /// <summary>
        ///   Returns true if the input is true, 'true', '1' or 1.
        ///   &#10;1 Returns false if the input is false, 'false', '0' or 0.
        ///   &#10;2 Otherwise returns false.
        /// </summary>
        /// <param name="str" type="Object">String or object to test</param>
        /// <param name="isTrueIfNull" type="Boolean">If true, null will evaluate to true.</param>
        /// <returns type="Bool" />
        var retBool;
        function toBool() {
            /// <summary>Converts the input to a boolean.</summary>
            /// <returns type="Bool" />
            var ret, truthy;
            if (str === false) {
                ret = false;
            } else {
                truthy = Csw.string(str).toLowerCase().trim();
                if (truthy === 'true' || truthy === '1') {
                    ret = true;
                } else if (truthy === 'false' || truthy === '0') {
                    ret = false;
                } else if (isTrueIfNull && Csw.isNullOrEmpty(str)) {
                    ret = true;
                } else {
                    ret = false;
                }
            }
            return ret;
        }

        retBool = toBool();

        return retBool;
    }

    Csw.register('bool', bool);
    Csw.bool = Csw.bool || bool;

} ());
