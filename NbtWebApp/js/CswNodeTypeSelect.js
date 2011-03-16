; (function ($)
{
	$.fn.CswNodeTypeSelect = function (options)
	{

		var o = {
			ID: '',
			ViewUrl: '/NbtWebApp/wsNBT.asmx/getNodeTypes',
			nodetypeid: '',
			onSelect: function (nodetypeid) {},
			onSuccess: function () {},
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
				url: o.ViewUrl,
				data: '',
				success: function ($xml)
				{
					$xml.children('nodetype').each(function() {
						var $nodetype = $(this);
						$select.append('<option value="'+ $nodetype.attr('id') +'">'+ $nodetype.attr('name') +'</option>');
					});

					o.onSuccess();
				}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

