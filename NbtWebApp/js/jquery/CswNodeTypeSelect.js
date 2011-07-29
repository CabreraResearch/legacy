/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($)
{
	var PluginName = "CswNodeTypeSelect";

	var methods = {
		'init': function(options) 
			{
				var o = {
					ID: '',
					NodeTypesUrl: '/NbtWebApp/wsNBT.asmx/getNodeTypes',
					nodetypeid: '',
					onSelect: function (nodetypeid) {},
					onSuccess: function () {}
				};

				if (options)
				{
					$.extend(o, options);
				}

				var $parent = $(this);
				$parent.contents().remove();

				var $select = $('<select id="'+ o.ID +'_sel" />')
								.appendTo($parent);
				$select.change(function(event) { o.onSelect( $select.val() ); });

				CswAjaxXml({
						url: o.NodeTypesUrl,
						data: {},
						stringify: false,
						success: function ($xml)
						{
							$xml.children('nodetype').each(function() {
								var $nodetype = $(this);
								$select.append('<option value="'+ $nodetype.CswAttrXml('id') +'">'+ $nodetype.CswAttrXml('name') +'</option>');
							});

							o.onSuccess();
						}
				});

				return $select;
			},
		'value': function()
			{
				var $select = $(this);
				return $select.val();
			}
	};
		// Method calling logic
	$.fn.CswNodeTypeSelect = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

})(jQuery);

