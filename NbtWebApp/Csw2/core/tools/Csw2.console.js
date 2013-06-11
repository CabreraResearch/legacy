/*global Csw2:true,window:true*/
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

    Csw2.makeSubNameSpace('console');

    Csw2.console.lift('assert', function (truth, msg) {
        window.console.assert(truth, msg);
    });

    Csw2.console.lift('count', function (msg) {
        window.console.count(msg);
    });

    Csw2.console.lift('error', function (msg) {
        window.console.error(msg);
    });

    Csw2.console.lift('group', function (name) {
        window.console.group(name);
    });

    Csw2.console.lift('groupCollapsed', function (name) {
        window.console.groupCollapsed(name);
    });

    Csw2.console.lift('groupEnd', function (name) {
        window.console.groupEnd(name);
    });

    Csw2.console.lift('info', function (msg) {
        window.console.info(msg);
    });

    Csw2.console.lift('log', function (msg) {
        window.console.log(msg);
    });

    Csw2.console.lift('profile', function (msg) {
        window.console.profile(msg);
    });

    Csw2.console.lift('profileEnd', function (msg) {
        window.console.profileEnd(msg);
    });

    Csw2.console.lift('table', function (msg) {
        window.console.table(msg);
    });

    Csw2.console.lift('time', function (msg) {
        window.console.time(msg);
    });

    Csw2.console.lift('timeEnd', function (msg) {
        window.console.timeEnd(msg);
    });

    Csw2.console.lift('trace', function (msg) {
        window.console.trace(msg);
    });

    Csw2.console.lift('warn', function (msg) {
        window.console.warn(msg);
    });

}());