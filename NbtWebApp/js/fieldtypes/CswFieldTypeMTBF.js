; (function ($) {
        
    var PluginName = 'CswFieldTypeMTBF';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.children().remove();

            var StartDate = o.$propxml.children('startdatetime').text().trim();
            var Value = o.$propxml.children('value').text().trim();
            var Units = o.$propxml.children('units').text().trim();

            var $table = makeTable(o.ID + '_tbl')
                            .appendTo($Div);
            var $cell11 = getTableCell($table, 1, 1);
            var $cell12 = getTableCell($table, 1, 2);

            $cell11.append(Value + '&nbsp;' + Units);
            if(!o.ReadOnly)
            {
                var $EditButton = $('<div />')
                                    .appendTo($cell12);
                $EditButton.CswImageButton({
                                                ButtonType: CswImageButton_ButtonType.Edit,
                                                AlternateText: 'Edit',
                                                'ID': o.ID,
                                                onClick: function (alttext) { alert('This is not implemented yet'); return CswImageButton_ButtonType.None; }
                                            });
            }
        },
        save: function(o) { //$propdiv, $xml
                var $StartDateTextBox = o.$propdiv.find('input#'+ o.ID +'_sd');
                o.$propxml.children('startdatetime').text($StartDateTextBox.val());
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
