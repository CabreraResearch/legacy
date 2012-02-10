/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";        
    var pluginName = 'CswFieldTypePassword';

    var methods = {
        init: function (o) { 

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var isExpired = (false === o.Multi) ? Csw.bool(propVals.isexpired) : null;
            var isAdmin = (false === o.Multi) ? Csw.bool(propVals.isadmin) : null;

            if(o.ReadOnly) {
                // show nothing
            } else {
                var table = Csw.controls.table({
                    $parent: $Div,
                    ID: Csw.controls.dom.makeId(o.ID, 'tbl'),
                    OddCellRightAlign: true 
                });
                    
                table.add(1, 1, 'Set New');
                var cell12 = table.cell(1, 2);
                table.add(2, 1, 'Confirm');
                var cell22 = table.cell(2, 2);
                var cell31 = table.cell(3, 1);
                table.add(3, 2, 'Expired');

                var $TextBox1 = cell12.$.CswInput('init',{ID: o.ID + '_pwd1',
                                                         type: Csw.enums.inputTypes.password,
                                                         cssclass: 'textinput',
                                                         value: (false === o.Multi) ? '' : Csw.enums.multiEditDefaultValue,
                                                         onChange: o.onchange
                                                 }); 
                /* Text Box 2 */
                cell22.$.CswInput('init',{ID: o.ID + '_pwd2',
                                                         type: Csw.enums.inputTypes.password,
                                                         value: (false === o.Multi) ? '' : Csw.enums.multiEditDefaultValue,
                                                         cssclass: 'textinput password2',
                                                         onChange: o.onchange
                                                 }); 
                if(isAdmin) {
                    var $IsExpiredCheck = cell31.$.CswInput({ 
                            id: o.ID + '_exp',
                            name: o.ID + '_exp',
                            type: Csw.enums.inputTypes.checkbox
                        });
                    if(isExpired) {
                        $IsExpiredCheck.CswAttrDom('checked', 'true');
                    }

                }
                
                if (o.Required && Csw.isNullOrEmpty(propVals.password)) {
                    $TextBox1.addClass("required");
                }

                $.validator.addMethod( "password2", function () { 
                            var pwd1 = $('#' + o.ID + '_pwd1').val();
                            var pwd2 = $('#' + o.ID + '_pwd2').val();
                            return ((pwd1 === '' && pwd2 === '') || pwd1 === pwd2);
                        }, 'Passwords do not match!');
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = {
                isexpired: null,
                newpassword: null
            };
            var $newpw = o.$propdiv.find('input#' + o.ID + '_pwd1');
            if (false === Csw.isNullOrEmpty($newpw)) {
                attributes.newpassword = $newpw.val();
            }
            
            var $IsExpiredCheck = o.$propdiv.find('input#' + o.ID + '_exp');
            if (false === Csw.isNullOrEmpty($IsExpiredCheck) && $IsExpiredCheck.length > 0) {
                attributes.isexpired = $IsExpiredCheck.is(':checked');    
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypePassword = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
