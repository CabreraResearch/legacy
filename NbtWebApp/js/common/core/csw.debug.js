/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';

    var cswPrivate = {
        that: this
    };

    var cswPublic = { };

    cswPublic.assert = function (truth, msg) {
        /// <summary>Evaluates the truthiness of truth and throws an exception with msg val if false.</summary>
        try {
            console.assert(truth, msg);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.count = function (msg) {
        /// <summary>Displays a count of the number of time the msg has been met in the console log(Webkit,FF).</summary>
        try {
            console.count(msg);
        } catch (e) {
            /* Do nothing */
        }
    };
    
    cswPublic.error = function (msg) {
        /// <summary>Outputs an error message to the console log(Webkit,FF)</summary>
        try {
            console.error(msg);
        } catch (e) {
            Csw.debug.log(msg);
        }
    };

    cswPublic.group = function (name) {
        /// <summary>Begins a named message group in console log(Webkit,FF)</summary>
        try {
            console.group(name);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.groupCollapsed = function (name) {
        /// <summary>Creates a named, collapsed message group in the console log(Webkit,FF)</summary>
        try {
            console.groupCollapsed(name);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.groupEnd = function (name) {
        /// <summary>Ends a named message group in the console log(Webkit,FF)</summary>
        try {
            console.groupEnd(name);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.info = function (msg) {
        /// <summary>Outputs an info message to the console log(Webkit,FF)</summary>
        try {
            console.info(msg);
        } catch (e) {
            Csw.debug.log(msg);
        }
    };

    cswPublic.log = function (msg) {
        /// <summary>Outputs an unstyled message to the console log(Webkit,FF)</summary>
        try {
            console.log(msg);
        } catch (e) {
            try {
                window.dump(msg);
            } catch (e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.profile = function (name) {
        /// <summary>Beginds a named profile in the console log(Webkit,FF)</summary>
        try {
            console.profile(name);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.profileEnd = function (name) {
        /// <summary>Ends a named profile in the console log(Webkit,FF)</summary>
        try {
            console.profileEnd(name);
        } catch (e) {
            /* Do nothing */
        }
    };
    
    cswPublic.time = function (name) {
        /// <summary>Begins a named timer in the console log(Webkit,FF)</summary>
        try {
            console.time(name);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.timeEnd = function (name) {
        /// <summary>Ends a named timer in the console log(Webkit,FF)</summary>
        try {
            console.timeEnd(name);
        } catch (e) {
            /* Do nothing */
        }
    };
    cswPublic.trace = function () {
        /// <summary>Dumps the stack to the console log(Webkit,FF)</summary>
        try {
            console.trace();;
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.warn = function (msg) {
        /// <summary>Outputs an warning message to the console log(Webkit,FF)</summary>
        try {
            console.warn(msg);
        } catch (e) {
            Csw.debug.log(msg);
        }
    };

    Csw.debug = Csw.debug ||
        Csw.register('debug', cswPublic);

} ());
