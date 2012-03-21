
//global, ChemSW namespace. Eventually home to all 'global' variables.
var ChemSW = ChemSW || (function(undefined) {
    "use strict";
    return {
        constants: {
            unknownEnum: 'unknown'    
        },
        enums: {
            tryParse: function (cswEnum, enumMember, caseSensitive) {
                var ret = ChemSW.constants.unknownEnum;
                if(contains(cswEnum, enumMember)) {
                    ret = cswEnum[enumMember];
                } 
                else if (false === caseSensitive) {
                    each(cswEnum, function(member) {
                        if(contains(cswEnum, member) && 
                            tryParseString(member).toLowerCase() === tryParseString(enumMember).toLowerCase() ) {
                            ret = member;
                        }
                    });
                }
                return ret;
            },
            EditMode: {
                Edit: 'Edit',
                AddInPopup: 'AddInPopup',
                EditInPopup: 'EditInPopup',
                Demo: 'Demo',
                PrintReport: 'PrintReport',
                DefaultValue: 'DefaultValue',
                AuditHistoryInPopup: 'AuditHistoryInPopup',
                Preview: 'Preview' 
            },
            ErrorType: {
                warning: {
                    name: 'warning',
                    cssclass: 'CswErrorMessage_Warning'
                },
                error: {
                    name: 'error',
                    cssclass: 'CswErrorMessage_Error'
                }
            },
            Events: {
                CswNodeDelete: 'CswNodeDelete'
            },
            CswInspectionDesign_WizardSteps: {
                step1: { step: 1, description: 'Select an Inspection Target' },
                step2: { step: 2, description: 'Select an Inspection Design' },
                step3: { step: 3, description: 'Upload Template' },
                step4: { step: 4, description: 'Review Inspection Design' },
                //step5: { step: 5, description: 'Create Inspection Schedules' },
                step5: { step: 5, description: 'Finish' },
                stepcount: 5
            },
            CswScheduledRulesGrid_WizardSteps: {
                step1: { step: 1, description: 'Select a Customer ID' },
                step2: { step: 2, description: 'Review the Scheduled Rules' },
                stepcount: 2
            },
            CswDialogButtons: {
                1: 'ok',
                2: 'ok/cancel',
                3: 'yes/no'
            }
        },
        ajax: {
             
        },
        tools: {
            getMaxValueForPrecision: function (precision, maxPrecision) {
                var i, 
                    ret = '',
                    precisionMax = maxPrecision || 6;
                if (precision > 0 && 
                    precision <= precisionMax) {
                    
                    ret += '.';
                    for(i=0; i < precision; i += 1) {
                        ret += '9';
                    }
                }
                return ret;
            },
            loadResource: function(filename, filetype, useJquery) {
                var fileref, type = filetype || 'js';
                switch (type) {
                    case 'js':
                        if (jQuery && (($.browser.msie && $.browser.version <= 8) || useJquery)) {
                            $.ajax({
                                url: '/NbtWebApp/' + filename,
                                dataType: 'script'
                            });
                        } else {
                            fileref = document.createElement('script');
                            fileref.setAttribute("type", "text/javascript");
                            fileref.setAttribute("src", filename);
                        }
                        break;
                    case 'css':
                        fileref = document.createElement("link");
                        fileref.setAttribute("rel", "stylesheet");
                        fileref.setAttribute("type", "text/css");
                        fileref.setAttribute("href", filename);
                        break;
                }
                if (fileref) {
                    document.getElementsByTagName("head")[0].appendChild(fileref);
                }
            },
            tryExecMethod: function (func) {
                if(isFunction(func)) {
                    func.apply(this, arguments);
                }
            }


        },
        makeSequentialArray: function(start, end) {
            var ret = [],
                i;
            end = +end;
            if (isNumber(start) && 
                    isNumber(end)) {
                for (i = +start; i <= end; i += 1) {
                    ret.push(i);
                }
            }
            return ret;
        },
        makeClientSideError: function(errorType, friendlyMsg, esotericMsg) {
            return {
                type: tryParseString(errorType, ChemSW.enums.ErrorType.warning.name),
                message: tryParseString(friendlyMsg),
                detail: tryParseString(esotericMsg)
            };
        }
    };
}());


function isArray(obj) {
    "use strict";
    /// <summary> Returns true if the object is an array</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = ($.isArray(obj));
    return ret;
}

function isJQuery(obj) {
    "use strict";
    var ret = (obj instanceof jQuery);
    return ret;
}

function hasLength(obj) {
    "use strict";
    /// <summary> Returns true if the object is an Array or jQuery</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = (isArray(obj) || isJQuery(obj));
    return ret;
}

var dateTimeMinValue = new Date('1/1/0001 12:00:00 AM');
function isDate(obj) {
    "use strict";
    /// <summary> Returns true if the object is a Date</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = (obj instanceof Date);
    return ret;
}

