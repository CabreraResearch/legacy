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
					onSelect: function (nodeid) {},
					onSuccess: function () {},
				};

				if (options)
				{
					$.extend(o, options);
				}

				var $parent = $(this);

				var $select = $('<select id="'+ o.ID +'_nodeselect" />')
								.appendTo($parent);
				$select.change(function(event) { o.onSelect( $select.val() ); });

				CswAjaxXml({
						url: o.NodesUrl,
						data: 'NodeTypeId=' + o.nodetypeid + '&ObjectClassId=' + o.objectclassid + '&ObjectClass=' + o.objectclass,
						success: function ($xml)
						{
							$xml.children('node').each(function() {
								var $node = $(this);
								$select.append('<option value="'+ $node.attr('id') +'">'+ $node.attr('name') +'</option>');
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

