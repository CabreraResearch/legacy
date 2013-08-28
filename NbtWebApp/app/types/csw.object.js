
/// <reference path="~/app/CswApp-vsdoc.js" />

(function _cswObject() {
    'use strict';

    Csw.isPlainObject = Csw.isPlainObject ||
        Csw.register('isPlainObject', function (obj) {
            /// <summary>
            ///    Returns true if the object is a JavaScript object.
            ///     &#10; isPlainObject(Csw.enums.inputTypes) === true
            ///     &#10; isPlainObject('Csw.enums.inputTypes') === false
            /// </summary>
            /// <param name="obj" type="Object"> Object to test</param>
            /// <returns type="Boolean" />
            'use strict';
            var ret = (window.$.isPlainObject(obj));
            return ret;
        });

    Csw.isJQuery = Csw.isJQuery ||
        Csw.register('isJQuery', function (obj) {
            /// <summary> Returns true if the object jQuery</summary>
            /// <param name="obj" type="Object"> Object to test</param>
            /// <returns type="Boolean" />
            'use strict';
            var ret = (obj instanceof window.jQuery);
            return ret;
        });

    Csw.hasLength = Csw.hasLength ||
        Csw.register('hasLength', function (obj) {
            /// <summary> Returns true if the object is an Array or jQuery</summary>
            /// <param name="obj" type="Object"> Object to test</param>
            /// <returns type="Boolean" />
            'use strict';
            var ret = (Csw.isArray(obj) || Csw.isJQuery(obj));
            return ret;
        });

    Csw.isGeneric = Csw.isGeneric ||
        Csw.register('isGeneric', function (obj) {
            /// <summary> Returns true if the object is not a function, array, jQuery or JSON object</summary>
            /// <param name="obj" type="Object"> Object to test</param>
            /// <returns type="Boolean" />
            'use strict';
            var ret = (false === Csw.isFunction(obj) && false === Csw.hasLength(obj) && false === Csw.isPlainObject(obj));
            return ret;
        });

    Csw.isNullOrUndefined = Csw.isNullOrUndefined ||
        Csw.register('isNullOrUndefined', function (obj) {
            /// <summary> Returns true if the input is null or undefined</summary>
            /// <param name="obj" type="Object"> Object to test</param>
            /// <returns type="Boolean" />
            'use strict';
            /* Don't attempt to evaluate validity of functions--they're not null */
            var ret = (false === Csw.isFunction(obj));

            if (ret) {
                /* Not a function */
                ret = (obj === null ||
                    obj === undefined ||
                    (/* Plain object is an object literal: {} */
                        window.$.isPlainObject(obj) && (
                            window.$.isEmptyObject(obj)) ||
                            false === obj.isValid)
                );
            }
            return ret;
        });

    Csw.isTrueOrFalse = Csw.isTrueOrFalse ||
        Csw.register('isTrueOrFalse', function (obj) {
            /// <summary> Returns true if a value can be evaluated as true or false.</summary>
            /// <returns type="Boolean" />
            'use strict';
            return (
                obj === true ||
                    obj === false ||
                    obj === 1 ||
                    obj === 0 ||
                    obj === 'true' ||
                    obj === 'false'
            );
        });

    Csw.isNullOrEmpty = Csw.isNullOrEmpty ||
        Csw.register('isNullOrEmpty', function (obj, checkLength) {
            /// <summary> Returns true if the input is null, undefined, or ''</summary>
            /// <param name="obj" type="Object"> Object to test</param>
            /// <returns type="Boolean" />
            'use strict';
            var ret = true;

            /* if(obj) is faster, but it coerces type. We also have to check if obj is a truthy value. */
            if (obj || Csw.isTrueOrFalse(obj)) {
                ret = Csw.isNullOrUndefined(obj);
                if (false === ret && Csw.isGeneric(obj)) {
                    ret = ((Csw.isString(obj) && obj.trim() === '') || (Csw.isDate(obj) && obj === Csw.dateTimeMinValue) || (Csw.isNumber(obj) && obj === Csw.int32MinVal));
                } else if (checkLength && Csw.hasLength(obj)) {
                    ret = (obj.length === 0);
                }
            }
            return ret;
        });

    Csw.isInstanceOf = Csw.isInstanceOf ||
        Csw.register('isInstanceOf', function (name, obj) {
            'use strict';
            return (Csw.contains(name, obj) && Csw.bool(obj[name]));
        });

    Csw.contains = Csw.contains ||
        Csw.register('contains', function (object, index) {
            /// <summary>Determines whether an object or an array contains a property or index. Null-safe.</summary>
            /// <param name="object" type="Object"> An object to evaluate </param>
            /// <param name="index" type="String"> An index or property to find </param>
            /// <returns type="Boolean" />
            'use strict';
            var ret = false;
            if (false === Csw.isNullOrUndefined(object)) {
                if (Csw.isArray(object)) {
                    ret = object.indexOf(index) !== -1;
                }
                if (false === ret && object.hasOwnProperty(index)) {
                    ret = true;
                }
            }
            return ret;
        });

    var cswPrivate = Object.create(null);
    Object.defineProperties(cswPrivate, {
        canIterate: {
            value: function (obj) {
                return (obj && (typeof obj === 'object' || Array.isArray(obj)));
            }
        }
    });

    Csw.iterate = Csw.iterate ||
        Csw.register('iterate', function iterate(obj, onSuccess, recursive) {
            /// <summary>
            /// Safely iterates an Object or an Array
            /// </summary>
            if (cswPrivate.canIterate(obj)) {
                Object.keys(obj).forEach(function (key) {
                    var val = obj[key];
                    if (onSuccess && key) {
                        var quit = onSuccess(val, key);
                        if (false === quit) {
                            return false;
                        }
                    }
                    if (true === recursive) {
                        iterate(val, onSuccess, true);
                    }
                });
            }
        });

    Csw.each = Csw.each ||
        Csw.register('each', function (thisObj, onSuccess) {
            /// <summary>Iterates an Object or an Array and handles length property</summary>
            /// <param name="thisObj" type="Object"> An object to crawl </param>
            /// <param name="onSuccess" type="Function"> A function to execute on finding a property, which should return true to stop.
            ///<para>if an Array, onSuccess receives (value, key)</para> 										 
            ///<para>if an Object, onSuccess receives (thisObject, name, parentObject)</para>
            ///</param>
            /// <returns type="Object">Returns the return of onSuccess</returns>
            'use strict';
            //http://stackoverflow.com/questions/7356835/jquery-each-fumbles-if-non-array-object-has-length-property
            var ret = false,
                childKey, obj, childObj;
            if (Csw.isFunction(onSuccess)) {
                if (Csw.isArray(thisObj)) {
                    thisObj.forEach(function (element, index, array) {
                        obj = thisObj[index];
                        ret = onSuccess(obj, index, thisObj, element);
                        return !ret; //false signals break
                    });
                }
                else if (Csw.isPlainObject(thisObj) && false === Csw.contains(thisObj, 'length')) {
                    window.$.each(thisObj, function (key, value) {
                        obj = thisObj[key];
                        ret = onSuccess(obj, key, thisObj, value);
                        return !ret; //false signals break
                    });
                } else if (Csw.isPlainObject(thisObj)) {
                    for (childKey in thisObj) {
                        if (Csw.contains(thisObj, childKey)) {
                            childObj = thisObj[childKey];
                            ret = onSuccess(childObj, childKey, thisObj);
                            if (ret) {
                                break;
                            }
                        }
                    }
                }
            }
            return ret;
        });

    Csw.eachRecursive = Csw.eachRecursive ||
        Csw.register('eachRecursive', function (thisObj, onSuccess, doRecursion) {
            /// <summary>Iterates (optionally recursively) an object and executes a function on each of its properties.</summary>
            /// <param name="thisObj" type="Object"> An object to crawl </param>
            /// <param name="onSuccess" type="Function"> A function to execute on finding a property. To force iteration to stop, onSuccess should return false. </param>
            /// <param name="doRecursion" type="Boolean"> If true, recurse on all properties. Recursion will stop if onSuccess returns false. </param>
            /// <returns type="Object">Returns the return of onSuccess</returns>
            'use strict';
            //borrowed from http://code.google.com/p/shadejs
            var stopCrawling = false;
            var onEach = function (childObj, childKey, parentObj, value) {
                if (false === stopCrawling) {
                    stopCrawling = Csw.bool(onSuccess(childObj, childKey, parentObj, value));
                }
                if (false === stopCrawling && doRecursion) {
                    stopCrawling = Csw.bool(Csw.eachRecursive(childObj, onSuccess, doRecursion));
                }
                return stopCrawling;
            };
            stopCrawling = Csw.each(thisObj, onEach);
            return stopCrawling;
        });

    /**
     * Add a property to an object
     * @param obj {Object} an Object onto which to add a property
     * @param name {String} the property name
     * @param value {Object} the value of the property. Can be any type.
     * @param writable {Boolean} [writable=true] True if the property can be modified
     * @param configurable {Boolean} [configurable=true] True if the property can be removed
     * @param enumerable {Boolean} [enumerable=true] True if the property can be enumerated and is listed in Object.keys
    */
    var property = function (obj, name, value, writable, configurable, enumerable) {
        if (!obj) {
            throw new Error('Cannot define a property without an Object.');
        }
        if (!(typeof name === 'string')) {
            throw new Error('Cannot create a property without a valid property name.');
        }

        var isWritable = (writable !== false);
        var isConfigurable = (configurable !== false);
        var isEnumerable = (enumerable !== false);

        Object.defineProperty(obj, name, {
            value: value,
            writable: isWritable,
            configurable: isConfigurable,
            enumerable: isEnumerable
        });

        return obj;
    };

    Csw.register('property', property);

    /**
     * Create an instance of Object
     * @param properties {Object} [properties={}] properties to define on the Object
     * @param inheritsFromPrototype {Prototype} [inheritsFromPrototype=null] The prototype to inherit from
    */
    var object = function (properties, inheritsFromPrototype) {

        if (!inheritsFromPrototype) {
            inheritsFromPrototype = null;
        }
        if (!properties) {
            properties = {};
        }
        var obj = Object.create(inheritsFromPrototype, properties);

        Csw.property(obj, 'add',
            /**
             * Add a property to the object and return it
            */
            function (name, val, writable, configurable, enumerable) {
                return Csw.property(obj, name, val, writable, configurable, enumerable);
            }, false, false, false);

        return obj;
    };

    Csw.register('object', object);

    /**
     * Compares two objects, serialized to strings, stripped of whitespace
    */
    var compare = function(obj1, obj2) {
        var string1 = '',
            string2 = '';
        if (obj1) {
            string1 = Csw.serialize(obj1).trim().replace(' ', '');
        }
        if (obj2) {
            string2 = Csw.serialize(obj2).trim().replace(' ', '');
        }
        return string1 === string2;
    };

    Csw.register('compare', compare);

    Csw.clone = Csw.clone ||
        Csw.register('clone', function (data) {
            /// <summary>Get a dereferenced copy of an object</summary>
            /// <param name="data" type="Object"> An object </param>
            /// <returns type="Object"> An identical copy of the object. </returns>
            'use strict';
            return JSON.parse(JSON.stringify(data));
        });

    Csw.serialize = Csw.serialize ||
        Csw.register('serialize', function (data) {
            /// <summary>Convert an object to a string</summary>
            /// <param name="data" type="Object"> An object </param>
            /// <returns type="String"> A string representation of the object. </returns>
            'use strict';
            var ret = '';
            Csw.tryExec(function () { ret = JSON.stringify(data); });
            return ret;
        });

    Csw.deserialize = Csw.deserialize ||
        Csw.register('deserialize', function (data) {
            /// <summary>Convert a string to a native JS Object</summary>
            /// <param name="data" type="String"> A string representation of an object </param>
            /// <returns type="Object"> An object. </returns>
            'use strict';
            var ret = {};
            if (data) {
                Csw.tryExec(function() { ret = window.$.parseJSON(data); });
                if (Csw.isNullOrEmpty(ret)) {
                    ret = { };
                }
            }
            return ret;
        });

    Csw.params = Csw.params ||
        Csw.register('params', function (data, delimiter) {
            /// <summary>Convert an object to delimited list of parameters (x='1'&y='2')</summary>
            /// <param name="data" type="Object"> An object </param>
            /// <returns type="String"> A parameter string. </returns>
            var ret = '';
            delimiter = delimiter || '&';
            if (delimiter === '&') {
                Csw.tryExec(function () { ret = $.param(data); });
            } else {
                Csw.each(data, function (val, key) {
                    if (ret.length > 0) {
                        ret += delimiter;
                    }
                    ret += key + '=' + val;
                });
            }
            return Csw.string(ret);
        });

    Csw.extend = Csw.extend ||
        Csw.register('extend', function (destObj, srcObj, deepCopy) {
            /// <summary>Copy one object to another</summary>
            /// <returns type="Object"> An object. </returns>
            'use strict';
            var ret = destObj || {};
            if (arguments.length === 3) {
                ret = window.$.extend(Csw.bool(deepCopy), ret, srcObj);
            } else {
                ret = window.$.extend(ret, srcObj);
            }
            return ret;
        });



    /*
    The following two methods are to make your life easier with IE9, WCF and jQueryFileUpload

    If your return data contract is something like:

    class MyContract{
       string prop1;
       string prop2;
       MyInnerClass innerClassProp;
    }

    class MyInnerClass{
       string innerProp1;
       string innerProp2;
    }

    1. prop1 and prop2 are stored in the iFrame as '<prop1>value</prop1><prop2>value2</prop2>'
    2. innerProp1 and innerProp2 are stored in the iFrame as '<innerProp1>value</innerProp1><a:innerProp2>value2</innerProp2>'

    If you want to retreive prop1, call 'Csw.getPropFromIFrame'
    If you want to retreive innerProp2, call Csw.getPropFromIFrame' and set 'isSubProp' to TRUE
    */

    Csw.getPropFromIFrame = Csw.getPropFromIFrame ||
        Csw.register('getPropFromIFrame', function(iFrameObj, propName, isSubProp) {
            var tagName = propName;
            if (isSubProp) {
                tagName = 'a:' + propName;
            }
            
            return $(iFrameObj.children()[0].getElementsByTagName(tagName)[0]).text();
        });

    var dialogsCount = 0;
    Csw.dialogsCount = Csw.dialogsCount ||
        Csw.register('dialogsCount', function (num) {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="num">Number to add/subtract from the amount of open dialogs (optional)</param>
            /// <returns type="">The number of open dialogs</returns>
            if (num) {
                dialogsCount += num;
            }
            return dialogsCount;
        });

    Csw.subStrAfter = Csw.subStrAfter ||
        Csw.register('subStrAfter', function(src, substr) {
            /// <summary>
            /// Returns everything after the LAST occurence of substr
            /// </summary>
            /// <param name="src">The source string</param>
            /// <param name="substr">What to get everything after</param>
            return src.substr(src.lastIndexOf(substr) + 1);
        });

} ());
