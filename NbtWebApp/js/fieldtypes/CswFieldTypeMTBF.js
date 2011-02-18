; (function ($) {
        
    var PluginName = 'CswFieldTypeMTBF';

    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = ($xml.attr('required') == "true");
                var ReadOnly = ($xml.attr('readonly') == "true");

                var StartDate = $xml.children('startdatetime').text();
                var Value = $xml.children('value').text();
                var Units = $xml.children('units').text();

                var $table = makeTable(ID + '_tbl')
                                .appendTo($Div);
                var $cell11 = getTableCell($table, 1, 1);
                var $cell12 = getTableCell($table, 1, 2);

                $cell11.append(Value + '&nbsp;' + Units);
                if(!ReadOnly)
                {
                    var $EditButton = $('<div />')
                                        .appendTo($cell12);
                    $EditButton.CswImageButton({
                                                 ButtonType: CswImageButton_ButtonType.Edit,
                                                 AlternateText: 'Edit',
                                                 'ID': ID,
                                                 onClick: function (alttext) { alert('This is not implemented yet'); return CswImageButton_ButtonType.None; }
                                              });
                }
            },
        save: function($propdiv, $xml) {
                var $StartDateTextBox = $propdiv.find('input#'+ ID +'_sd');
                $xml.children('startdatetime').text($StartDateTextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeMTBF = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
