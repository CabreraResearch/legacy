
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    Csw.makeDelegate = Csw.makeDelegate ||
        Csw.register('makeDelegate', function (method, options) {
            'use strict';
            /// <summary>
            /// Returns a function with the argument parameter of the value of the current instance of the object.
            /// </summary>
            /// <param name="method" type="Function"> A function to delegate. </param>
            /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
            /// <returns type="Function">A delegate function: function (options)</returns>
            return function () {
                'use strict';
                method(options);
            };
        });

    Csw.makeEventDelegate = Csw.makeEventDelegate ||
        Csw.register('makeEventDelegate', function (method, options) {
            'use strict';
            /// <summary>
            /// Returns a function with the event object as the first parameter, and the current value of options as the second parameter.
            /// </summary>
            /// <param name="method" type="Function"> A function to delegate. </param>
            /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
            /// <returns type="Function">A delegate function: function (eventObj, options)</returns>
            return function (eventObj) {
                'use strict';
                method(eventObj, options);
            };
        });

} ());
