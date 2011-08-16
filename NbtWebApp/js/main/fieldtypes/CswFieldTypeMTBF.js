/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeMTBF';

    var methods = {
        init: function(o) {

            var $Div = $(this);
            $Div.contents().remove();

            var startDate = tryParseString(o.propData.startdatetime).trim();
            var value = tryParseString(o.propData.value).trim();
            var units = tryParseString(o.propData.units).trim();

            var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
            var $cell11 = $table.CswTable('cell', 1, 1);
            var $cell12 = $table.CswTable('cell', 1, 2);

            $cell11.append(value + '&nbsp;' + units);
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
                                                                  value: startDate,
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
				if(units === 'hours') $hoursopt.CswAttrDom('selected', 'true');
				var $daysopt = $('<option value="days">days</option>').appendTo($UnitsSelect);
				if(units === 'days') $daysopt.CswAttrDom('selected', 'true');

//				var $refreshbtn = $('<input type="button" id="'+ o.ID + '_refresh" value="Refresh">')
//									.appendTo($edittable.CswTable('cell', 4, 2));

				$edittable.hide();
            }
        },
        save: function(o) { //$propdiv, $xml
                var StartDate = o.$propdiv.find('#'+ o.ID +'_sd').val();
                var Units = o.$propdiv.find('#'+ o.ID +'_units').val();
                o.propData.startdatetime = StartDate;
                o.propData.units = Units;
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeMTBF = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
