/// <reference path="~app/CswApp-vsdoc.js" />


; (function ($) {

    var pluginName = 'CswFieldTypeComments';

    var methods = {
        init: function (o) {

            var parent = o.propDiv;
            parent.empty();
           
            var propVals = o.propData.values;
            var rows = Csw.string(propVals.rows);
            var columns = Csw.string(propVals.columns);


            //comments:  [ { datetime: '12/31/2012', commenter: 'david', message: 'yuck' }, { ... } ]

            var commentsDiv = parent.div({
                'value': '',
                'cssclass': 'scrollingdiv',
                'width': '350px'
            });
            var myTable = commentsDiv.table({
                            TableCssClass: '',
                            CellCssClass: '',
                            cellpadding: 4,
                            cellspacing: 0,
                            align: '',
                            width: '100%',
                            cellalign: 'top',
                            cellvalign: 'top',
                            //onCreateCell: function (e, $table, $cell, row, column) { },
                            FirstCellRightAlign: false,
                            OddCellRightAlign: false,
                            border: 0
                        });
            var arow=0;
            var bgclass='';
            Csw.each(propVals.comments,function(acomment){
               arow+=1;
               var cell1 = myTable.cell(arow*2,1); 
               var cell2 = myTable.cell(arow*2,2); 
                if( (arow % 2)===0 ){
                    bgclass='OddRow';
                }
                else{
                    bgclass='EvenRow';
                }
                cell1.addClass(bgclass);
                cell2.addClass(bgclass);
                cell2.append(acomment.datetime);
                cell1.append(acomment.commenter);
                cell2.propDom('align','right');
                cell1.css({fontStyle: 'italic'});
                cell2.css({fontStyle: 'italic'});
               var cell3 = myTable.cell(arow*2 + 1,1); 
                cell3.propNonDom('colspan','2');
                cell3.addClass(bgclass);
                cell3.append(acomment.message);
            });
            if (false === o.ReadOnly) {
                var TextArea = parent.textArea({ rows: rows, cols: columns, onChange: o.onChange }); //$('<textarea id="' + o.ID + '" name="' + o.ID + '" rows="' + rows + '" cols="' + columns + '"></textarea>')       
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = { newmessage: null };
            var compare = {};
            var TextArea = o.propDiv.find('textarea');
            if (false === Csw.isNullOrEmpty(TextArea)) {
                attributes.newmessage = TextArea.val();
                compare = attributes;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeComments = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };
})(jQuery);
