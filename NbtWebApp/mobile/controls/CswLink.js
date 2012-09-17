/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

(function ($) { /// <param name="$" type="jQuery" />
	"use strict";
	var pluginName = "CswLink";
	
	var methods = {
	
		init: function(options) 
		{
			var o = {
				ID: '',
				value: '',
				type: '', //MIME type
				media: 'all',
				target: '',
			    title: '',
				rel: '',
				cssclass: '',
				onClick: function() {}
			};
			if (options) $.extend(o, options);
			
			var $parent = $(this);
			var $link = $('<a></a>');
			
			var elementId = tryParseString(o.ID,'');
			if (elementId !== '' ) $link.CswAttrDom('id',elementId);
			if (false === isNullOrEmpty(o.href)) $link.CswAttrDom('href', o.href);
			if (false === isNullOrEmpty(o.value)) $link.text(o.value);
			if (false === isNullOrEmpty(o.cssclass)) $link.addClass(o.cssclass);
			if (false === isNullOrEmpty(o.type)) $link.CswAttrDom('type',o.type);
		    if (false === isNullOrEmpty(o.title)) $link.CswAttrDom('title',o.title);
			if (false === isNullOrEmpty(o.rel)) $link.CswAttrDom('rel',o.rel);
			if (false === isNullOrEmpty(o.media)) $link.CswAttrDom('media',o.media);
			if (false === isNullOrEmpty(o.target)) $link.CswAttrDom('target',o.target);
			if (false === isNullOrEmpty(o.onClick)) 
			{
				$link.click( function() {
							 o.onClick();
				});
			}
					
			$parent.append($link);
			return $link;
		}
	};
		// Method calling logic
	$.fn.CswLink = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
		}    
  
	};


})(jQuery);
