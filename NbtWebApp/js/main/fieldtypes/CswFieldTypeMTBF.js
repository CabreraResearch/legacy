; (function ($) {
        
    var PluginName = 'CswFieldTypeMTBF';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var StartDate = o.$propxml.children('startdatetime').text().trim();
            var Value = o.$propxml.children('value').text().trim();
            var Units = o.$propxml.children('units').text().trim();

            var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
            var $cell11 = $table.CswTable('cell', 1, 1);
            var $cell12 = $table.CswTable('cell', 1, 2);

            $cell11.append(Value + '&nbsp;' + Units);
            if(!o.ReadOnly)
            {
                var $EditButton = $cell12.CswImageButton({
                                                ButtonType: CswImageButton_ButtonType.Edit,
                                                AlternateText: 'Edit',
                                                'ID': o.ID,
                                                onClick: function ($ImageDiv) { 
													$edittable.show();
												}
                                            });
				
				var $edittable = $table.CswTable('cell', 2, 2).CswTable('init', { 'ID': o.ID + '_edittbl' });
				$edittable.CswTable('cell', 1, 1).append('Start Date');
                var $StartDateBoxCell = $edittable.CswTable('cell', 1, 2);
                var $StartDateBox = $StartDateBoxCell.CswInput('init',{ID: o.ID + '_sd',
                                                                  type: CswInput_Types.text,
                                                                  cssclass: 'textinput date',
                                                                  value: StartDate,
                                                                  onChange: o.onchange
                                                                }); 
                $StartDateBox.datepicker();
				
//				$edittable.CswTable('cell', 2, 1).append('End Date');
//                var $EndDateBox = $('<input type="text" class="textinput date" id="'+ o.ID +'_ed" name="' + o.ID + '_ed" value="" />"' )
//									.appendTo($edittable.CswTable('cell', 2, 2))
//                                    .datepicker();
				
				$edittable.CswTable('cell', 3, 1).append('Units');
				var $UnitsSelect = $('<select id="'+ o.ID + '_units" />')
									.appendTo($edittable.CswTable('cell', 3, 2))
									.change(o.onchange);
				var $hoursopt = $('<option value="hours">hours</option>').appendTo($UnitsSelect);
				if(Units === 'hours') $hoursopt.CswAttrDom('selected', 'true');
				var $daysopt = $('<option value="days">days</option>').appendTo($UnitsSelect);
				if(Units === 'days') $daysopt.CswAttrDom('selected', 'true');

//				var $refreshbtn = $('<input type="button" id="'+ o.ID + '_refresh" value="Refresh">')
//									.appendTo($edittable.CswTable('cell', 4, 2));

				$edittable.hide();
            }
        },
        save: function(o) { //$propdiv, $xml
                var StartDate = o.$propdiv.find('#'+ o.ID +'_sd').val();
                var Units = o.$propdiv.find('#'+ o.ID +'_units').val();
                o.$propxml.children('startdatetime').text(StartDate);
                o.$propxml.children('units').text(Units);
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeMTBF = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
