;/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function() {
    'use strict';

    function makeDelegate(method, options) {
        /// <summary>
        /// Returns a function with the argument parameter of the value of the current instance of the object.
        /// </summary>
        /// <param name="method" type="Function"> A function to delegate. </param>
        /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
        /// <returns type="Function">A delegate function: function (options)</returns>
        return function() { method(options); };
    }
    Csw.register('makeDelegate', makeDelegate);
    Csw.makeDelegate = Csw.makeDelegate || makeDelegate;

    function makeEventDelegate(method, options) {
        /// <summary>
        /// Returns a function with the event object as the first parameter, and the current value of options as the second parameter.
        /// </summary>
        /// <param name="method" type="Function"> A function to delegate. </param>
        /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
        /// <returns type="Function">A delegate function: function (eventObj, options)</returns>
        return function(eventObj) { method(eventObj, options); };
    }
    Csw.register('makeEventDelegate', makeEventDelegate);
    Csw.makeEventDelegate = Csw.makeEventDelegate || makeEventDelegate;

}());
