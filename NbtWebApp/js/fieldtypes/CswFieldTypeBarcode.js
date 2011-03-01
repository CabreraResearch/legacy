; (function ($) {
        
    var PluginName = 'CswFieldTypeBarcode';

    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = ($xml.attr('required') == "true");
                var ReadOnly = ($xml.attr('readonly') == "true");

                var Value = $xml.children('barcode').text();

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $table = makeTable(ID + '_tbl').appendTo($Div);

                    var $cell1 = getTableCell($table, 1, 1);
                    var $TextBox = $('<input type="text" class="textinput" id="'+ ID +'" name="' + ID + '" value="'+ Value +'" />"' )
                                     .appendTo($cell1);

                    var $cell2 = getTableCell($table, 1, 2);
                    var $PrintButton = $('<div/>' )
                                         .appendTo($cell2)
                                         .CswImageButton({  ButtonType: CswImageButton_ButtonType.Print,
                                                            AlternateText: '',
                                                            ID: '',
                                                            onClick: function (alttext) { 
                                                                $.CswDialog('OpenDialog', ID + '_dialog', 'Popup_PrintLabel.aspx?nodeid=' + nodepk + '&propid=' + ID); 
                                                                return CswImageButton_ButtonType.None; 
                                                            }
                                                         });

                    if(Required)
                    {
                        $TextBox.addClass("required");
                    }
                }
            },
        save: function($propdiv, $xml) {
                var $TextBox = $propdiv.find('input');
                $xml.children('barcode').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeBarcode = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
