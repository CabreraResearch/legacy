
//#region Function

Function.prototype.inheritsFrom = function(parentClassOrObject) {
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
};

//#endregion Function

//#region Object

//#endregion Object

//#region Window

//Case 24114: IE7 doesn't support web storage
if (false === Modernizr.localstorage) {
    window.localStorage = (function () {
        var storage = {};
        var keys = [];
        var length = 0;
        return {
            getItem: function (sKey) {
                var ret = null;
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
                    ret = storage[sKey];
                }
                return ret;
            },
            key: function (nKeyId) {
                var ret = null;
                if (contains(keys, nKeyId)) {
                    ret = keys[nKeyId];
                }
                return ret;
            },
            setItem: function (sKey, sValue) {
                var ret = null;
                if (false === isNullOrEmpty(sKey)) {
                    if (false === contains(storage, sKey)) {
                        keys.push(sKey);
                        length += 1;
                    }
                    storage[sKey] = sValue;
                }
                return ret;
            },
            length: length,
            removeItem: function (sKey) {
                var ret = false;
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
                    keys.splice(sKey, 1);
                    length -= 1;
                    delete storage[sKey];
                    ret = true;
                }
                return ret;
            },
            clear: function () {
                storage = {};
                keys = [];
                length = 0;
                return true;
            },
            hasOwnProperty: function (sKey) {
                return contains(storage, sKey);
            }
        };
    } ());
}
if (false === Modernizr.sessionstorage) {
    //https://developer.mozilla.org/en/DOM/Storage
//    window.sessionStorage = {
//        getItem: function (sKey) {
//            if (isNullOrEmpty(sKey) || false === contains(this, sKey)) { return null; }
//            return unescape(document.cookie.replace(new RegExp("(?:^|.*;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*((?:[^;](?!;))*[^;]?).*"), "$1"));
//        },
//        key: function (nKeyId) { return unescape(document.cookie.replace(/\s*\=(?:.(?!;))*$/, "").split(/\s*\=(?:[^;](?!;))*[^;]?;\s*/)[nKeyId]); },
//        setItem: function (sKey, sValue) {
//            if (isNullOrEmpty(sKey)) { return; }
//            var now = new Date();
//            var soon = now.setTime(now.getTime() + 90000);
//            var expires = "; expires=" + soon.toGMTString();
//            document.cookie = escape(sKey) + "=" + escape(sValue) + expires + "; path=/";
//            this.length = document.cookie.match(/\=/g).length;
//        },
//        length: 0,
//        removeItem: function (sKey) {
//            if (isNullOrEmpty(sKey) || false === contains(this, sKey)) { return; }
//            var sExpDate = new Date();
//            sExpDate.setDate(sExpDate.getDate() - 1);
//            document.cookie = escape(sKey) + "=; expires=" + sExpDate.toGMTString() + "; path=/";
//            this.length--;
//        },
//        clear: function () { },
//        hasOwnProperty: function (sKey) { return (new RegExp("(?:^|;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie); }
//    };
    //    window.sessionStorage.length = (document.cookie.match(/\=/g) || window.sessionStorage).length;
    window.sessionStorage = (function () {
        var storage = {};
        var keys = [];
        var length = 0;
        return {
            getItem: function (sKey) {
                var ret = null;
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
                    ret = storage[sKey];
                }
                return ret;
            },
            key: function (nKeyId) {
                var ret = null;
                if (contains(keys, nKeyId)) {
                    ret = keys[nKeyId];
                }
                return ret;
            },
            setItem: function (sKey, sValue) {
                var ret = null;
                if (false === isNullOrEmpty(sKey)) {
                    if (false === contains(storage, sKey)) {
                        keys.push(sKey);
                        length += 1;
                    }
                    storage[sKey] = sValue;
                }
                return ret;
            },
            length: length,
            removeItem: function (sKey) {
                var ret = false;
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
                    keys.splice(sKey, 1);
                    length -= 1;
                    delete storage[sKey];
                    ret = true;
                }
                return ret;
            },
            clear: function () {
                storage = {};
                keys = [];
                length = 0;
                return true;
            },
            hasOwnProperty: function (sKey) {
                return contains(storage, sKey);
            }
        };
    } ());
}

//#endregion Window