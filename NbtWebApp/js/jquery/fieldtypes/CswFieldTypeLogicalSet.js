; (function ($) {
        
    var PluginName = 'CswFieldTypeLogicalSet';
    var NameCol = "name";
    var KeyCol = "key";

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var $LogicalSetXml = o.$propxml.children('logicalsetxml');

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
                                    var fieldname = $(this).CswAttrXml('field');
                                    if(fieldname !== NameCol && fieldname != KeyCol)
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
                    var value = $this.children('column[field="'+ cols[c] +'"]').CswAttrXml('value');
                    values[r] = (value === "True");
                    r++;
                }

                var $elm = { 'label': $this.children('column[field="' + NameCol + '"]').CswAttrXml('value'),
                             'key': $this.children('column[field="' + KeyCol + '"]').CswAttrXml('value'),
                             'values': values };
                data[d] = $elm;
                d++;
            });
                
            $CBADiv.CswCheckBoxArray('init', {
                                     'ID': o.ID + '_cba',
                                     'cols': cols,
                                     'data': data,
                                     'onchange': o.onchange,
									 'ReadOnly': o.ReadOnly
                                    });


        },
        save: function(o) { //$propdiv, $xml
                var $LogicalSetXml = o.$propxml.children('logicalsetxml');
                var $CBADiv = o.$propdiv.children('div').first();
                var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
                for( var r = 0; r < formdata.length; r++)
                {
                    for( var c = 0; c < formdata[r].length; c++)
                    {
                        var checkitem = formdata[r][c];
                        var $xmlitem = $LogicalSetXml.find('item:has(column[field="'+ KeyCol +'"][value="'+ checkitem.key +'"])');
                        var $xmlitemcolumn = $xmlitem.find('column[field="' + checkitem.collabel + '"]');
                    
                        if(checkitem.checked && $xmlitemcolumn.CswAttrXml('value') === "False")
                            $xmlitemcolumn.CswAttrXml('value', 'True');
                        else if(!checkitem.checked && $xmlitemcolumn.CswAttrXml('value') === "True")
                            $xmlitemcolumn.CswAttrXml('value', 'False');

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





