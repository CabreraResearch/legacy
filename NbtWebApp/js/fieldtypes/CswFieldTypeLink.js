; (function ($) {
        
    var PluginName = 'CswFieldTypeLink';

    var methods = {
        init: function(nodepk, $xml, onchange) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = ($xml.attr('required') == "true");
                var ReadOnly = ($xml.attr('readonly') == "true");

                var Text = $xml.children('text').text().trim();
                var Href = $xml.children('href').text().trim();

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $table = makeTable(ID + '_tbl')
                                    .appendTo($Div);
                    var $cell11 = getTableCell($table, 1, 1);
                    var $cell12 = getTableCell($table, 1, 2);

                    var $Link = $('<a href="'+ Href +'" target="_blank">'+ Text +'</a>&nbsp;&nbsp;' )
                                  .appendTo($cell11);
                    
                    var $EditButton = $('<div/>')
                                   .appendTo($cell12)
                                   .CswImageButton({
                                                    ButtonType: CswImageButton_ButtonType.Edit,
                                                    AlternateText: 'Edit',
                                                    ID: ID + '_edit',
                                                    Required: Required,
                                                    onClick: function (alttext) { alert('not implemented yet'); return CswImageButton_ButtonType.None; }
                                                   });
//                    if(Required)
//                    {
//                        $Link.addClass("required");
//                    }
                }
            },
        save: function($propdiv, $xml) {
//                var $Link = $propdiv.find('input');
//                $xml.children('text').text($Link.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeLink = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
