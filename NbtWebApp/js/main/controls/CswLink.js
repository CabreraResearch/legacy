/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

(function ($) { 
	"use strict";
	var pluginName = "CswLink";
	
	var methods = {
	
		init: function (options) 
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
				onClick: function () {}
			};
			if (options) $.extend(o, options);
			
			var $parent = $(this);
			var $link = $('<a></a>');
			
			var elementId = Csw.string(o.ID,'');
			if (elementId !== '' ) $link.CswAttrDom('id',elementId);
			if (false === Csw.isNullOrEmpty(o.href)) $link.CswAttrDom('href', o.href);
			if (false === Csw.isNullOrEmpty(o.value)) $link.text(o.value);
			if (false === Csw.isNullOrEmpty(o.cssclass)) $link.addClass(o.cssclass);
			if (false === Csw.isNullOrEmpty(o.type)) $link.CswAttrDom('type',o.type);
		    if (false === Csw.isNullOrEmpty(o.title)) $link.CswAttrDom('title',o.title);
			if (false === Csw.isNullOrEmpty(o.rel)) $link.CswAttrDom('rel',o.rel);
			if (false === Csw.isNullOrEmpty(o.media)) $link.CswAttrDom('media',o.media);
			if (false === Csw.isNullOrEmpty(o.target)) $link.CswAttrDom('target',o.target);
			if (false === Csw.isNullOrEmpty(o.onClick)) 
			{
				$link.click( function () {
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