function isNumber(obj) {
    "use strict";
    /// <summary> Returns true if the object is typeof number</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = (typeof obj === 'number');
    return ret;
}

function isFunction(obj) {
    "use strict";
    /// <summary> Returns true if the object is a function</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = ($.isFunction(obj));
    return ret;
}

function isPlainObject(obj) {
    "use strict";
    /// <summary> 
    ///    Returns true if the object is a JavaScript object.
    ///     &#10; isPlainObject(CswInput_Types) === true 
    ///     &#10; isPlainObject('CswInput_Types') === false
    /// </summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = ($.isPlainObject(obj));
    return ret;
}

function isGeneric(obj) {
    "use strict";
    /// <summary> Returns true if the object is not a function, array, jQuery or JSON object</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = (false === isFunction(obj) && false === hasLength(obj) && false === isPlainObject(obj));
    return ret;
}

function isNullOrEmpty(obj, checkLength) {
    "use strict";
    /// <summary> Returns true if the input is null, undefined, or ''</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = isNullOrUndefined(obj);
    if (false === ret && isGeneric(obj)) {
        ret = ((trim(obj) === '') ||(isDate(obj) && obj === dateTimeMinValue) ||(isNumber(obj) && obj === Int32MinVal));
    }
    else if (checkLength && hasLength(obj)) {
        ret = (obj.length === 0);
    }
    return ret;
}

function isNullOrUndefined(obj) {
    "use strict";
    /// <summary> Returns true if the input is null or undefined</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = false;
    if (false === isFunction(obj)) {
        ret = obj === null || obj === undefined || ($.isPlainObject(obj) && $.isEmptyObject(obj));
    }
    return ret;
}

function makeId(options) {
    "use strict";
    /// <summary>
    ///   Generates an ID for DOM assignment
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.ID: Base ID string
    ///     &#10;2 - options.prefix: String prefix to prepend
    ///     &#10;3 - options.suffix: String suffix to append
    ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
    /// </param>
    /// <returns type="String">A concatenated string of provided values</returns>
    var o = {
        'ID': '',
        'prefix': '',
        'suffix': '',
        'Delimiter': '_'
    };
    if (options) $.extend(o, options);
    var elementId = o.ID;
    if (false === isNullOrEmpty(o.prefix) && false === isNullOrEmpty(elementId)) {
        elementId = o.prefix + o.Delimiter + elementId;
    }
    if (false === isNullOrEmpty(o.suffix) && false === isNullOrEmpty(elementId)) {
        elementId += o.Delimiter + o.suffix;
    }
    return elementId;
}

