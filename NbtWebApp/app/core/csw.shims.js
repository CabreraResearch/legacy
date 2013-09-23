/// <reference path="../release/nsApp-vsdoc.js" />
/*global Csw:true,window:true*/
(function () {
    'use strict';

    var onError = window.onerror;
    /**
     * Log errors to the console
    */
    window.onerror = function (msg, url, lineNumber) {

        console.warn('%s\r url: %s\r line: %d', msg, Csw.to.string(url), lineNumber);
        if (onError) {
            onError.apply(this, arguments);
        }
        return false; //true means don't propogate the error
    };

    if (!window.setImmediate) {
        /**
         * Shim for setImmediate
        */
        window.setImmediate = function (func, args) {
            return window.setTimeout(func, 0, args);
        };
        window.clearImmediate = window.clearTimeout;
    }

    if (!Function.prototype.inheritsFrom) {
        Object.defineProperties(Function.prototype, {
            inheritsFrom: {
                value:
                    /**
                     * Easy inheritance by prototype
                    */
                    function inheritsFrom(parentClassOrObject) {
                        if (parentClassOrObject.constructor === Function) {
                            //Normal Inheritance
                            this.prototype = new parentClassOrObject;
                            this.prototype.constructor = this;
                            this.prototype.parent = parentClassOrObject.prototype;
                        } else {
                            //Pure Virtual Inheritance
                            this.prototype = parentClassOrObject;
                            this.prototype.constructor = this;
                            this.prototype.parent = parentClassOrObject;
                        }
                        return this;
                    }
            }
        });
    }

}(window.$nameSpace$));
