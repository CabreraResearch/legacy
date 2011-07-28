///// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
///// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
///// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
///// <reference path="_Global.js" />

//#region CswClientDb
function CswClientDb()
{
    //private
    var storedInMemory = {};
    var keys = [];
    
    var serializer = JSON;
    var serialize = serializer.stringify;
    var deserialize = $.parseJSON;

    //priveleged, public
    this.clear = function() {
        //nuke the entire storage collection
        localStorage.clear();
        sessionStorage.clear();
        storedInMemory = {};
        return this;
    };

    this.getItem = function(key)
    {
        var ret = '';
        if (!isNullOrEmpty(key))
        {
            var value = tryParseString(localStorage.getItem(key), '');
            if (isNullOrEmpty(value) || value === 'undefined')
            {
                value = tryParseString(sessionStorage.getItem(key), '');
            }
            if (isNullOrEmpty(value) || value === 'undefined')
            {
                value = tryParseString( storedInMemory[key], '');
            }
            if (!isNullOrEmpty(value) && value !== 'undefined')
            {
                try
                {
                    ret = deserialize(value);
                }
                catch(e)
                {
                    ret = value;
                }
            }
        }
        return ret;
    };

    this.getKeys = function ()
    {
        if (isNullOrEmpty(keys) && localStorage.length > 0)
        {
            for (var key in localStorage)
            {
                keys.push(key);
            }
            if (sessionStorage.length > 0)
            {
                for (var key in sessionStorage)
                {
                    keys.push(key);
                }
            }
            if (storedInMemory.length > 0)
            {
                for (var key in storedInMemory)
                {
                    keys.push(key);
                }
            }
        }
        return keys;
    };

    this.hasKey = function(key)
    {
        var ret = (this.getKeys().indexOf(key) !== -1);
        return ret;
    };

    this.removeItem = function(key)
    {
        localStorage.removeItem(key);
        sessionStorage.removeItem(key);
        delete storedInMemory[key];
        delete keys[key];
    };

    this.setItem = function (key, value)
    {
        /// <summary>
        ///   Stores a key/value pair in localStorage. 
        ///   If localStorage is full, use sessionStorage. 
        ///   if sessionStorage is full, store in memory.
        /// </summary>
        /// <param name="key" type="String">The property name to store.</param>
        /// <param name="value" type="String">The property value to store. If not a string, serializer will be called.</param>
        /// <returns type="Boolean">True if successful</returns>
        var ret = true;
        if (!isNullOrEmpty(key))
        {
            if (!this.hasKey(key))
            {
                keys.push(key);
            }
            var val = (typeof value === 'object') ? serialize(value) : value;

            // if localStorage is full, we should fail gracefully into sessionStorage, then memory
            try {
                localStorage.setItem(key, val);
            }
            catch (locErr) {
                try {
                    if (debugOn()) {
                        log('localStorage failed:' + locErr);
                    }
                    localStorage.removeItem(key);
                    sessionStorage.setItem(key, val);
                } catch (ssnErr) {
                    if (debugOn()) {
                        log('sessionStorage failed:' + ssnErr);
                    }
                    try {
                        sessionStorage.removeItem(key);
                        storedInMemory[key] = value;
                    }
                    catch (memErr) {
                        if (debugOn()) {
                            log('memory storage failed:' + memErr);
                        }
                        ret = false;
                    }
                }
            }
        }
        return ret;
    };
}

//#endregion CswClientDb
