/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.4-vsdoc.js" />
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

				var windowX = $(window).width() - 10;
				var windowY = $(window).height() - 10;
				var $div = $('<div id="' + o.ID + '" class="CswNodePreview"></div>')
								.css({
									position: 'absolute',
									overflow: 'auto',
									border: '1px solid #003366',
									padding: '2px',
									backgroundColor: '#ffffff',
									maxWidth: windowX,
									maxHeight: windowY
								})
								.appendTo('body')
								.hide();

				var timeoutHandle = setTimeout(function() {
						$div.CswNodeTabs({
										ID: o.ID + 'tabs',
										nodeids: [ o.nodeid ],
										cswnbtnodekeys: [ o.cswnbtnodekey ],
										EditMode: EditMode.Preview.name,
										ShowAsReport: false,
										onInitFinish: function() {
											
											// Make sure preview div is within the window
											windowX = $(window).width() - 10;
											windowY = $(window).height() - 10;
											var divwidth = $div.width();
											var divheight = $div.height();
											var X = o.eventArg.pageX;
											var Y = o.eventArg.pageY;

											if(X + divwidth > windowX) X = windowX - divwidth;
											if(Y + divheight > windowY) Y = windowY - divheight;

											$div.css({ 
													top: Y + 'px',
													left: X + 'px'
											});
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
					// Clear all node previews, in case other ones are hanging around
					$('.CswNodePreview').remove();
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

