///// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
///// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
///// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
///// <reference path="_Global.js" />

//#region CswLocalStorage
function CswClientDb(nativeStorage, serializer, useJSON)
{
    //abstracts localStorage or sessionStorage
    this.storage = nativeStorage;
    this.useJSON = useJSON;
    this.serializer = serializer;
    if (useJSON)
    {
        this.serialize = this.serializer.stringify;
        this.deserialize = this.serializer.parse;
    }
    else
    {
        this.serialize = this.serializer.serialize;
        this.deserialize = this.serializer.deserialize;
    }

    this.keys = new Array;
}

CswClientDb.prototype = {

    clear: function ()
    {
        //only clear the storage consumed by this instance
        for (var key in this.keys)
        {
            var keyName = this.keys[key];
            this.storage.removeItem(keyName);
        }
        return (this);
    },
    purgeAllStorage: function ()
    {
        //nuke the entire storage collection
        this.storage.clear();
        return this;
    },
    getItem: function (key, defaultValue)
    {
        var ret;
        var value = this.storage.getItem(key);
        if (isNullOrEmpty(value))
        {
            ret = (!isNullOrEmpty(defaultValue)) ? defaultValue : null;
        }
        else
        {
            ret = this.deserialize(value);
        }
        return ret;
    },
    getKeys: function ()
    {
        var ret = this.keys;
        return ret;
    },
    hasItem: function (key)
    {
        ret = (!isNullOrEmpty(this.storage.getItem(key)));
        return ret;
    },
    removeItem: function (key)
    {
        this.storage.removeItem(key);
        return (this);
    },
    setItem: function (key, value)
    {
        if (this.keys.indexOf(key) === -1)
        {
            this.keys.push(key);
        }
        var val = this.serialize(value);
        this.storage.setItem(key, val);
        return (this);
    }
    //TODO: space evaluation, storage event handlers
};
//#endregion CswLocalStorage