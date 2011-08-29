; (function ($) {
	$.fn.CswDashboard = function (options) {

		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getDashboard',
			onSuccess: function() { }
		};

		if (options) {
			$.extend(o, options);
		}

		var $DashDiv = $(this);
		
		CswAjaxJson({
			url: o.Url,
			data: {},
			stringify: false,
			success: function (data) {

				var $table = $DashDiv.CswTable('init', { ID: 'DashboardTable' });
				$table.addClass('DashboardTable');
				var $tr = $table.append('<tr />');

				for (var dashId in data) {
				    if (data.hasOwnProperty(dashId)) {
				        var thisIcon = data[dashId];
				        var cellcontent = '';
				        if (thisIcon.href !== undefined)
				        {
				            cellcontent = '<td class="DashboardCell">' +
    				            '  <a target="_blank" href="' + thisIcon.href + '">' +
        				            '    <div title="' + thisIcon.text + '" id="' + dashId + '" class="' + dashId + '" />' +
            				            '  </a>' +
                				            '</td>';
				        } else {
				            cellcontent = '<td class="DashboardCell">' +
    				            '  <div title="' + thisIcon.text + '" id="' + dashId + '" class="' + dashId + '" />' +
        				            '</td>';
				        }
				        $tr.append(cellcontent);
				    }
				}
			    o.onSuccess();

			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);




