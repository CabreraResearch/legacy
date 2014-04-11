/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    var method;
    var noop = function () { };
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeStamp', 'trace', 'warn'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }


    var cswPrivate = {
        prepMsg: function (msg) {
            var ret = {};
            var data = msg || {};

            if (false === Csw.isPlainObject(data)) {
                data = { message: Csw.string(data) };
            }

            ret.customerid = ret.customerid || Csw.clientSession.currentAccessId();
            ret.username = ret.username || Csw.clientSession.currentUserName();
            ret.sessionid = ret.sessionid || Csw.clientSession.currentSessionId();
            ret.server = ret.server || Csw.clientSession.currentServer();
            ret.data = data;
            return ret;
        },
        logLevels: ['info', 'performance', 'warn', 'error', 'none']
    };

    var cswPublic = {
    };

    cswPrivate.isLogLevelSupported = function (requestLevel) {
        var maxLevel = 'error';//maybe this should be configurable in some way?
        return cswPrivate.logLevels.indexOf(maxLevel) <= cswPrivate.logLevels.indexOf(requestLevel);
    };

    cswPublic.showExceptions = function () {
        return (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('error'));
    };

    cswPublic.logLevels = function () {
        return cswPrivate.logLevels.slice(0);
    };

    cswPublic.assert = function (truth, msg) {
        /// <summary>Evaluates the truthiness of truth and throws an exception with msg val if false.</summary>
        if (Csw.clientSession.isDebug()) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    console.assert(truth, msg);
                }
            );
        }
    };

    cswPublic.count = function (msg) {
        /// <summary>Displays a count of the number of time the msg has been met in the console log(Webkit,FF).</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    console.count(msg);
                }
            );
        }
    };

    cswPrivate.tryExecSwallow = function (toConsole, onFail) {

        try {
            toConsole();
        } catch (e) {
            try {
                onFail();
            } catch (e) {

            }
        }
    };

    cswPublic.error = function (msg) {
        /// <summary>Outputs an error message to the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('error')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    console.error(msg);
                },
                function onFail() {
                    Csw.debug.log(msg);
                }
            );
        }
    };

    cswPublic.group = function (name) {
        /// <summary>Begins a named message group in console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    name = name || '';
                    console.group(name);
                }
            );
        }
    };

    cswPublic.groupCollapsed = function (name) {
        /// <summary>Creates a named, collapsed message group in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    name = name || '';
                    console.groupCollapsed(name);
                }
            );
        }
    };

    cswPublic.groupEnd = function (name) {
        /// <summary>Ends a named message group in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    name = name || '';
                    console.groupEnd(name);
                }
            );
        }
    };

    cswPublic.perf = function (msg) {
        /// <summary>Outputs an info message to the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    console.info(msg);
                },
                function onFail() {
                    Csw.debug.log(msg);
                }
            );
        }
    };

    cswPublic.log = function (msg) {
        /// <summary>Outputs an unstyled message to the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('info')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    console.log(msg);
                },
                function onFail() {
                    window.dump(msg);
                }
            );
        }
    };

    cswPublic.profile = function (name) {
        /// <summary>Beginds a named profile in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    name = name || '';
                    console.profile(name);
                }
            );
        }
    };

    cswPublic.profileEnd = function (name) {
        /// <summary>Ends a named profile in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    name = name || '';
                    console.profileEnd(name);
                }
            );
        }
    };

    cswPublic.time = function (name) {
        /// <summary>Begins a named timer in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    name = name || '';
                    console.time(name);
                }
            );
        }
    };

    cswPublic.timeEnd = function (name) {
        /// <summary>Ends a named timer in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    name = name || '';
                    console.timeEnd(name);
                }
            );
        }
    };
    cswPublic.trace = function () {
        /// <summary>Dumps the stack to the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    console.trace();
                }
            );
        }
    };

    cswPublic.warn = function (msg) {
        /// <summary>Outputs an warning message to the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('warn')) {
            cswPrivate.tryExecSwallow(
                function toConsole() {
                    console.warn(msg);
                },
                function onFail() {
                    Csw.debug.log(msg);
                }
            );
        }
    };


    Csw.register('debug', cswPublic);

}());
