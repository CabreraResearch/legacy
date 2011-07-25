/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var PluginName = 'CswFieldTypePassword';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            if(o.ReadOnly)
            {
                // show nothing
            }
            else 
            {
                var $table = $Div.CswTable('init', { 
					ID: o.ID + '_tbl', 
					'OddCellRightAlign': true 
				});
                var $cell11 = $table.CswTable('cell', 1, 1);
                var $cell12 = $table.CswTable('cell', 1, 2);
                var $cell21 = $table.CswTable('cell', 2, 1);
                var $cell22 = $table.CswTable('cell', 2, 2);

                $cell11.append('Set New');
				var $TextBox1 = $cell12.CswInput('init',{ID: o.ID + '_pwd1',
                                                         type: CswInput_Types.password,
                                                         cssclass: 'textinput',
                                                         onChange: o.onchange
                                                 }); 
                $cell21.append('Confirm');
                var $TextBox2 = $cell22.CswInput('init',{ID: o.ID + '_pwd2',
                                                         type: CswInput_Types.password,
                                                         cssclass: 'textinput password2',
                                                         onChange: o.onchange
                                                 }); 

                if(o.Required && isNullOrEmpty(o.$propxml.children('password').text()))
                {
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
                var $TextBox = o.$propdiv.find('input#' + o.ID + '_pwd1');
                var newpw = $TextBox.val();
				if(newpw !== '')
				{
					o.$propxml.children('newpassword').text(newpw);
				}
			}
    };
    
    // Method calling logic
    $.fn.CswFieldTypePassword = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
