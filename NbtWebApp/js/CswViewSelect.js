; (function ($) {
	$.fn.CswViewSelect = function (options) {

		var o = {
			ID: '',
			ViewUrl: '', //'/NbtWebApp/wsNBT.asmx/getViews',
			viewid: '',
			onSelect: function(optSelect) { },
			ClickDelay: 300
		};

		if (options) {
			$.extend(o, options);
		}

		var $selectdiv = $(this);
		$selectdiv.children().remove();

		getViewSelect(o.viewid);

		function getViewSelect(optSelect) // selectedviewid
		{
			var m = {
				selectedviewid: '',
				viewid: '',
			};

			if (optSelect) {
				$.extend(m, optSelect);
			}

			$viewtreediv = $('<div/>');
			$selectdiv.CswComboBox('init', { 'ID': o.ID + '_combo', 
											 'TopContent': 'Select a View',
											 'SelectContent': $viewtreediv,
											 'Width': '266px' });

			$viewtreediv.CswViewTree({ 'onSelect': onTreeSelect });
			
		} // getViewSelect()

		function onTreeSelect(optSelect) //itemid, text, iconurl
		{
			var $newTopContent = $('<div></div>');
			var $table = makeTable(o.ID + 'selectedtbl')
						   .appendTo($newTopContent);
			var $cell1 = getTableCell($table, 1, 1);
			var $icondiv = $('<div />').appendTo($cell1);
			$icondiv.css('background-image',  optSelect.iconurl);
			$icondiv.css('width', '18px');
			$icondiv.css('height' ,'18px');

			var $cell2 = getTableCell($table, 1, 2);
			$cell2.append(optSelect.text);

			$selectdiv.CswComboBox( 'TopContent', $newTopContent );
			setTimeout(function() { $selectdiv.CswComboBox( 'toggle'); }, o.ClickDelay);
			o.onSelect(optSelect);
		}
		
		// For proper chaining support
		return this;

	}; // function(options) {
}) (jQuery);

