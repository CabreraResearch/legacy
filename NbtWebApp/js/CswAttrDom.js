///// <reference path="../jquery/jquery-1.6-vsdoc.js" />
///// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
///// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
///// <reference path="_Global.js" />

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
        
	    if(arguments.length === 1)
			ret = $Dom.prop(name);
		else
            ret = $Dom.prop(name,value);

		// special cases
		if( ret === undefined ||
			name === 'href' ||
			name === 'cellpadding' || 
			name === 'cellspacing' ||
			name === 'rowspan' ||
			name === 'colspan' )
		{
	        if(arguments.length === 1)
				ret = $Dom.attr(name);
			else
                ret = $Dom.attr(name,value);
		}

        return ret;

    }; // function(options) {
})(jQuery);


