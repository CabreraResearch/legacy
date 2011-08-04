; (function ($) {
        
    var PluginName = 'CswFieldTypeMultiList';
    
    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var Value = o.$propxml.children('value').text().trim();
            var Options = o.$propxml.children('options').text().trim();

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
				var $SelectBox = $('<select id="'+ o.ID +'" name="'+ o.ID +'" class="selectinput" />"' )
                                    .appendTo($Div)
                                    .change(function() { _handleOnChange(); });

                var $ValueTableDiv = $('<div />')
									.appendTo($Div)
									.css('padding', '2px')
									.css('max-height', '143px')
									.css('border', '1px solid #336699')
									.css('overflow', 'auto');

				var $ValueTable = $ValueTableDiv.CswTable({
										ID: o.ID + '_valtbl'
									});

				var $HiddenValue = $('<input type="hidden" name="' + o.ID + '_value" id="' + o.ID + '_value" value="'+ Value +'"/>')
                                    .appendTo($Div);

                var SplitOptions = Options.split(',');
                for(var i = 0; i < SplitOptions.length; i++)
                {
                    $SelectBox.append('<option value="' + SplitOptions[i] + '">' + SplitOptions[i] + '</option>');
                }

                if(!isNullOrEmpty(Value))
				{
					var SplitValue = Value.split(',');
					for(var i = 0; i < SplitValue.length; i++)
					{
						_addValue(SplitValue[i], false);
					}
				} else {
					$ValueTableDiv.hide();
				}

//                if(o.Required)
//                {
//                    $SelectBox.addClass("required");
//                }

				function _handleOnChange()
				{
					var valuetext = $SelectBox.val();

					$SelectBox.val('');

					var currentvalue = $HiddenValue.val();
					if(!isNullOrEmpty(currentvalue)) currentvalue += ',';
					$HiddenValue.val(currentvalue + valuetext);

					_addValue(valuetext, true);

					o.onchange();
				} // _handleOnChange()

				function _addValue(valuetext, doAnimation)
				{
					$SelectBox.children('option[value="'+ valuetext +'"]').remove();

					$ValueTableDiv.show();
					var row = $ValueTable.CswTable('maxrows') + 1;
					var $cell1 = $ValueTable.CswTable('cell', row, '1');
					var $cell2 = $ValueTable.CswTable('cell', row, '2');
					$cell1.css('padding-right', '20px');

					if(doAnimation) $cell1.parent().hide();
					var $ThisValue = $('<div id="val_'+ valuetext +'">'+ valuetext + '</div>')
										.appendTo( $cell1 );
					if(doAnimation) $cell1.parent().fadeIn('slow');

					$cell2.CswImageButton({
						ButtonType: CswImageButton_ButtonType.Delete,
						AlternateText: 'Remove',
						ID: makeId({ 'prefix': valuetext, 'id': 'rembtn' }),
						onClick: function ($ImageDiv) { 
							$cell1.parent().fadeOut('slow', function() { 
								$SelectBox.append('<option value="' + valuetext + '">'+ valuetext + '</option>');

								var currentvalue = $HiddenValue.val();
								var splitvalue = currentvalue.split(',');
								var newvalue = '';
								for(var i = 0; i < splitvalue.length; i++)
								{
									if(splitvalue[i] != valuetext)
									{
										if(!isNullOrEmpty(newvalue)) newvalue += ',';
										newvalue += splitvalue[i];
									}
								}
								$HiddenValue.val(newvalue);
								if(isNullOrEmpty(newvalue))
								{
									$ValueTableDiv.hide();
								}
								$ThisValue.remove();
							});
							o.onchange();
							return CswImageButton_ButtonType.None; 
						}
					})
				} // _addValue()

            } // if-else(o.ReadOnly)
        },
        save: function(o) { //$propdiv, $xml
                var $HiddenValue = o.$propdiv.find('#' + o.ID + '_value');
                o.$propxml.children('value').text($HiddenValue.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeMultiList = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
