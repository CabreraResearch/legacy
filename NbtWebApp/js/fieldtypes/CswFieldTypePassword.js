; (function ($) {
        
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
                var $table = $.CswTable({ ID: o.ID + '_tbl' })
                                .appendTo($Div);
                var $cell11 = $table.CswTable('cell', 1, 1);
                var $cell21 = $table.CswTable('cell', 2, 1);

                var $TextBox1 = $('<input type="password" class="textinput" id="'+ o.ID +'_pwd1" name="' + o.ID + '" />"' )
                                    .appendTo($cell11)
                                    .change(onchange);
                var $TextBox2 = $('<input type="password" class="textinput password2" id="'+ o.ID +'_pwd2" name="' + o.ID + '" />"' )
                                    .appendTo($cell21)
                                    .change(onchange);
//                    if(o.Required)
//                    {
//                        $TextBox.addClass("required");
//                    }

                jQuery.validator.addMethod( "password2", function(value, element) { 
                            var pwd1 = $('#' + o.ID + '_pwd1').val();
                            var pwd2 = $('#' + o.ID + '_pwd2').val();
                            return ((pwd1 == '' && pwd2 == '') || pwd1 == pwd2);
                        }, 'Passwords do not match!');
            }
        },
        save: function(o) { //$propdiv, $xml
                var $TextBox = $propdiv.find('input#' + o.ID + '_pwd1');
                o.$propxml.children('newpassword').text($TextBox.val());
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
