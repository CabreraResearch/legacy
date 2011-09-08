/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypeBarcode';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly  == nodeid,propxml,onchange

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? tryParseString(propVals.barcode).trim() : CswMultiEditDefaultValue;

            if(o.ReadOnly)
            {
                $Div.append(value);
            }
            else 
            {
                var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

                var $cell1 = $table.CswTable('cell', 1, 1);
                var $TextBox = $cell1.CswInput('init',{ID: o.ID,
                                                       type: CswInput_Types.text,
                                                       cssclass: 'textinput',
                                                       onChange: o.onchange,
                                                       value: value
                                               });
                if (false === o.Multi) {
                    var $cell2 = $table.CswTable('cell', 1, 2);
                    $('<div/>')
                        .appendTo($cell2)
                        .CswImageButton({  ButtonType: CswImageButton_ButtonType.Print,
                                AlternateText: '',
                                ID: o.ID + '_print',
                                onClick: function($ImageDiv) {
                                    $.CswDialog('PrintLabelDialog', { 'nodeid': o.nodeid, 'propid': o.ID });
                                    return CswImageButton_ButtonType.None;
                                }
                            });
                }
                if(o.Required) {
                    $TextBox.addClass("required");
                }
				
				$TextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function(o) {
            var attributes = { barcode: barcode };
            var $TextBox = o.$propdiv.find('input');
            if(false === isNullOrEmpty($TextBox)) {
                attributes.barcode = $TextBox.val();
            }
            preparePropJsonForSave(o.Multi, o, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeBarcode = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
