
/// <reference path="~app/CswApp-vsdoc.js" />

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

    Csw.tryParseObjByIdx = Csw.tryParseObjByIdx ||
        Csw.register('tryParseObjByIdx', function (object, index, defaultStr) {
            /// <summary> Attempts to fetch the value at an array index. Null-safe.</summary>
            /// <param name="object" type="Object"> Object or array to parse </param>
            /// <param name="index" type="String"> Index or property to find </param>
            /// <param name="defaultStr" type="String"> Optional. String to use instead of '' if index does not exist. </param>
            /// <returns type="String">Parsed string</returns>
            'use strict';
            var ret = '';
            if (false === Csw.isNullOrEmpty(defaultStr)) {
                ret = defaultStr;
            }
            if (Csw.contains(object, index)) {
                if (false === Csw.isNullOrUndefined(object[index])) {
                    ret = object[index];
                }
            }
            return ret;
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

    Csw.renameProperty = Csw.renameProperty ||
        Csw.register('renameProperty', function (obj, oldName, newName) {
            /// <summary>Renames a property on a Object literal</summary>
            /// <param name="obj" type="Object"> Object containing property </param>
            /// <param name="oldName" type="String"> Current property name </param>
            /// <param name="newName" type="String"> New property name </param>
            /// <returns type="Object"> The modified object </returns>
            'use strict';
            if (false === Csw.isNullOrUndefined(obj) && Csw.contains(obj, oldName)) {
                obj[newName] = obj[oldName];
                delete obj[oldName];
            }
            return obj;
        });

    Csw.foundMatch = Csw.foundMatch ||
        Csw.register('foundMatch', function (anObj, prop, value) {
            /// <summary>Determines whether a prop/value match can be found on a property</summary>
            /// <param name="anObj" type="Object"> Object containing property </param>
            /// <param name="prop" type="String"> Property name </param>
            /// <param name="value" type="Object"> Value to match </param>
            /// <returns type="Boolean"> True if succeeded </returns>
            'use strict';
            var ret = false;
            if (false === Csw.isNullOrEmpty(anObj) && Csw.contains(anObj, prop) && anObj[prop] === value) {
                ret = true;
            }
            return ret;
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
                else if(Csw.isPlainObject(thisObj) && false === Csw.contains(thisObj, 'length')) {
                    window.$.each(thisObj, function(key, value) {
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

    Csw.crawlObject = Csw.crawlObject ||
        Csw.register('crawlObject', function (thisObj, onSuccess, doRecursion) {
            /// <summary>Iterates (optionally recursively) an object and executes a function on each of its properties.</summary>
            /// <param name="thisObj" type="Object"> An object to crawl </param>
            /// <param name="onSuccess" type="Function"> A function to execute on finding a property. To force iteration to stop, onSuccess should return false. </param>
            /// <param name="doRecursion" type="Boolean"> If true, recurse on all properties. Recursion will stop if onSuccess returns false. </param>
            /// <returns type="Object">Returns the return of onSuccess</returns>
            'use strict';
            //borrowed from http://code.google.com/p/shadejs
            var stopCrawling = false;
            var onEach = function(childObj, childKey, parentObj, value) {
                if (false === stopCrawling) {
                    stopCrawling = Csw.bool(onSuccess(childObj, childKey, parentObj, value));
                }
                if (false === stopCrawling && doRecursion) {
                    stopCrawling = Csw.bool(Csw.crawlObject(childObj, onSuccess, doRecursion));
                }
                return stopCrawling;
            };
            stopCrawling = Csw.each(thisObj, onEach);
            return stopCrawling;
        });

    Csw.object = Csw.object ||
        Csw.register('object', function (obj) {
            /// <summary>Find an object in a JSON object.</summary>
            /// <param name="obj" type="Object"> Object to search </param>
            /// <returns type="Object"></returns>
            'use strict';
            var thisObj = obj || {};
            var currentObj = null;
            var parentObj = thisObj;
            var currentKey;
            //var parentKey = null;

            function find(key, value) {
                /// <summary>Find a property's parent</summary>
                /// <param name="key" type="String"> Property name to match. </param>
                /// <param name="value" type="Object"> Property value to match </param>
                /// <returns type="Object"> Returns the value of the 'property' property which contains a matching key/value prop. </returns>
                'use strict';
                var ret = false;
                if (Csw.foundMatch(thisObj, key, value)) {
                    ret = thisObj;
                    currentObj = ret;
                    parentObj = ret;
                    currentKey = key;
                }
                if (false === ret) {
                    var onSuccess = function(childObj, childKey, parObj) {
                        var found = false;
                        if (Csw.foundMatch(childObj, key, value)) {
                            ret = childObj;
                            currentObj = ret;
                            parentObj = parObj;
                            currentKey = childKey;
                            found = true;
                        }
                        return found;
                    };
                    Csw.crawlObject(thisObj, onSuccess, true);
                }
                return ret;
            }

            function remove(key, value) {
                'use strict';
                var onSuccess = function (childObj, childKey, parObj) {
                    var deleted = false;
                    if (Csw.foundMatch(childObj, key, value)) {
                        deleted = true;
                        delete parObj[childKey];
                        currentKey = null;
                        currentObj = null;
                        //parentObj = parentObj;
                    }
                    return deleted;
                };
                return Csw.crawlObject(thisObj, onSuccess, true);
            }

            return {
                find: find,
                remove: remove,
                obj: thisObj,
                parentObj: parentObj,
                currentObj: currentObj,
                currentKey: currentObj
            };
        });

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
            Csw.tryExec(function() { ret = JSON.stringify(data); });
            return ret;
        });

    Csw.deserialize = Csw.deserialize ||
        Csw.register('deserialize', function (data) {
            /// <summary>Convert a string to a native JS Object</summary>
            /// <param name="data" type="String"> A string representation of an object </param>
            /// <returns type="Object"> An object. </returns>
            'use strict';
            var ret = {};
            Csw.tryExec(function () { ret = window.$.parseJSON(data); });
            if(Csw.isNullOrEmpty(ret)) {
				ret = {};
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
                Csw.tryExec(function() { ret = $.param(data); });
            } else {
                Csw.each(data, function(val, key) {
                    if(ret.length > 0) {
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
            if(arguments.length === 3) {
                ret = window.$.extend(Csw.bool(deepCopy), ret, srcObj);
            } else {
                ret = window.$.extend(ret, srcObj);
            }
            return ret;
        });

} ());
