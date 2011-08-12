/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

; (function ($)
{
	var PluginName = "CswNodeSelect";

	var methods = {
		'init': function(options) 
			{
				var o = {
					ID: '',
					NodesUrl: '/NbtWebApp/wsNBT.asmx/getNodes',
					nodetypeid: '',
					objectclassid: '',
					objectclass: '',
					onSelect: null, // function (nodeid) {},
					onSuccess: null // function () {}
				};

				if (options)
				{
					$.extend(o, options);
				}

				var $parent = $(this);

				var $select = $('<select id="'+ o.ID +'_nodeselect" />')
								.appendTo($parent);
				$select.change(function() { o.onSelect( $select.val() ); });

				var jsonData = {
					NodeTypeId: o.nodetypeid,
					ObjectClassId: o.objectclassid,
					ObjectClass: o.objectclass
				};

				CswAjaxJson({
						url: o.NodesUrl,
						data: jsonData,
						success: function (data)
						{
							for (var nodeId in data) {
							    if (data.hasOwnProperty(nodeId)) {
							        var nodeName = data[nodeId];
							        $select.append('<option value="' + nodeId + '">' + nodeName + '</option>');
							    }
							}
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
	$.fn.CswNodeSelect = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

})(jQuery);

