; (function ($) {
        
    var PluginName = 'CswFieldTypePassword';

    var methods = {
        init: function(nodepk, $xml, onchange) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = ($xml.attr('required') == "true");
                var ReadOnly = ($xml.attr('readonly') == "true");

                if(ReadOnly)
                {
                    // show nothing
                }
                else 
                {
                    var $table = makeTable(ID + '_tbl')
                                    .appendTo($Div);
                    var $cell11 = getTableCell($table, 1, 1);
                    var $cell21 = getTableCell($table, 2, 1);

                    var $TextBox1 = $('<input type="password" class="textinput" id="'+ ID +'_pwd1" name="' + ID + '" />"' )
                                     .appendTo($cell11)
                                     .change(onchange);
                    var $TextBox1 = $('<input type="password" class="textinput password2" id="'+ ID +'_pwd2" name="' + ID + '" />"' )
                                     .appendTo($cell21)
                                     .change(onchange);
//                    if(Required)
//                    {
//                        $TextBox.addClass("required");
//                    }

                    jQuery.validator.addMethod( "password2", function(value, element) { 
                                var pwd1 = $('#' + ID + '_pwd1').val();
                                var pwd2 = $('#' + ID + '_pwd2').val();
                                return ((pwd1 == '' && pwd2 == '') || pwd1 == pwd2);
                            }, 'Passwords do not match!');
                }
            },
        save: function($propdiv, $xml) {
                var ID = $xml.attr('id');
                var $TextBox = $propdiv.find('input#' + ID + '_pwd1');
                $xml.children('newpassword').text($TextBox.val());
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
