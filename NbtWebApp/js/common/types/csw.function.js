/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswFunction() {
    'use strict';

    function isFunction(obj) {
        /// <summary> Returns true if the object is a function</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = ($.isFunction(obj));
        return ret;
    }
    Csw.register('isFunction', isFunction);
    Csw.isFunction = Csw.isFunction || isFunction;

    function tryExec(func) {
        /// <summary> If the supplied argument is a function, execute it. </summary>
        /// <param name="func" type="Function"> Function to evaluate </param>
        /// <returns type="undefined" />
        if (isFunction(func)) {
            return func.apply(this, Array.prototype.slice.call(arguments, 1));
        }
    }
    Csw.register('tryExec', tryExec);
    Csw.tryExec = Csw.tryExec || tryExec;

    function tryJqExec(cswObj, method) {
        /// <summary> If the supplied argument is a function, execute it. </summary>
        /// <param name="func" type="Function"> Function to evaluate </param>
        /// <returns type="undefined" />
        var args = arguments[2];
        return cswObj.$[method].apply(cswObj.$, args);
    }
    Csw.register('tryJqExec', tryJqExec);
    Csw.tryJqExec = Csw.tryJqExec || tryJqExec;

} ());
