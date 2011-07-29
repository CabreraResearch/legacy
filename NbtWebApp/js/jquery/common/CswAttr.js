/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	$.fn.CswAttrDom = function (name, value) {
		/// <summary>
		///   Gets or sets a DOM attribute
		/// </summary>
		/// <param name="name" type="String">The name of the attribute</param>
		/// <param name="value" type="String">The value of the attribute</param>
		/// <returns type=Object>Either the value of the attribute (get) or this (set) for chaining</returns> 
		
		var $Dom = $(this);
		
		if( typeof name === "object") {
			for( var prop in name )
			{
				doProp($Dom,prop,name[prop]);
			}
		} else {
			doProp($Dom,name,value);
		}

		return $Dom;

	}; // function(options) {

	function doProp($Dom,name,value) {
	    var propVal;
	    if(arguments.length === 2) {
			propVal = $Dom.prop(name);
		} else {
			propVal = $Dom.prop(name, value);
		}

		// special cases
		if( propVal === undefined ||
			name === 'href' ||
			name === 'cellpadding' || 
			name === 'cellspacing' ||
			name === 'rowspan' ||
			name === 'colspan' )
		{
			if(arguments.length === 2) {
			    $Dom.attr(name);
			} else {
			    $Dom.attr(name, value);
			}
		}
	}

	$.fn.CswAttrXml = function (name, value) {
		/// <summary>
		///   Gets or sets an Xml attribute
		/// </summary>
		/// <param name="name" type="String">The name of the attribute</param>
		/// <param name="value" type="String">The value of the attribute</param>
		/// <returns type="Object">Either the value of the attribute (get) or this (set) for chaining</returns> 

		var X$xml = $(this);

		if( typeof name === "object") {
			for(var prop in name) {
				doAttr(X$xml,prop,name[prop]);
			}
		} else {
			doAttr(X$xml,name,value);
		}
		// For proper chaining support
		return X$xml;

	}; // function(options) {

	function doAttr(X$xml,name,value) {
		switch( arguments.length ) {
			case 2:
			{
				X$xml.attr(name);
				break;
			}
			case 3:
			{
				X$xml.attr(name, value);
				break;
			}
		}
	}

})(jQuery);


 