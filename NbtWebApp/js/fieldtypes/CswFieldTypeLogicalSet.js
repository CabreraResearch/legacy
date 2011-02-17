; (function ($) {
        
    var PluginName = 'CswFieldTypeLogicalSet';

    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = $xml.attr('required');
                var ReadOnly = $xml.attr('readonly');

                var $LogicalSetXml = $xml.children('logicalsetxml');
                var NameCol = "name";
                var KeyCol = "key";

                //<LogicalSetXml>
                //    <item>
                //        <column field="name" value="Assembly"></column>
                //        <column field="key" value="7"></column>
                //        <column field="View" value="True"></column>
                //        <column field="Create" value="True"></column>
                //        <column field="Delete" value="True"></column>
                //        <column field="Edit" value="True"></column>
                //    </item>
                //    <item>
                //        ...
                //    </item>
                //</LogicalSetXml>
            
                var $CBADiv = $('<div />')
                                .appendTo($Div);

                // get columns
                var cols = new Array();
                var c = 0;

                $LogicalSetXml.find('item')
                              .first()
                              .children('column')
                              .each(function() {
                                      var fieldname = $(this).attr('field');
                                      if(fieldname != NameCol && fieldname != KeyCol)
                                      {
                                          cols[c] = fieldname;
                                          c++;
                                      }
                              });

                // get data
                var data = new Array();
                var d = 0;
                $LogicalSetXml.find('item').each(function () {
                    var $this = $(this);
                    var values = new Array();
                    var r = 0;
                    for(var c = 0; c < cols.length; c++)
                    {
                        var value = $this.children('column[field="'+ cols[c] +'"]').attr('value');
                        values[r] = (value == "True");
                        r++;
                    }

                    var $elm = { 'label': $this.children('column[field="' + NameCol + '"]').attr('value'),
                                 'key': $this.children('column[field="' + KeyCol + '"]').attr('value'),
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
                var $LogicalSetXml = $xml.children('logicalsetxml');
                var $CBADiv = $propdiv.children('div').first();
                var formdata = $CBADiv.CswCheckBoxArray( 'getdata' );
                for( var r = 0; r < formdata.length; r++)
                {
                    for( var c = 0; c < formdata[r].length; c++)
                    {
                        var checkitem = formdata[r][c];
                        var $xmlitem = $LogicalSetXml.find('item:has(column[field="key"][value="'+ checkitem.key +'"])');
                        var $xmlitemcolumn = $xmlitem.find('column[field="'+checkitem.collabel+'"]');
                    
                        if(checkitem.checked && $xmlitemcolumn.attr('value') == "False")
                            $xmlitemcolumn.attr('value', 'True');
                        else if(!checkitem.checked && $xmlitemcolumn.attr('value') == "True")
                            $xmlitemcolumn.attr('value', 'False');

                    } // for( var c = 0; c < formdata.length; c++)
                } // for( var r = 0; r < formdata.length; r++)
            } // save()
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





