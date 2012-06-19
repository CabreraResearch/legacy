/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';

    var cswPrivate = {
        prepMsg: function (msg) {
            var ret = msg;
            if (window.internetExplorerVersionNo > -1) {
                if (Csw.isPlainObject(msg)) {
                    ret = JSON.stringify(msg);
                } else {
                    ret = Csw.string(msg);
                }
            }
            return ret;
        }
    };

    var cswPublic = {};

    cswPublic.assert = function (truth, msg) {
        /// <summary>Evaluates the truthiness of truth and throws an exception with msg val if false.</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
            console.assert(truth, msg);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.count = function (msg) {
        /// <summary>Displays a count of the number of time the msg has been met in the console log(Webkit,FF).</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
            console.count(msg);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.error = function (msg) {
        /// <summary>Outputs an error message to the console log(Webkit,FF)</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
            console.error(msg);
        } catch (e) {
            Csw.debug.log(msg);
        }
    };

    cswPublic.group = function (name) {
        /// <summary>Begins a named message group in console log(Webkit,FF)</summary>
        try {
            name = cswPrivate.prepMsg(name);
            console.group(name);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.groupCollapsed = function (name) {
        /// <summary>Creates a named, collapsed message group in the console log(Webkit,FF)</summary>
        try {
            name = cswPrivate.prepMsg(name);
            console.groupCollapsed(name);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.groupEnd = function (name) {
        /// <summary>Ends a named message group in the console log(Webkit,FF)</summary>
        try {
            name = cswPrivate.prepMsg(name);
            console.groupEnd(name);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.info = function (msg) {
        /// <summary>Outputs an info message to the console log(Webkit,FF)</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
            console.info(msg);
        } catch (e) {
            Csw.debug.log(msg);
        }
    };

    cswPublic.log = function (msg) {
        /// <summary>Outputs an unstyled message to the console log(Webkit,FF)</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
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
            if (window.internetExplorerVersionNo === -1) {
                console.profile(name);
            }
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.profileEnd = function (name) {
        /// <summary>Ends a named profile in the console log(Webkit,FF)</summary>
        try {
            if (window.internetExplorerVersionNo === -1) {
                console.profileEnd(name);
            }
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.time = function (name) {
        /// <summary>Begins a named timer in the console log(Webkit,FF)</summary>
        try {
            name = cswPrivate.prepMsg(name);
            console.time(name);
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.timeEnd = function (name) {
        /// <summary>Ends a named timer in the console log(Webkit,FF)</summary>
        try {
            name = cswPrivate.prepMsg(name);
            console.timeEnd(name);
        } catch (e) {
            /* Do nothing */
        }
    };
    cswPublic.trace = function () {
        /// <summary>Dumps the stack to the console log(Webkit,FF)</summary>
        try {
            console.trace(); ;
        } catch (e) {
            /* Do nothing */
        }
    };

    cswPublic.warn = function (msg) {
        /// <summary>Outputs an warning message to the console log(Webkit,FF)</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
            console.warn(msg);
        } catch (e) {
            Csw.debug.log(msg);
        }
    };

    Csw.debug = Csw.debug ||
        Csw.register('debug', cswPublic);

} ());
