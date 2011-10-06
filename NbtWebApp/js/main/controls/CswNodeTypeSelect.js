/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

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
					onSelect: null, //function (nodetypeid) {},
					onSuccess: null //function () {}
				};

				if (options)
				{
					$.extend(o, options);
				}

				var $parent = $(this);
				$parent.contents().remove();

				var $select = $('<select id="'+ o.ID +'_sel" />')
								.appendTo($parent);
				$select.change(function() { if (isFunction(o.onSelect)) o.onSelect( $select.val() ); });

				CswAjaxJson({
						url: o.NodeTypesUrl,
						data: {},
						success: function (data)
						{
							for (var nodeType in data) {
							    if (data.hasOwnProperty(nodeType)) {
							        var thisNodeType = data[nodeType];
							        $select.append('<option value="' + thisNodeType.id + '">' + thisNodeType.name + '</option>');
							    }
							}
						    if (isFunction(o.onSuccess)) {
						        o.onSuccess();
						    }
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

