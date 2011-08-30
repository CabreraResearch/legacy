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
					eventArg: {},
					delay: 500
				};
				if(options) $.extend(o, options);
				
				var $div = $('<div id="' + o.ID + '"></div>')
								.css({
									position: 'absolute',
									top: o.eventArg.pageY + 'px',
									left: o.eventArg.pageX + 'px',
									width: '400px',
									height: '300px',
									overflow: 'auto',
									border: '1px solid #003366',
									padding: '2px',
									backgroundColor: '#ffffff'
								})
								.appendTo('body')
								.hide();

				var timeoutHandle = setTimeout(function() {
						$div.CswNodeTabs({
										ID: o.ID + 'tabs',
										nodeid: o.nodeid,               
										cswnbtnodekey: o.cswnbtnodekey,        
										EditMode: EditMode.Preview.name,
										ShowAsReport: false,
										onInitFinish: function() {
											$div.show();
										}
									});
					}, o.delay);

				$div.data('timeoutHandle', timeoutHandle);

				return $div;
			},
		'close': function()
			{
				var $div = $(this);
				if($div.length > 0)
				{
					clearTimeout($div.data('timeoutHandle'));
					$div.remove();
				}
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

