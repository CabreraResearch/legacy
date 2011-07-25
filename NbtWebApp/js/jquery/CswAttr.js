/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
    $.fn.CswAttrDom = function (name, value)
    {
        /// <summary>
        ///   Gets or sets a DOM attribute
        /// </summary>
        /// <param name="name" type="String">The name of the attribute</param>
        /// <param name="value" type="String">The value of the attribute</param>
        /// <returns type=Object>Either the value of the attribute (get) or this (set) for chaining</returns> 
        
        var $Dom = $(this);
        var ret = undefined;
        
		if( typeof name === "object")
        {
            for( prop in name )
            {
                ret = doProp($Dom,prop,name[prop]);
            }
        }
        else
        {
            ret = doProp($Dom,name,value);
        }

        return ret;

    }; // function(options) {

    function doProp($Dom,name,value)
    {
        var ret = undefined;
        ret = $Dom.prop(name,value);

        if(arguments.length === 2)
		{
        	ret = $Dom.prop(name);
        }
        else
        {
            ret = $Dom.prop(name, value);
        }

		// special cases
		if( ret === undefined ||
			name === 'href' ||
			name === 'cellpadding' || 
			name === 'cellspacing' ||
			name === 'rowspan' ||
			name === 'colspan' )
		{
	        if(arguments.length === 2)
				ret = $Dom.attr(name);
			else
                ret = $Dom.attr(name,value);
		}

        return ret;
    }

    $.fn.CswAttrXml = function (name, value)
    {
        
        /// <summary>
        ///   Gets or sets an Xml attribute
        /// </summary>
        /// <param name="name" type="String">The name of the attribute</param>
        /// <param name="value" type="String">The value of the attribute</param>
        /// <returns type=Object>Either the value of the attribute (get) or this (set) for chaining</returns> 

        var X$xml = $(this);
        var ret = undefined;

        if( typeof name === "object")
        {
            for(prop in name)
            {
                ret = doAttr(X$xml,prop,name[prop]);
            }
        }
        else
        {
            ret = doAttr(X$xml,name,value);
        }
        // For proper chaining support
        return ret;

    }; // function(options) {

    function doAttr(X$xml,name,value)
    {
        var ret = undefined;

        switch( arguments.length )
        {
            case 2:
            {
                ret = X$xml.attr(name);
                break;
            }
            case 3:
            {
                ret = X$xml.attr(name, value);
                break;
            }
        }
        // For proper chaining support
        return ret;
    }

})(jQuery);


