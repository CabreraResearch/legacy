/// <reference path="_Global.js" />

//#region Browser Compatibility

// for IE 8
if (typeof String.prototype.trim !== 'function')
{
    String.prototype.trim = function ()
    {
        return this.replace(/^\s+|\s+$/g, '');
    };
}

//#endregion Browser Compatibility

//#region Inheritance

Function.prototype.inheritsFrom = function(parentClassOrObject)
{
    if (parentClassOrObject.constructor === Function)
    {
        //Normal Inheritance 
        this.prototype = new parentClassOrObject;
        this.prototype.constructor = this;
        this.prototype.parent = parentClassOrObject.prototype;
    }
    else
    {
        //Pure Virtual Inheritance 
        this.prototype = parentClassOrObject;
        this.prototype.constructor = this;
        this.prototype.parent = parentClassOrObject;
    }
    return this;
};

//#endregion Inheritance