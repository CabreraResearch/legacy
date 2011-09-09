/// <reference path="CswEnums.js" />
/// <reference path="CswGlobalTools.js" />

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

Object.defineProperty(
    Object.prototype, 
    'renameProperty',
    {
        writable : false, // Cannot alter this property
        enumerable : false, // Will show up in a for-in loop.
        configurable : false, // Cannot be deleted via the delete operator
        value : function (oldName, newName) {
            if (this.hasOwnProperty(oldName)) {
                this[newName] = this[oldName];
                delete this[oldName];
            }
            return this;
        }
    }
);

//#endregion Object