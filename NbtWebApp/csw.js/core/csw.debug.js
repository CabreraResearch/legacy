/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';
    // because IE 8 doesn't support console.log unless the console is open (*duh*)
    function log(s, includeCallStack) {
        /// <summary>Outputs a message to the console log(Webkit,FF) or an alert(IE)</summary>
        /// <param name="s" type="String"> String to output </param>
        /// <param name="includeCallStack" type="Boolean"> If true, include the callStack </param>
        var extendedLog = '';
        if (Csw.bool(includeCallStack)) {
            extendedLog = console.trace();
        }
        try {
            if (false === Csw.isNullOrEmpty(extendedLog)) {
                console.log(s, extendedLog);
            } else {
                console.log(s);
            }
        } catch (e) {
            alert(s);
            if (false === Csw.isNullOrEmpty(extendedLog)) {
                alert(extendedLog);
            }
        }
    }
    Csw.register('log', log);
    Csw.log = Csw.log || log;

    function iterate(obj) {
        var str = '';
        for (var x in obj) {
            str = str + x + "=" + obj[x] + "<br><br>";
        }
        var popup = window.open("", "popup");
        if (popup !== null) {
            popup.document.write(str);
        } else {
            console.log("iterate() error: No popup!");
        }
    };
    Csw.register('iterate', iterate);
    Csw.iterate = Csw.iterate || iterate;

    function doLogging(value) {
        var ret = undefined;
        if (Csw.hasWebStorage()) {
            if (arguments.length === 1) {
                localStorage['doLogging'] = Csw.bool(value);
            }
            ret = Csw.bool(localStorage['doLogging']);
        }
        return ret;
    }
    Csw.register('doLogging', doLogging);
    Csw.doLogging = Csw.doLogging || doLogging;
    
    function debugOn(value) {
        var ret = undefined;
        if (Csw.hasWebStorage()) {
            if (arguments.length === 1) {
                localStorage['debugOn'] = Csw.bool(value);
            }
            ret = Csw.bool(localStorage['debugOn']);
        }
        return ret;
    };
    Csw.register('debugOn', debugOn);
    Csw.debugOn = Csw.debugOn || debugOn;
    
    var cacheLogInfo = function (logger) {
        if (doLogging()) {
            if (hasWebStorage()) {
                if (undefined !== logger.setEnded) {
                    logger.setEnded();
                }
                var logStorage = CswClientDb();
                var log = logStorage.getItem('debuglog');
                log += logger.toHtml();

                logStorage.setItem('debuglog', log);
            }
        }
    }
    Csw.register('cacheLogInfo', cacheLogInfo);
    Csw.cacheLogInfo = Csw.cacheLogInfo || cacheLogInfo;

    function purgeLogInfo() {
        if (hasWebStorage()) {
            window.sessionStorage.clear();
        }
    }
    Csw.register('purgeLogInfo', purgeLogInfo);
    Csw.purgeLogInfo = Csw.purgeLogInfo || purgeLogInfo;
    
}());