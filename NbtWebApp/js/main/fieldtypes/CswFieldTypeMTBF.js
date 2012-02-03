/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";    
    var pluginName = 'CswFieldTypeMTBF';

    var methods = {
        init: function (o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var startDate = (false === o.Multi) ? Csw.string(propVals.startdatetime.date) : Csw.enums.multiEditDefaultValue; 
            var dateFormat = Csw.serverDateFormatToJQuery(propVals.startdatetime.dateformat);

            var value = (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue; 
            var units = (false === o.Multi) ? Csw.string(propVals.units).trim() : Csw.enums.multiEditDefaultValue; 

            var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
            var $cell11 = $table.CswTable('cell', 1, 1);
            var $cell12 = $table.CswTable('cell', 1, 2);

            var mtbfStatic = (units !== Csw.enums.multiEditDefaultValue) ? value + '&nbsp;' + units : value;
            $cell11.append(mtbfStatic);
            if(!o.ReadOnly) {
                $cell12.CswImageButton({
                            ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                            AlternateText: 'Edit',
                            'ID': o.ID,
                            onClick: function () { 
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
//                                                                  type: Csw.enums.inputTypes.text,
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
                    unitVals.push(Csw.enums.multiEditDefaultValue);
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
        save: function (o) { //$propdiv, $xml

            var attributes = {
                startdatetime: {
                    date: null,
                    time: null
                }, 
                units: null
            };
            
            var $StartDate = o.$propdiv.find('#' + o.ID + '_sd'),
                dateVal;
            
            if (false === Csw.isNullOrEmpty($StartDate)) {
                dateVal = $StartDate.CswDateTimePicker('value', o.propData.readonly);
                attributes.startdatetime.date = dateVal.date;
                attributes.startdatetime.time = dateVal.time;
            }

            var $Units = o.$propdiv.find('#' + o.ID + '_units');
            if (false === Csw.isNullOrEmpty($Units)) {
                attributes.units = $Units.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeMTBF = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
