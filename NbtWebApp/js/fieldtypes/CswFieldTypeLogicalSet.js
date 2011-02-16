; (function ($) {
        
    var PluginName = 'CswFieldTypeLogicalSet';

    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = $xml.attr('required');
                var ReadOnly = $xml.attr('readonly');

                var $LogicalSetXml = $($xml.children('LogicalSetXml').text());
                
                var $CBADiv = $('<div />')
                                .appendTo($Div);

                // get columns
                var cols = new Array();
                var c = 0;

                $LogicalSetXml.find('yvalue')
                              .first()
                              .children()
                              .each(function() {
                                  cols[c] = this.nodeName;
                                  c++;
                              });


                // get data
                var data = new Array();
                var d = 0;
                $LogicalSetXml.find('yvalue').each(function () {
                    var $this = $(this);
                    var values = new Array();
                    var r = 0;
                    for(var c = 0; c < cols.length; c++)
                    {
                        values[r] = ($this.children(cols[c]).text() == "1");
                        r++;
                    }

                    var $elm = { 'label': $this.attr('y'),
                                 'key': $this.attr('y'),
                                 'values': values };
                    data[d] = $elm;
                    d++;
                });
                
                $CBADiv.CswCheckBoxArray('init', {
                                         'ID': ID + '_cba',
                                         'cols': cols,
                                         'data': data
                                        });


            },
        save: function($propdiv, $xml) {
                var $LogicalSetXml = $($xml.children('LogicalSetXml').text());
                var $CBADiv = $propdiv.children('div').first();
                var formdata = $CBADiv.CswCheckBoxArray( 'getdata' );
                for( var r = 0; r < formdata.length; r++)
                {
                    for( var c = 0; c < formdata[r].length; c++)
                    {
                        var checkitem = formdata[r][c];
                        $xmlitem = $LogicalSetXml.find('yvalue[y="'+ checkitem.key +'"]');
                        if(checkitem.checked && $xmlitem.find(checkitem.collabel).text() == "0")
                            $xmlitem.find(checkitem.collabel).text('1');
                        else if(!checkitem.checked && $xmlitem.find(checkitem.collabel).text() == "1")
                            $xmlitem.find(checkitem.collabel).text('0');

                    } // for( var c = 0; c < formdata.length; c++)
                } // for( var r = 0; r < formdata.length; r++)
                $xml.children('LogicalSetXml').text($LogicalSetXml.get(0).outerHTML);
            }
    };
    

    // Method calling logic
    $.fn.CswFieldTypeLogicalSet = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);





