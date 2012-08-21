/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function _cswObject() {
    'use strict';

    function isPlainObject(obj) {
        /// <summary>
        ///    Returns true if the object is a JavaScript object.
        ///     &#10; isPlainObject(Csw.enums.inputTypes) === true
        ///     &#10; isPlainObject('Csw.enums.inputTypes') === false
        /// </summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = ($.isPlainObject(obj));
        return ret;
    }
    Csw.register('isPlainObject', isPlainObject);
    Csw.isPlainObject = Csw.isPlainObject || isPlainObject;

    function isJQuery(obj) {
        /// <summary> Returns true if the object jQuery</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (obj instanceof jQuery);
        return ret;
    }
    Csw.register('isJQuery', isJQuery);
    Csw.isJQuery = Csw.isJQuery || isJQuery;

    function hasLength(obj) {
        /// <summary> Returns true if the object is an Array or jQuery</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (Csw.isArray(obj) || isJQuery(obj));
        return ret;
    }
    Csw.register('hasLength', hasLength);
    Csw.hasLength = Csw.hasLength || hasLength;

    function isGeneric(obj) {
        /// <summary> Returns true if the object is not a function, array, jQuery or JSON object</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (false === Csw.isFunction(obj) && false === hasLength(obj) && false === isPlainObject(obj));
        return ret;
    }
    Csw.register('isGeneric', isGeneric);
    Csw.isGeneric = Csw.isGeneric || isGeneric;

    function isNullOrUndefined(obj) {
        /// <summary> Returns true if the input is null or undefined</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />

        /* Don't attempt to evaluate validity of functions--they're not null */
        var ret = (false === Csw.isFunction(obj));

        if (ret) {
            /* Not a function */
            ret = (obj === null ||
                  obj === undefined ||
                  ( /* Plain object is an object literal: {} */
                   $.isPlainObject(obj) && (
                      $.isEmptyObject(obj)) ||
                      false === obj.isValid)
            );
        }
        return ret;
    }
    Csw.register('isNullOrUndefined', isNullOrUndefined);
    Csw.isNullOrUndefined = Csw.isNullOrUndefined || isNullOrUndefined;

    function isTrueOrFalse(obj) {
        /// <summary> Returns true if a value can be evaluated as true or false.</summary>
        /// <returns type="Boolean" />
        return (
            obj === true ||
            obj === false ||
            obj === 1 ||
            obj === 0 ||
            obj === 'true' ||
            obj === 'false'
        );
    }
    Csw.register('isTrueOrFalse', isTrueOrFalse);
    Csw.isTrueOrFalse = Csw.isTrueOrFalse || isTrueOrFalse;

    function isNullOrEmpty(obj, checkLength) {
        /// <summary> Returns true if the input is null, undefined, or ''</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = true;

        /* if(obj) is faster, but it coerces type. We also have to check if obj is a truthy value. */
        if (obj || isTrueOrFalse(obj)) {
            ret = isNullOrUndefined(obj);
            if (false === ret && isGeneric(obj)) {
                ret = ((Csw.isString(obj) && obj.trim() === '') || (Csw.isDate(obj) && obj === Csw.dateTimeMinValue) || (Csw.isNumber(obj) && obj === Csw.int32MinVal));
            } else if (checkLength && hasLength(obj)) {
                ret = (obj.length === 0);
            }
        }
        return ret;
    }
    Csw.register('isNullOrEmpty', isNullOrEmpty);
    Csw.isNullOrEmpty = Csw.isNullOrEmpty || isNullOrEmpty;

    function isInstanceOf(name, obj) {
        return (Csw.contains(name, obj) && Csw.bool(obj[name]));
    }
    Csw.register('isInstanceOf', isInstanceOf);
    Csw.isInstanceOf = Csw.isInstanceOf || isInstanceOf;



    function tryParseObjByIdx(object, index, defaultStr) {
        /// <summary> Attempts to fetch the value at an array index. Null-safe.</summary>
        /// <param name="object" type="Object"> Object or array to parse </param>
        /// <param name="index" type="String"> Index or property to find </param>
        /// <param name="defaultStr" type="String"> Optional. String to use instead of '' if index does not exist. </param>
        /// <returns type="String">Parsed string</returns>
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
    }
    Csw.register('tryParseObjByIdx', tryParseObjByIdx);
    Csw.tryParseObjByIdx = Csw.tryParseObjByIdx || tryParseObjByIdx;

    function contains(object, index) {
        /// <summary>Determines whether an object or an array contains a property or index. Null-safe.</summary>
        /// <param name="object" type="Object"> An object to evaluate </param>
        /// <param name="index" type="String"> An index or property to find </param>
        /// <returns type="Boolean" />
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
    }
    Csw.register('contains', contains);
    Csw.contains = Csw.contains || contains;

    function renameProperty(obj, oldName, newName) {
        /// <summary>Renames a property on a Object literal</summary>
        /// <param name="obj" type="Object"> Object containing property </param>
        /// <param name="oldName" type="String"> Current property name </param>
        /// <param name="newName" type="String"> New property name </param>
        /// <returns type="Object"> The modified object </returns>
        if (false === Csw.isNullOrUndefined(obj) && contains(obj, oldName)) {
            obj[newName] = obj[oldName];
            delete obj[oldName];
        }
        return obj;
    }
    Csw.register('renameProperty', renameProperty);
    Csw.renameProperty = Csw.renameProperty || renameProperty;

    function foundMatch(anObj, prop, value) {
        /// <summary>Determines whether a prop/value match can be found on a property</summary>
        /// <param name="anObj" type="Object"> Object containing property </param>
        /// <param name="prop" type="String"> Property name </param>
        /// <param name="value" type="Object"> Value to match </param>
        /// <returns type="Boolean"> True if succeeded </returns>
        var ret = false;
        if (false === Csw.isNullOrEmpty(anObj) && contains(anObj, prop) && anObj[prop] === value) {
            ret = true;
        }
        return ret;
    }
    Csw.register('foundMatch', foundMatch);
    Csw.foundMatch = Csw.foundMatch || foundMatch;

    function each(thisObj, onSuccess) {
        /// <summary>Iterates an Object or an Array and handles length property</summary>
        /// <param name="thisObj" type="Object"> An object to crawl </param>
        /// <param name="onSuccess" type="Function"> A function to execute on finding a property, which should return true to stop.
        ///<para>if an Array, onSuccess receives (value, key)</para> 										 
        ///<para>if an Object, onSuccess receives (thisObject, name, parentObject)</para>
        ///</param>
        /// <returns type="Object">Returns the return of onSuccess</returns>
        //http://stackoverflow.com/questions/7356835/jquery-each-fumbles-if-non-array-object-has-length-property
        var ret = false,
            childKey, obj, childObj;
        if (Csw.isFunction(onSuccess)) {
            if (Csw.isArray(thisObj) || (Csw.isPlainObject(thisObj) && false === contains(thisObj, 'length'))) {
                $.each(thisObj, function (key, value) {
                    obj = thisObj[key];
                    ret = onSuccess(obj, key, thisObj, value);
                    return !ret; //false signals break
                });
            } else if (Csw.isPlainObject(thisObj)) {
                for (childKey in thisObj) {
                    if (contains(thisObj, childKey)) {
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
    } // each()
    Csw.register('each', each);
    Csw.each = Csw.each || each;

    function crawlObject(thisObj, onSuccess, doRecursion) {
        /// <summary>Iterates (optionally recursively) an object and executes a function on each of its properties.</summary>
        /// <param name="thisObj" type="Object"> An object to crawl </param>
        /// <param name="onSuccess" type="Function"> A function to execute on finding a property. To force iteration to stop, onSuccess should return false. </param>
        /// <param name="doRecursion" type="Boolean"> If true, recurse on all properties. Recursion will stop if onSuccess returns false. </param>
        /// <returns type="Object">Returns the return of onSuccess</returns>
        //borrowed from http://code.google.com/p/shadejs
        var stopCrawling = false;
        var onEach = function (childObj, childKey, parentObj, value) {
            if (false === stopCrawling) {
                stopCrawling = Csw.bool(onSuccess(childObj, childKey, parentObj, value));
            }
            if (false === stopCrawling && doRecursion) {
                stopCrawling = Csw.bool(crawlObject(childObj, onSuccess, doRecursion));
            }
            return stopCrawling;
        };
        stopCrawling = each(thisObj, onEach);
        return stopCrawling;
    }
    Csw.register('crawlObject', crawlObject);
    Csw.crawlObject = Csw.crawlObject || crawlObject;

    function object(obj) {
        /// <summary>Find an object in a JSON object.</summary>
        /// <param name="obj" type="Object"> Object to search </param>
        /// <returns type="Object"></returns>
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
            var ret = false;
            if (foundMatch(thisObj, key, value)) {
                ret = thisObj;
                currentObj = ret;
                parentObj = ret;
                currentKey = key;
            }
            if (false === ret) {
                var onSuccess = function (childObj, childKey, parObj) {
                    var found = false;
                    if (foundMatch(childObj, key, value)) {
                        ret = childObj;
                        currentObj = ret;
                        parentObj = parObj;
                        currentKey = childKey;
                        found = true;
                    }
                    return found;
                };
                crawlObject(thisObj, onSuccess, true);
            }
            return ret;
        }

        function remove(key, value) {
            var onSuccess = function (childObj, childKey, parObj) {
                var deleted = false;
                if (foundMatch(childObj, key, value)) {
                    deleted = true;
                    delete parObj[childKey];
                    currentKey = null;
                    currentObj = null;
                    //parentObj = parentObj;
                }
                return deleted;
            };
            return crawlObject(thisObj, onSuccess, true);
        }

        return {
            find: find,
            remove: remove,
            obj: thisObj,
            parentObj: parentObj,
            currentObj: currentObj,
            currentKey: currentObj
        };
    }
    Csw.register('object', object);
    Csw.object = Csw.object || object;

    Csw.clone = Csw.clone ||
        Csw.register('clone', function (data) {
            /// <summary>Get a dereferenced copy of an object</summary>
            /// <param name="data" type="Object"> An object </param>
            /// <returns type="Object"> An identical copy of the object. </returns>
            return JSON.parse(JSON.stringify(data));
        });

    Csw.serialize = Csw.serialize ||
        Csw.register('serialize', function (data) {
            /// <summary>Convert an object to a string</summary>
            /// <param name="data" type="Object"> An object </param>
            /// <returns type="String"> A string representation of the object. </returns>
            var ret = '';
            Csw.tryExec(function() { ret = JSON.stringify(data); });
            return ret;
        });

    Csw.deserialize = Csw.deserialize ||
        Csw.register('deserialize', function (data) {
            /// <summary>Convert a string to a native JS Object</summary>
            /// <param name="data" type="String"> A string representation of an object </param>
            /// <returns type="Object"> An object. </returns>
            var ret = {};
            Csw.tryExec(function () { ret = $.parseJSON(data); });
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

} ());
