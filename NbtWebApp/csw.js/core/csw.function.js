/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswFunction() {
    'use strict';

     function tryExec(func) {
        /// <summary> If the supplied argument is a function, execute it. </summary>
        /// <param name="func" type="Function"> Function to evaluate </param>
        /// <returns type="undefined" />
        if (isFunction(func)) {
            func.apply(this, Array.prototype.slice.call(arguments, 1));
        }
    };
    Csw.register('tryExec', tryExec);
    Csw.tryExec = Csw.tryExec || tryExec;

    function isFunction(obj) {
        /// <summary> Returns true if the object is a function</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = ($.isFunction(obj));
        return ret;
    };
    Csw.register('isFunction', isFunction);
    Csw.isFunction = Csw.isFunction || isFunction;

}());