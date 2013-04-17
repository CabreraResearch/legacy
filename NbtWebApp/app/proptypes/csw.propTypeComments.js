/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.comments = Csw.properties.register('comments',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';

                var cswPrivate = Csw.object();

                cswPrivate.rows = nodeProperty.propData.values.rows;
                cswPrivate.columns = nodeProperty.propData.values.columns;

                cswPrivate.commentsDiv = nodeProperty.propDiv.div({
                    value: '',
                    cssclass: 'scrollingdiv',
                    width: '350px'
                });
                
                cswPrivate.myTable = cswPrivate.commentsDiv.table({
                    TableCssClass: '',
                    CellCssClass: '',
                    cellpadding: 4,
                    cellspacing: 0,
                    align: '',
                    width: '100%',
                    cellalign: 'top',
                    cellvalign: 'top',
                    FirstCellRightAlign: false,
                    OddCellRightAlign: false,
                    border: 0
                });

                cswPrivate.arow = 0;
                cswPrivate.bgclass = '';

                Csw.iterate(nodeProperty.propData.values.comments, function(acomment) {
                    acomment = acomment || { datetime: '', commenter: '', message: '' };
                    cswPrivate.arow += 1;
                    var cell1 = cswPrivate.myTable.cell(cswPrivate.arow * 2, 1);
                    var cell2 = cswPrivate.myTable.cell(cswPrivate.arow * 2, 2);
                    if ((cswPrivate.arow % 2) === 0) {
                        cswPrivate.bgclass = 'OddRow';
                    } else {
                        cswPrivate.bgclass = 'EvenRow';
                    }
                    cell1.addClass(cswPrivate.bgclass);
                    cell2.addClass(cswPrivate.bgclass);
                    cell2.append(acomment.datetime);
                    cell1.append(acomment.commenter);
                    cell2.propDom('align', 'right');
                    cell1.css({ fontStyle: 'italic' });
                    cell2.css({ fontStyle: 'italic' });
                    var cell3 = cswPrivate.myTable.cell(cswPrivate.arow * 2 + 1, 1);
                    cell3.propNonDom('colspan', '2');
                    cell3.addClass(cswPrivate.bgclass);
                    cell3.append(acomment.message);
                });
                if (false === nodeProperty.isReadOnly()) {
                    nodeProperty.propDiv.textArea({
                        rows: cswPrivate.rows,
                        cols: cswPrivate.columns,
                        onChange: function(comment) {
                            //Case 29390: No sync for comments either
                            nodeProperty.propData.values.newmessage = comment;
                            
                            //Csw.tryExec(nodeProperty.onChange, comment);
                            //nodeProperty.onPropChange({ newmessage: comment });
                        }
                    });
                }
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());
