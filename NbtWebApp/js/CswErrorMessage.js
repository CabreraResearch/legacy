; (function ($) {
	$.fn.CswErrorMessage = function (options) {

		var o = {
			message: '',
			detail: ''
		};
		if (options) $.extend(o, options);

		var $parentdiv = $(this);
		$parentdiv.show();

		var date = new Date();
		var id = "error_" + date.getTime();

		var $errordiv = $('<div />')
						.appendTo($parentdiv)
						.attr('id', id)
						.addClass('CswErrorMessage_ErrorMessage');

	 	var $tbl = $errordiv.CswTable('init', {
												'id': makeId({ 
														'prefix': id, 
														'id': 'tbl' 
													}), 
												'width': '100%' 
											});
		var $cell11 = $tbl.CswTable('cell', 1, 1)
							.attr('width', '100%');
		var $cell12 = $tbl.CswTable('cell', 1, 2);
		var $cell21 = $tbl.CswTable('cell', 2, 1);

		var $msglink = $('<a href="#">' + o.message + '</a>')
						.appendTo($cell11)
						.click(function() { 
							$cell21.toggle();
						});
		$cell21.append(o.detail);
		$cell21.hide();

		$cell12.CswImageButton({
		    ButtonType: CswImageButton_ButtonType.Delete,
            AlternateText: 'Hide',
            ID: makeId({ 'prefix': id, 'id': 'hidebtn' }),
            onClick: function ($ImageDiv) { 
				$errordiv.remove();
				if($parentdiv.children().length === 0)
					$parentdiv.hide();
				return CswImageButton_ButtonType.None; 
			}
		})

		return $errordiv;

	}; // function(options) {
})(jQuery);
