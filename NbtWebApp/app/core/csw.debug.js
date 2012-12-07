/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
    var cswPrivate = {
        prepMsg: function (msg) {
            var ret = {};
            var data = msg || { };
            if (window.internetExplorerVersionNo > -1) {
                if (false === Csw.isPlainObject(data)) {
                    data = { message: Csw.string(data) }; 
                } 
            }
            ret.customerid = ret.customerid || Csw.clientSession.currentAccessId();
            ret.username = ret.username || Csw.clientSession.currentUserName();
            ret.sessionid = ret.sessionid || Csw.clientSession.currentSessionId();
            ret.server = ret.server || Csw.clientSession.currentServer();
            ret.data = data;
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

    cswPublic.showExceptions = function() {
        return (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('error'));
    };

    cswPublic.logLevels = function() {
        return cswPrivate.logLevels.slice(0);
    };

    cswPublic.assert = function (truth, msg) {
        /// <summary>Evaluates the truthiness of truth and throws an exception with msg val if false.</summary>
        if (Csw.clientSession.isDebug()) {
            cswPrivate.tryExecSwallow(
                function first() {
                    console.assert(truth, msg);
                }
            );
        }
    };

    cswPublic.count = function (msg) {
        /// <summary>Displays a count of the number of time the msg has been met in the console log(Webkit,FF).</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function first() {
                    console.count(msg);
                }
            );
        }
    };

    cswPrivate.tryExecSwallow = function(first, second, third) {
        var failed = false;
        try {
            first();
        } catch(e) {
            failed = true;
        }
        try {
            second();
        } catch(e) {
            failed = true;
        }
        if (failed) {
            try {
                third();
            } catch(e) {
                
            }
        }
    };

    cswPublic.error = function (msg) {
        /// <summary>Outputs an error message to the console log(Webkit,FF)</summary>
        cswPrivate.tryExecSwallow(
            function first() {
                if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('error')) {
                    console.error(msg);
                }
            },
            function second() {
                if (cswPrivate.isLogLevelSupported('error')) {
                    msg = cswPrivate.prepMsg(msg);
                    msg.type = 'Error';
                    Csw.debug.loggly.error.error(Csw.serialize(msg));
                }
            },
            function third() {
                Csw.debug.log(msg);
            }
        );
    };

    cswPublic.group = function (name) {
        /// <summary>Begins a named message group in console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function first() {
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
                function first() {
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
                function first() {
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
                function first() {
                    console.info(msg);
                },
                function second() {
                    msg = cswPrivate.prepMsg(msg);
                    msg.type = 'Performance';
                    Csw.debug.loggly.perf.info(Csw.serialize(msg));
                },
                function third() {
                    Csw.debug.log(msg);
                }
            );
        }
    };

    cswPublic.log = function (msg) {
        /// <summary>Outputs an unstyled message to the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('info')) {
            cswPrivate.tryExecSwallow(
                function first() {
                    console.log(msg);
                },
                function second() {
                    msg = cswPrivate.prepMsg(msg);
                    msg.type = 'Info';
                    Csw.debug.loggly.info.debug(Csw.serialize(msg));
                },
                function third() {
                    window.dump(msg);
                }
            );
        }
    };

    cswPublic.profile = function (name) {
        /// <summary>Beginds a named profile in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function first() {
                    name = name || '';
                    if (window.internetExplorerVersionNo === -1) {
                        console.profile(name);
                    }
                }
            );
        }
    };

    cswPublic.profileEnd = function (name) {
        /// <summary>Ends a named profile in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function first() {
                    name = name || '';
                    if (window.internetExplorerVersionNo === -1) {
                        console.profileEnd(name);
                    }
                }
            );
        }
    };

    cswPublic.time = function (name) {
        /// <summary>Begins a named timer in the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('performance')) {
            cswPrivate.tryExecSwallow(
                function first() {
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
                function first() {
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
                function first() {
                    console.trace();
                }
            );
        }
    };

    cswPublic.warn = function (msg) {
        /// <summary>Outputs an warning message to the console log(Webkit,FF)</summary>
        if (Csw.clientSession.isDebug() || cswPrivate.isLogLevelSupported('warn')) {
            cswPrivate.tryExecSwallow(
                function first() {
                    console.warn(msg);
                },
                function second() {
                    msg = cswPrivate.prepMsg(msg);
                    msg.type = 'Warning';
                    Csw.debug.loggly.warn.warn(Csw.serialize(msg));
                },
                function third() {
                    Csw.debug.log(msg);
                }
            );
        }
    };

    Csw.debug = Csw.debug ||
        Csw.register('debug', cswPublic);

} ());
