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
            var propVals = o.propData.values;
            var startDate = (false === o.Multi) ? tryParseString(propVals.startdatetime.date) : CswMultiEditDefaultValue; 
            var dateFormat = ServerDateFormatToJQuery(propVals.startdatetime.dateformat);

            var value = (false === o.Multi) ? tryParseString(propVals.value).trim() : CswMultiEditDefaultValue; 
            var units = (false === o.Multi) ? tryParseString(propVals.units).trim() : CswMultiEditDefaultValue; 

            var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
            var $cell11 = $table.CswTable('cell', 1, 1);
            var $cell12 = $table.CswTable('cell', 1, 2);

            var mtbfStatic = (units !== CswMultiEditDefaultValue) ? value + '&nbsp;' + units : value;
            $cell11.append(mtbfStatic);
            if(!o.ReadOnly) {
                $cell12.CswImageButton({
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
				
				$StartDateBoxCell.CswDateTimePicker('init', { 
				                    ID: o.ID + '_sd',
									Date: startDate,
									DateFormat: dateFormat,
									DisplayMode: 'Date',
									ReadOnly: o.ReadOnly,
									Required: o.Required,
									OnChange: o.onchange
								});

//                var $StartDateBox = $StartDateBoxCell.CswInput('init',{ID: o.ID + '_sd',
//                                                                  type: CswInput_Types.text,
//                                                                  cssclass: 'textinput date',
//                                                                  value: startDate,
//                                                                  onChange: o.onchange
//                                                                }); 
//				$StartDateBox.datepicker({ 'dateFormat': dateFormat });

//				$edittable.CswTable('cell', 2, 1).append('End Date');
//                var $EndDateBox = $('<input type="text" class="textinput date" id="'+ o.ID +'_ed" name="' + o.ID + '_ed" value="" />"' )
//									.appendTo($edittable.CswTable('cell', 2, 2))
//                                    .datepicker();
				
				$edittable.CswTable('cell', 3, 1).append('Units');
                var unitVals = ['hours', 'days'];
                if (o.Multi) {
                    unitVals.push(CswMultiEditDefaultValue);
                }
                $edittable.CswTable('cell', 3, 2)
                          .CswSelect('init', {
                                ID: o.ID + '_units',
                                onChange: o.onchange,
                                values: unitVals,
                                selected: units
                            });
                
//				var $refreshbtn = $('<input type="button" id="'+ o.ID + '_refresh" value="Refresh">')
//									.appendTo($edittable.CswTable('cell', 4, 2));

				$edittable.hide();
            }
        },
        save: function(o) { //$propdiv, $xml
            var startDate = o.$propdiv.find('#'+ o.ID +'_sd').CswDateTimePicker('value').Date;
            var units = o.$propdiv.find('#'+ o.ID +'_units').val();
            var propVals = o.propData.values;
            if(false === o.Multi || (startDate !== CswMultiEditDefaultValue && units !== CswMultiEditDefaultValue)) {
                propVals.startdatetime.date = startDate;
                propVals.units = units;
            }
            else if(startDate !== CswMultiEditDefaultValue) {
                propVals.startdatetime.date = startDate;
                delete propVals.units;
            }
            else if(units !== CswMultiEditDefaultValue) {
                propVals.units = units;
                delete propVals.startdatetime;
            } else {
                delete o.propData;
            }
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
