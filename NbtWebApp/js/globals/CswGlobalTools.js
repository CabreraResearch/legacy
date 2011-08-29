function makeId(options)
{
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
	/// <returns type="String>A concatenated string of provided values</returns>
	var o = {
		'ID': '',
		'prefix': '',
		'suffix': '',
		'Delimiter': '_'
	};
	if (options) $.extend(o, options);

	var elementId = o.ID;

	if (!isNullOrEmpty(o.prefix) && !isNullOrEmpty(elementId))
	{
		elementId = o.prefix + o.Delimiter + elementId;
	}
	if (!isNullOrEmpty(o.suffix) && !isNullOrEmpty(elementId))
	{
		elementId += o.Delimiter + o.suffix;
	}
	return elementId;
}

function makeSafeId(options)
{
	/// <summary>
	///   Generates a "safe" ID for DOM assignment
	/// </summary>
	/// <param name="options" type="Object">
	///     A JSON Object
	///     &#10;1 - options.ID: Base ID string
	///     &#10;2 - options.prefix: String prefix to prepend
	///     &#10;3 - options.suffix: String suffix to append
	///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
	/// </param>
	/// <returns type="String>A concatenated string of provided values</returns>
	var o = {
		'ID': '',
		'prefix': '',
		'suffix': '',
		'Delimiter': '_'
	};
	if (options) $.extend(o, options);

	var elementId = o.ID;
	var toReplace = [/'/gi, / /gi, /\//g];

	if (!isNullOrEmpty(o.prefix) && !isNullOrEmpty(elementId))
	{
		elementId = o.prefix + o.Delimiter + elementId;
	}
	if (!isNullOrEmpty(o.suffix) && !isNullOrEmpty(elementId))
	{
		elementId += o.Delimiter + o.suffix;
	}
	for (var i = 0; i < toReplace.length; i++)
	{
		if(toReplace.hasOwnProperty(i)) {
		    if (!isNullOrEmpty(elementId))
		    {
		        elementId = elementId.replace(toReplace[i], '');
		    }
		}
	}

	return elementId;
}

function isNullOrEmpty(obj)
{
	/// <summary> Returns true if the input is null, undefined, or ''</summary>
	/// <param name="obj" type="Object"> Object to test</param>
	/// <returns type="Boolean" />
    var ret = false;
	if (!isFunction(obj))
	{
		ret = $.isPlainObject(obj) && $.isEmptyObject(obj);
		if (!ret && isGeneric(obj))
		{
			ret = (trim(obj) === '');
		}
	} 
	return ret;
}

function isGeneric(obj)
{
	/// <summary> Returns true if the object is not a function, array, jQuery or JSON object</summary>
	/// <param name="obj" type="Object"> Object to test</param>
	/// <returns type="Boolean" />
	var ret = (!isFunction(obj) && !isArray(obj) && !isJQuery(obj) && !isJson(obj));
	return ret;
}

function isFunction(obj)
{
	/// <summary> Returns true if the object is a function</summary>
	/// <param name="obj" type="Object"> Object to test</param>
	/// <returns type="Boolean" />
	var ret = ($.isFunction(obj));
	return ret;
}

function isArray(obj)
{
	/// <summary> Returns true if the object is an array</summary>
	/// <param name="obj" type="Object"> Object to test</param>
	/// <returns type="Boolean" />
	var ret = ($.isArray(obj));
	return ret;
}

function isJson(obj)
{
	/// <summary> 
	///    Returns true if the object is a JSON object.
	///     &#10; isJson(CswInput_Types.text) === true 
	///     &#10; isJson(CswInput_Types.text.name) === false
	/// </summary>
	/// <param name="obj" type="Object"> Object to test</param>
	/// <returns type="Boolean" />
	var ret = ($.isPlainObject(obj));
	return ret;
}

function isJQuery(obj)
{
	var ret = (obj instanceof jQuery);
	return ret;
}

function isNumeric(obj)
{
	/// <summary>
	///   Returns true if the input is a number
	/// </summary>
	/// <param name="str" type="Object">
	///     String or object to test
	/// </param>
	var ret = false;
	if (!isNullOrEmpty(obj))
	{
		var num = parseInt(obj);
		if (num !== NaN)
		{
			ret = true;
		}
	}
	return ret;
}

function isTrue(str,isTrueIfNull)
{
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

function tryParseString(inputStr, defaultStr)
{
	/// <summary>
	///   Returns the inputStr if !isNullOrEmpty, else returns the defaultStr
	/// </summary>
	/// <param name="inputStr" type="String"> String to parse </param>
	/// <param name="defaultStr" type="String"> Default value if null or empty </param>
	/// <returns type="String" />
    var ret = '';
    if (defaultStr) {
        ret = defaultStr;
    }
	if (!isNullOrEmpty(inputStr))
	{
		ret = inputStr;
	}
	return ret;
}

var Int32MinVal = new Number(-2147483648);
function tryParseNumber(inputNum, defaultNum)
{
	/// <summary>
	///   Returns the inputNum if !NaN, else returns the defaultNum
	/// </summary>
	/// <param name="inputNum" type="Number"> String to parse to number </param>
	/// <param name="defaultNum" type="Number"> Default value if not a number </param>
	/// <returns type="Number" />
	var ret = new Number(defaultNum);
	var tryRet = new Number(inputNum);
	if (tryRet.toString() !== "NaN" && tryRet !== Int32MinVal)
	{
		ret = tryRet;
	}
	return ret;
}

function tryParseElement(elementId, $context)
{
	/// <summary>
	///   Attempts to fetch an element from the DOM first through jQuery, then through JavaScript
	/// </summary>
	/// <param name="elementId" type="String"> ElementId to find </param>
	/// <param name="$context" type="jQuery"> Optional context to limit the search </param>
	/// <returns type="jQuery">jQuery object, empty if no match found.</returns>
	var $ret = $('');
	if (!isNullOrEmpty(elementId))
	{
		if (arguments.length == 2 && !isNullOrEmpty($context))
		{
			$ret = $('#' + elementId, $context);
		}
		else
		{
			$ret = $('#' + elementId);
		}

		if ($ret.length === 0)
		{
			$ret = $(document.getElementById(elementId));
		}
		if ($ret.length === 0)
		{
			$ret = $(document.getElementsByName(elementId));
		}
	}
	return $ret;
}

function trim(str)
{
	/// <summary>Returns a string without left and right whitespace</summary>
	/// <param name="str" type="String"> String to parse </param>
	/// <returns type="String">Parsed string</returns>
	return $.trim(str);
}

function makeDelegate(method, options) {
    /// <summary>
    /// Returns a function with the argument parameter of the value of the current instance of the object.
    /// <para>For example, in a "for(i=0;i<10;i++)" loop, makeDelegate will capture the value of "i" for a given function.</para>
    /// </summary>
	/// <param name="method" type="Function"> A function to delegate. </param>
    /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
	/// <returns type="Function">A delegate function: function(options)</returns>
    return function() { method(options); };
}

function makeEventDelegate(method, options) {
    /// <summary>
    /// Returns a function with the event object as the first parameter, and the current value of options as the second parameter.
    /// </summary>
	/// <param name="method" type="Function"> A function to delegate. </param>
    /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
	/// <returns type="Function">A delegate function: function(eventObj, options)</returns>
    return function(eventObj) { method(eventObj,options); };
}

function ObjectHelper(obj) {
    /// <summary>Find an object in a JSON object.</summary>
	/// <param name="obj" type="Object"> Object to search </param>
	/// <returns type="ObjectHelper"></returns>
    var thisObj = obj;
    
    function foundMatch(anObj, prop, value) {
        var ret = false;
        if(false === isNullOrEmpty(anObj) &&
           anObj.hasOwnProperty(prop) &&
           anObj[prop] === value ) {
            ret = true;
        }
        return ret;
    }
    
    function recursiveFind(parentObj, key, value) {
        /// <summary>Recursively search an object</summary>
	    /// <param name="parentObj" type="Object"> Object to search </param>
        /// <param name="key" type="String"> Property name to match. </param>
        /// <param name="value" type="Object"> Property value to match </param>
	    /// <returns type="Object"> Returns the value of the 'property' property which contains a matching key/value prop. </returns>
        var ret = { };
        if (jQuery.isPlainObject(parentObj)) {
            for (var childKey in parentObj) {
                if (parentObj.hasOwnProperty(childKey)) {
                    var childObj = parentObj[childKey];
                    if (foundMatch(childObj, key, value)) {
                        ret = childObj;
                        break;
                    } 
                    else if (isNullOrEmpty(ret) && jQuery.isPlainObject(childObj)) {
                        ret = recursiveFind(childObj, key, value);
                    }
                }
            }
        }
        return ret;
    }
    
    function find(key, value) {
        /// <summary>Find a property's parent</summary>
        /// <param name="key" type="String"> Property name to match. </param>
        /// <param name="value" type="Object"> Property value to match </param>
	    /// <returns type="Object"> Returns the value of the 'property' property which contains a matching key/value prop. </returns>
        var ret = { };
        if (jQuery.isPlainObject(thisObj))
        {
            for (var childKey in thisObj) {
                if (thisObj.hasOwnProperty(childKey)) {
                    var childObj = thisObj[childKey];
                    if (foundMatch(childObj, key, value)) {
                        ret = thisObj;
                        break;
                    } 
                    else if (isNullOrEmpty(ret) && jQuery.isPlainObject(childObj)) {
                        ret = recursiveFind(childObj, key, value);                        
                    }
                }
            }
        }
        return ret;
    }
    
    function recursiveDelete(parentObj, key, value) {
        var ret = false;
        if (jQuery.isPlainObject(parentObj)) {
            for (var childKey in parentObj) {
                if (parentObj.hasOwnProperty(childKey)) {
                    var childObj = parentObj[childKey];
                    if (foundMatch(childObj, key, value)) {
                        delete parentObj[childKey];
                        ret = true;
                        break;
                    } 
                    else if (false === ret && jQuery.isPlainObject(childObj)) {
                        ret = recursiveDelete(childObj, key, value);
                    }
                }
            }
        }
        return ret;
    }
    
    function remove(key, value) {
        var ret = false;
        if (jQuery.isPlainObject(thisObj))
        {
            for (var childKey in thisObj) {
                if (thisObj.hasOwnProperty(childKey)) {
                    var childObj = thisObj[childKey];
                    if (foundMatch(childObj, key, value)) {
                        delete thisObj[childKey];
                        ret = true;
                        break;
                    } 
                    else if (false === ret && jQuery.isPlainObject(childObj)) {
                        ret = recursiveDelete(childObj, key, value);                        
                    }
                }
            }
        }
        return ret;
    }
    
    this.find = find;
    this.remove = remove;
    this.obj = thisObj;
}

// because IE 8 doesn't support console.log unless the console is open (*duh*)
function log(s, includeCallStack)
{
	/// <summary>Outputs a message to the console log(Webkit,FF) or an alert(IE)</summary>
	/// <param name="s" type="String"> String to output </param>
	/// <param name="includeCallStack" type="Boolean"> If true, include the callStack </param>
	var extendedLog = '';
	if (isTrue(includeCallStack))
	{
		extendedLog = console.trace();
	}

	try
	{
		if (!isNullOrEmpty(extendedLog))
			console.log(s, extendedLog);
		else
			console.log(s);
	} catch (e)
	{
		alert(s);
		if (!isNullOrEmpty(extendedLog)) alert(extendedLog);
	}
}