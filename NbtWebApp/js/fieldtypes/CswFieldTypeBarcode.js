; (function ($) {
        
    var PluginName = 'CswFieldTypeBarcode';

    var methods = {
        init: function(optSelect) { //nodepk, $xml, onchange == nodeid,propxml,onchange
            var o = {
                nodeid: '',
                propxml: '',
                onchange: ''
            }
            if(optSelect)
            {
                $.extend(o,optSelect);
            }
                var $Div = $(this);
                $Div.children().remove();

                var ID = o.propxml.attr('id');
                var Required = (o.propxml.attr('required') == "true");
                var ReadOnly = (o.propxml.attr('readonly') == "true");

                var Value = o.propxml.children('barcode').text().trim();

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $table = makeTable(ID + '_tbl').appendTo($Div);

                    var $cell1 = getTableCell($table, 1, 1);
                    var $TextBox = $('<input type="text" class="textinput" id="'+ ID +'" name="' + ID + '" value="'+ Value +'" />"' )
                                     .appendTo($cell1)
                                     .change(o.onchange);

                    var $cell2 = getTableCell($table, 1, 2);
                    var $PrintButton = $('<div/>' )
                                         .appendTo($cell2)
                                         .CswImageButton({  ButtonType: CswImageButton_ButtonType.Print,
                                                            AlternateText: '',
                                                            ID: '',
                                                            onClick: function (alttext) { 
                                                                $.CswDialog('OpenDialog', ID + '_dialog', 'Popup_PrintLabel.aspx?nodeid=' + o.nodeid + '&propid=' + ID); 
                                                                return CswImageButton_ButtonType.None; 
                                                            }
                                                         });

                    if(Required)
                    {
                        $TextBox.addClass("required");
                    }
                }
            },
        save: function(o.propdiv, o.propxml) {
                var $TextBox = $propdiv.find('input');
                o.propxml.children('barcode').text($TextBox.val());
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
