/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';
    
    var cswPrivate = {
        prepMsg: function (msg) {
            var ret = msg;
            if (window.internetExplorerVersionNo > -1) {
                if (false === Csw.isPlainObject(msg)) {
                    ret = { message: Csw.string(msg) }; 
                } 
            }
            ret.customerid = ret.customerid || Csw.clientSession.currentAccessId();
            ret.username = ret.username || Csw.clientSession.currentUserName();
            ret.sessionid = ret.sessionid || Csw.clientSession.currentSessionId();
            ret.server = ret.server || Csw.clientSession.currentServer();
            return ret;
        },
        logLevels: ['info','performance','warn','error','none']
    };

    var cswPublic = {
        loggly: {
            info: function () { },
            perf: function () {},
            warn: function () {},
            error: function () {}
        }
    };

    cswPrivate.prepareLoggly = Csw.method(function(url) {
        cswPublic.loggly = cswPublic.loggly || { };
        cswPublic.loggly.info = new window.loggly({ url: url, level: 'log' });
        cswPublic.loggly.perf = new window.loggly({ url: url, level: 'debug' });
        cswPublic.loggly.warn = new window.loggly({ url: url, level: 'warn' });
        cswPublic.loggly.error = new window.loggly({ url: url, level: 'error' });
    });

    $(document).ready(function () {
        (function() {
            var key = Csw.clientSession.getLogglyInput();
            var host = ("https:" == document.location.protocol) ? "https://logs.loggly.com" : 'http://logs.loggly.com';
            var url = host + '/inputs/' + key + '?rt=1';
            try {
                if (loggly) {
                    cswPrivate.prepareLoggly(url);
                } else {
                    Csw.defer(function() {
                        cswPrivate.prepareLoggly(url);
                    }, 5000);
                }
            } catch(e) {
                //Swallow
            }
        }());
    });

    cswPrivate.isLogLevelSupported = function (requestLevel) {
        var maxLevel = Csw.clientSession.getLogglyLevel();
        return cswPrivate.logLevels.indexOf(maxLevel) <= cswPrivate.logLevels.indexOf(requestLevel);
    };

    cswPublic.logLevels = function() {
        return cswPrivate.logLevels.slice(0);
    };

    cswPublic.assert = function (truth, msg) {
        /// <summary>Evaluates the truthiness of truth and throws an exception with msg val if false.</summary>
        if (Csw.clientSession.isDebug()) {
            try {
                msg = cswPrivate.prepMsg(msg);
                console.assert(truth, msg);
            } catch(e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.count = function (msg) {
        /// <summary>Displays a count of the number of time the msg has been met in the console log(Webkit,FF).</summary>
        if (Csw.clientSession.isDebug()) {
            try {
                msg = cswPrivate.prepMsg(msg);
                console.count(msg);
            } catch(e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.error = function (msg) {
        /// <summary>Outputs an error message to the console log(Webkit,FF)</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
            if(Csw.clientSession.isDebug()) {
                console.error(msg);
            }
            try {
                if (cswPrivate.isLogLevelSupported('error')) {
                    msg.type = 'Error';
                    Csw.debug.loggly.error.error(Csw.serialize(msg));
                }
            } catch(e) {}
        } catch (e) {
            Csw.debug.log(msg);
        }
    };

    cswPublic.group = function (name) {
        /// <summary>Begins a named message group in console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug()) {
            try {
                name = cswPrivate.prepMsg(name);
                console.group(name);
            } catch(e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.groupCollapsed = function (name) {
        /// <summary>Creates a named, collapsed message group in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug()) {
            try {
                name = cswPrivate.prepMsg(name);
                console.groupCollapsed(name);
            } catch(e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.groupEnd = function (name) {
        /// <summary>Ends a named message group in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug()) {
            try {
                name = cswPrivate.prepMsg(name);
                console.groupEnd(name);
            } catch(e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.perf = function (msg) {
        /// <summary>Outputs an info message to the console log(Webkit,FF)</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
            if(Csw.clientSession.isDebug()) {
                console.info(msg);
            }
            try {
                if (cswPrivate.isLogLevelSupported('performance')) {
                    msg.type = 'Performance';
                    Csw.debug.loggly.perf.info(Csw.serialize(msg));
                }
            } catch(e) {}

        } catch (e) {
            Csw.debug.log(msg);
        }
    };

    cswPublic.log = function (msg) {
        /// <summary>Outputs an unstyled message to the console log(Webkit,FF)</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
            if (Csw.clientSession.isDebug()) {
                console.log(msg);
            }
            try {
                if (cswPrivate.isLogLevelSupported('info')) {
                    msg.type = 'Info';
                    Csw.debug.loggly.info.debug(Csw.serialize(msg));
                }
            } catch (e) { }
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
        if (Csw.clientSession.isDebug()) {
            try {
                if (window.internetExplorerVersionNo === -1) {
                    console.profile(name);
                }
            } catch(e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.profileEnd = function (name) {
        /// <summary>Ends a named profile in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug()) {
            try {
                if (window.internetExplorerVersionNo === -1) {
                    console.profileEnd(name);
                }
            } catch(e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.time = function (name) {
        /// <summary>Begins a named timer in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug()) {
            try {
                name = cswPrivate.prepMsg(name);
                console.time(name);
            } catch(e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.timeEnd = function (name) {
        /// <summary>Ends a named timer in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug()) {
            try {
                name = cswPrivate.prepMsg(name);
                console.timeEnd(name);
            } catch(e) {
                /* Do nothing */
            }
        }
    };
    cswPublic.trace = function () {
        /// <summary>Dumps the stack to the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug()) {
            try {
                console.trace();
                ;
            } catch(e) {
                /* Do nothing */
            }
        }
    };

    cswPublic.warn = function (msg) {
        /// <summary>Outputs an warning message to the console log(Webkit,FF)</summary>
        try {
            msg = cswPrivate.prepMsg(msg);
            if (Csw.clientSession.isDebug()) {
                console.warn(msg);
            }
            try {
                if (cswPrivate.isLogLevelSupported('warn')) {
                    msg.type = 'Warning';
                    Csw.debug.loggly.warn.warn(Csw.serialize(msg));
                }
            } catch (e) { }
        } catch (e) {
            Csw.debug.log(msg);
        }
    };

    Csw.debug = Csw.debug ||
        Csw.register('debug', cswPublic);

} ());
