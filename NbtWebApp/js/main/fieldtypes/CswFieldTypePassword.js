/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/CswInput.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypePassword';

    var methods = {
        init: function(o) { 

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
			var isExpired = (false === o.Multi) ? isTrue(propVals.isexpired) : null;
			var isAdmin = (false === o.Multi) ? isTrue(propVals.isadmin) : null;

            if(o.ReadOnly) {
                // show nothing
            } else {
                var $table = $Div.CswTable('init', { 
					ID: o.ID + '_tbl', 
					'OddCellRightAlign': true 
				});
                var $cell11 = $table.CswTable('cell', 1, 1);
                var $cell12 = $table.CswTable('cell', 1, 2);
                var $cell21 = $table.CswTable('cell', 2, 1);
                var $cell22 = $table.CswTable('cell', 2, 2);
                var $cell31 = $table.CswTable('cell', 3, 1);
                var $cell32 = $table.CswTable('cell', 3, 2);

                $cell11.append('Set New');
				var $TextBox1 = $cell12.CswInput('init',{ID: o.ID + '_pwd1',
                                                         type: CswInput_Types.password,
                                                         cssclass: 'textinput',
				                                         value: (false === o.Multi) ? '' : CswMultiEditDefaultValue,
                                                         onChange: o.onchange
                                                 }); 
                $cell21.append('Confirm');
                var $TextBox2 = $cell22.CswInput('init',{ID: o.ID + '_pwd2',
                                                         type: CswInput_Types.password,
                                                         value: (false === o.Multi) ? '' : CswMultiEditDefaultValue,
                                                         cssclass: 'textinput password2',
                                                         onChange: o.onchange
                                                 }); 
                if(isAdmin) {
					var $IsExpiredCheck = $cell31.CswInput({ 
							'id': o.ID + '_exp',
							'name': o.ID + '_exp',
							'type': CswInput_Types.checkbox
						});
					if(isExpired) {
						$IsExpiredCheck.CswAttrDom('checked', 'true');
					}
                    $cell32.append('Expired');
				}
                
                if (o.Required && isNullOrEmpty(propVals.password)) {
                    $TextBox1.addClass("required");
                }

                jQuery.validator.addMethod( "password2", function(value, element) { 
                            var pwd1 = $('#' + o.ID + '_pwd1').val();
                            var pwd2 = $('#' + o.ID + '_pwd2').val();
                            return ((pwd1 === '' && pwd2 === '') || pwd1 === pwd2);
                        }, 'Passwords do not match!');
            }
        },
        save: function(o) { //$propdiv, $xml
            var attributes = {
                isexpired: null,
                newpassword: null
            };
            var $newpw = o.$propdiv.find('input#' + o.ID + '_pwd1');
            if (false === isNullOrEmpty($newpw)) {
                attributes.newpassword = $newpw.val();
            }
            
            var $IsExpiredCheck = o.$propdiv.find('input#' + o.ID + '_exp');
            if (false === isNullOrEmpty($IsExpiredCheck) && $IsExpiredCheck.length > 0) {
                attributes.isexpired = $IsExpiredCheck.is(':checked');    
            }
            preparePropJsonForSave(o.Multi, o, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypePassword = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
