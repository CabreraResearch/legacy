
/// <reference path="~/app/CswApp-vsdoc.js" />

(function _cswFunction() {
    'use strict';

    Csw.register('tryJqExec', function (cswObj, method) {
        'use strict';
        /// <summary> If the supplied argument is a function, execute it. </summary>
        /// <param name="func" type="Function"> Function to evaluate </param>
        /// <returns type="undefined" />
        try {
            var args = arguments[2];
            return cswObj.$[method].apply(cswObj.$, args);
        } catch (exception) {
            Csw.error.catchException(exception);
        }
    });

    Csw.register('defer', Csw.method(function (func, delay) {
        /// <summary>
        /// Defer the execution of a method by a number of milliseconds
        /// </summary>
        func = func || function () {
        };
        delay = delay || 1000;
        return window.Ext.defer(func, delay);
    }));

}());
