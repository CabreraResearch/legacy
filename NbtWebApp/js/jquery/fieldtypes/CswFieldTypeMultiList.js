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
                var $ValueTable = $Div.CswTable({
										ID: o.ID + '_valtbl'
									});
            
                var SplitOptions = Options.split(',');
                for(var i = 0; i < SplitOptions.length; i++)
                {
                    $SelectBox.append('<option value="' + SplitOptions[i] + '">' + SplitOptions[i] + '</option>');
                }
                $SelectBox.val( Value );

                if(o.Required)
                {
                    $SelectBox.addClass("required");
                }

				function _handleOnChange()
				{
					_addValue($SelectBox.val());
					$SelectBox.children('option:selected').remove();
					$SelectBox.val('');
					o.onchange();
				} // _handleOnChange()

				function _addValue(valuetext)
				{
					var row = $ValueTable.CswTable('maxrows') + 1;
					var $cell1 = $ValueTable.CswTable('cell', row, '1');
					var $cell2 = $ValueTable.CswTable('cell', row, '2');
					$cell1.parent().hide();
					var $ThisValue = $('<div id="val_'+ valuetext +'">'+ valuetext + '</div>')
										.appendTo( $cell1 );
					$cell1.parent().fadeIn('slow');
					$cell2.CswImageButton({
						ButtonType: CswImageButton_ButtonType.Delete,
						AlternateText: 'Remove',
						ID: makeId({ 'prefix': valuetext, 'id': 'rembtn' }),
						onClick: function ($ImageDiv) { 
							$cell1.parent().fadeOut('slow', function() { 
								$ThisValue.remove();
							});
							return CswImageButton_ButtonType.None; 
						}
					})
				} // _addValue()

            } // if-else(o.ReadOnly)
        },
        save: function(o) { //$propdiv, $xml
                var $SelectBox = o.$propdiv.find('select');
                o.$propxml.children('value').text($SelectBox.val());
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
