/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function() {
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
        var str = '',
            x, popup;
        for (x in obj) {
            if (Csw.contains(obj, x)) {
                str = str + x + '=' + obj[x] + '<br><br>';
            }
        }
        popup = window.open('', 'popup');
        if (popup !== null) {
            popup.document.write(str);
        } else {
            Csw.log('iterate() error: No popup!');
        }
    }
    Csw.register('iterate', iterate);
    Csw.iterate = Csw.iterate || iterate;

    function doLogging(value) {
        var ret = false;
        if (Csw.hasWebStorage()) {
            if (arguments.length === 1) {
                window.localStorage.doLogging = Csw.bool(value);
            }
            ret = Csw.bool(window.localStorage.doLogging);
        }
        return ret;
    }
    Csw.register('doLogging', doLogging);
    Csw.doLogging = Csw.doLogging || doLogging;

    function debugOn(value) {
        var ret = false;
        if (Csw.hasWebStorage()) {
            if (arguments.length === 1) {
                window.localStorage.debugOn = Csw.bool(value);
            }
            ret = Csw.bool(window.localStorage.debugOn);
        }
        return ret;
    }
    Csw.register('debugOn', debugOn);
    Csw.debugOn = Csw.debugOn || debugOn;

    function cacheLogInfo(logger) {
        if (doLogging()) {
            if (Csw.hasWebStorage()) {
                if (undefined !== logger.setEnded) {
                    logger.setEnded();
                }
                var log = Csw.clientDb.getItem('debuglog');
                log += logger.toHtml();

                Csw.clientDb.setItem('debuglog', log);
            }
        }
    }
    Csw.register('cacheLogInfo', cacheLogInfo);
    Csw.cacheLogInfo = Csw.cacheLogInfo || cacheLogInfo;

    function purgeLogInfo() {
        if (Csw.hasWebStorage()) {
            window.sessionStorage.clear();
        }
    }
    Csw.register('purgeLogInfo', purgeLogInfo);
    Csw.purgeLogInfo = Csw.purgeLogInfo || purgeLogInfo;

}());
