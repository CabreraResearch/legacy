/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
	var PluginName = 'CswNodePreview';

	var methods = {
		'open': function(options) 
			{    
				var o = {
					ID: '',
					nodeid: '',
					cswnbtnodekey: '',
					X: 0,
					Y: 0
				};
				if(options) $.extend(o, options);
				
				var $div = $('<div id="' + o.ID + '"></div>')
								.css({
									position: 'absolute',
									top: o.Y + 'px',
									left: o.X + 'px',
									width: '100px',
									height: '100px',
									border: '1px solid #003366',
									backgroundColor: '#ffffff'
								})
								.appendTo('body');

				$div.append('nodeid = ' + o.nodeid);

				return $div;
			},
		'close': function()
			{
				var $div = $(this);
				$div.remove();
			}
	};


	// Method calling logic
	$.CswNodePreview = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

	$.fn.CswNodePreview = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

})(jQuery);