function makeSafeId(options) {
    "use strict";
    /// <summary>   Generates a "safe" ID for DOM assignment </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.ID: Base ID string
    ///     &#10;2 - options.prefix: String prefix to prepend
    ///     &#10;3 - options.suffix: String suffix to append
    ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
    /// </param>
    /// <returns type="String">A concatenated string of provided values</returns>
    var o = {
        'ID': '',
        'prefix': '',
        'suffix': '',
        'Delimiter': '_'
    };
    if (options) {
        $.extend(o, options);
    }
    var elementId = o.ID;
    var toReplace = [/'/gi, / /gi, /\//g];
    if (false === isNullOrEmpty(o.prefix) && false === isNullOrEmpty(elementId)) {
        elementId = o.prefix + o.Delimiter + elementId;
    }
    if (false === isNullOrEmpty(o.suffix) && false === isNullOrEmpty(elementId)) {
        elementId += o.Delimiter + o.suffix;
    }
    for (var i = 0; i < toReplace.length; i++) {
        if (toReplace.hasOwnProperty(i)) {
            if (!isNullOrEmpty(elementId)) {
                elementId = elementId.replace(toReplace[i], '');
            }
        }
    }
    return elementId;
}

function isString(obj) {
    "use strict";
    /// <summary> Returns true if the object is a String object. </summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = typeof obj === 'string';
    return ret;
}


function isNumeric(obj) {
    "use strict";
    /// <summary> Returns true if the input can be parsed as a Number </summary>
    /// <param name="str" type="Object"> String or object to test </param>
    /// <returns type="Boolean" />
    var ret = false;
    if (isNumber(obj) && false === isNullOrEmpty(obj)) {
        var num = parseInt(obj);
        if (false === isNaN(num)) {
            ret = true;
        }
    }
    return ret;
}
function isTrue(str, isTrueIfNull) {
    "use strict";
    /// <summary>
    ///   Returns true if the input is true, 'true', '1' or 1.
    ///   &#10;1 Returns false if the input is false, 'false', '0' or 0.
    ///   &#10;2 Otherwise returns false and (if debug) writes an error to the log.
    /// </summary>
    /// <param name="str" type="Object">
    ///     String or object to test
    /// </param>
    /// <returns type="Bool" />
    var ret;
    var truthy = tryParseString(str).toString().toLowerCase().trim();
    if (truthy === 'true' || truthy === '1') {
        ret = true;
    }
    else if (truthy === 'false' || truthy === '0') {
        ret = false;
    }
    else if (isTrueIfNull && isNullOrEmpty(str)) {
        ret = true;
    } else {
        ret = false;
    }
    return ret;
}
function tryParseString(inputStr, defaultStr) {
    "use strict";
    /// <summary>
    ///   Returns the inputStr if !isNullOrEmpty, else returns the defaultStr
    /// </summary>
    /// <param name="inputStr" type="String"> String to parse </param>
    /// <param name="defaultStr" type="String"> Default value if null or empty </param>
    /// <returns type="String" />
    var ret = '';
    if (false === isPlainObject(inputStr) &&
        false === isFunction(inputStr) && 
        false === isNullOrEmpty(inputStr)) {
        ret = inputStr.toString();
    } 
    else if (false === isPlainObject(defaultStr) && 
        false === isFunction(defaultStr) &&
        false === isNullOrEmpty(defaultStr)) {
        ret = defaultStr.toString();
    }
    return ret;
}
var Int32MinVal = -2147483648;
function tryParseNumber(inputNum, defaultNum) {
    "use strict";
    /// <summary>
    ///   Returns the inputNum if !NaN, else returns the defaultNum
    /// </summary>
    /// <param name="inputNum" type="Number"> String to parse to number </param>
    /// <param name="defaultNum" type="Number"> Default value if not a number </param>
    /// <returns type="Number" />
    var ret = 0;
    var tryRet = +inputNum;

    if (false === isNaN(tryRet) && tryRet !== Int32MinVal) {
        ret = tryRet;
    } else {
        tryRet = +defaultNum;
        if (false === isNaN(tryRet) && tryRet !== Int32MinVal) {
            ret = tryRet;
        }
    }
    return ret;
}
function cswIndexOf(object, property) {
    "use strict";
    /// <summary>
    ///   Because IE 8 doesn't implement indexOf on the Array prototype.
    /// </summary>
    var ret = -1,i,len = object.length;
    if (isFunction(object.indexOf)) {
        ret = object.indexOf(property);
    }
    else if (hasLength(object) && len > 0) {
        for (i = 0; i < len; i++) {
            if (object[i] === property) {
                ret = i;
                break;
            }
        }
    }
    return ret;
}
function contains(object, index) {
    "use strict";
    /// <summary>Determines whether an object or an array contains a property or index. Null-safe.</summary>
    /// <param name="object" type="Object"> An object to evaluate </param>
    /// <param name="index" type="String"> An index or property to find </param>
    /// <returns type="Boolean" />
    var ret = false;
    if (false === isNullOrUndefined(object)) {
        if (isArray(object)) {
            ret = cswIndexOf(object, index) !== -1;
        }
        if (false === ret && object.hasOwnProperty(index)) {
            ret = true;
        }
    }
    return ret;
}
function tryParseObjByIdx(object, index, defaultStr) {
    "use strict";
    /// <summary> Attempts to fetch the value at an array index. Null-safe.</summary>
    /// <param name="object" type="Object"> Object or array to parse </param>
    /// <param name="index" type="String"> Index or property to find </param>
    /// <param name="defaultStr" type="String"> Optional. String to use instead of '' if index does not exist. </param>
    /// <returns type="String">Parsed string</returns>
    var ret = '';
    if (false === isNullOrEmpty(defaultStr)) {
        ret = defaultStr;
    }
    if (contains(object, index)) {
        if (false === isNullOrUndefined(object[index])) {
            ret = object[index];
        }
    }
    return ret;
}
function tryParseElement(elementId, $context) {
    "use strict";
    /// <summary>Attempts to fetch an element from the DOM first through jQuery, then through JavaScript.</summary>
    /// <param name="elementId" type="String"> ElementId to find </param>
    /// <param name="$context" type="jQuery"> Optional context to limit the search </param>
    /// <returns type="jQuery">jQuery object, empty if no match found.</returns>
    var $ret = $('');
    if (!isNullOrEmpty(elementId)) {
        if (arguments.length == 2 && !isNullOrEmpty($context)) {
            $ret = $('#' + elementId, $context);
        } else {
            $ret = $('#' + elementId);
        }
        if ($ret.length === 0) {
            $ret = $(document.getElementById(elementId));
        }
        if ($ret.length === 0) {
            $ret = $(document.getElementsByName(elementId));
        }
    }
    return $ret;
}
function renameProperty(obj, oldName, newName) {
    "use strict";
    if (false === isNullOrUndefined(obj) && contains(obj, oldName)) {
        obj[newName] = obj[oldName];
        delete obj[oldName];
    }
    return obj;
}
function trim(str) {
    "use strict";
    /// <summary>Returns a string without left and right whitespace</summary>
    /// <param name="str" type="String"> String to parse </param>
    /// <returns type="String">Parsed string</returns>
    return $.trim(str);
}
function makeDelegate(method, options) {
    "use strict";
    /// <summary>
    /// Returns a function with the argument parameter of the value of the current instance of the object.
    /// </summary>
    /// <param name="method" type="Function"> A function to delegate. </param>
    /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
    /// <returns type="Function">A delegate function: function(options)</returns>
    return function () { method(options); };
}
function makeEventDelegate(method, options) {
    "use strict";
    /// <summary>
    /// Returns a function with the event object as the first parameter, and the current value of options as the second parameter.
    /// </summary>
    /// <param name="method" type="Function"> A function to delegate. </param>
    /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
    /// <returns type="Function">A delegate function: function(eventObj, options)</returns>
    return function (eventObj) { method(eventObj, options); };
}
function foundMatch(anObj, prop, value) {
    "use strict";
    var ret = false;
    if (false === isNullOrEmpty(anObj) &&contains(anObj, prop) &&anObj[prop] === value) {
        ret = true;
    }
    return ret;
}
function ObjectHelper(obj) {
    "use strict";
    /// <summary>Find an object in a JSON object.</summary>
    /// <param name="obj" type="Object"> Object to search </param>
    /// <returns type="ObjectHelper"></returns>
    var thisObj = obj;
    var currentObj = null;
    var parentObj = thisObj;
    var currentKey = null;
    var parentKey = null;
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
                parentObj = parentObj;
            }
            return deleted;
        };
        return crawlObject(thisObj, onSuccess, true);
    }
    this.find = find;
    this.remove = remove;
    this.obj = thisObj;
    this.parentObj = parentObj;
    this.currentObj = currentObj;
    this.currentKey = currentObj;
}
//http://stackoverflow.com/questions/7356835/jquery-each-fumbles-if-non-array-object-has-length-property
function each(thisObj, onSuccess) {
    "use strict";
    /// <summary>Iterates an Object or an Array and handles length property</summary>
    /// <param name="thisObj" type="Object"> An object to crawl </param>
    /// <param name="onSuccess" type="Function"> A function to execute on finding a property, which should return true to stop.</param>
    /// <returns type="Object">Returns the return of onSuccess</returns>
    var ret = false;
    if (isFunction(onSuccess)) {
        if (isArray(thisObj) || (isPlainObject(thisObj) && false === contains(thisObj, 'length'))) {
            $.each(thisObj, function (childKey, value) {
                var childObj = thisObj[childKey];
                ret = onSuccess(childObj, childKey, thisObj, value);
                if (ret) {
                    return false; //false signals break
                }
            });
        }
        else if (isPlainObject(thisObj)) {
            for (var childKey in thisObj) {
                if (contains(thisObj, childKey)) {
                    var childObj = thisObj[childKey];
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
//borrowed from http://code.google.com/p/shadejs
function crawlObject(thisObj, onSuccess, doRecursion) {
    "use strict";
    /// <summary>Iterates (optionally recursively) an object and executes a function on each of its properties.</summary>
    /// <param name="thisObj" type="Object"> An object to crawl </param>
    /// <param name="onSuccess" type="Function"> A function to execute on finding a property. To force iteration to stop, onSuccess should return false. </param>
    /// <param name="doRecursion" type="Boolean"> If true, recurse on all properties. Recursion will stop if onSuccess returns false. </param>
    /// <returns type="Object">Returns the return of onSuccess</returns>
    var stopCrawling = false;
    var onEach = function (childObj, childKey, parentObj, value) {
        if (false === stopCrawling) {
            stopCrawling = isTrue(onSuccess(childObj, childKey, parentObj, value));
        }
        if (false === stopCrawling && doRecursion) {
            stopCrawling = isTrue(crawlObject(childObj, onSuccess, doRecursion));
        }
        return stopCrawling;
    };
    stopCrawling = each(thisObj, onEach);
    return stopCrawling;
}
// because IE 8 doesn't support console.log unless the console is open (*duh*)
function log(s, includeCallStack) {
    "use strict";
    /// <summary>Outputs a message to the console log(Webkit,FF) or an alert(IE)</summary>
    /// <param name="s" type="String"> String to output </param>
    /// <param name="includeCallStack" type="Boolean"> If true, include the callStack </param>
    var extendedLog = '';
    if (isTrue(includeCallStack)) {
        extendedLog = console.trace();
    }
    try {
        if (!isNullOrEmpty(extendedLog))
            console.log(s, extendedLog);
        else
            console.log(s);
    } catch (e) {
        alert(s);
        if (!isNullOrEmpty(extendedLog)) alert(extendedLog);
    }
}

window.abandonHope = false;