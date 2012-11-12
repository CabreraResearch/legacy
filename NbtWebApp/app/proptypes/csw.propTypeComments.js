/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.comments = Csw.properties.comments ||
        Csw.properties.register('comments',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPrivate = { };
                var cswPublic = {
                    data: propertyOption
                };
                
                //The render function to be executed as a callback
                var render = function() {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.rows = Csw.string(cswPrivate.propVals.rows);
                    cswPrivate.columns = Csw.string(cswPrivate.propVals.columns);

                    cswPrivate.commentsDiv = cswPrivate.parent.div({
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

                    Csw.each(cswPrivate.propVals.comments, function (acomment) {
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
                    if (false === cswPublic.data.isReadOnly()) {
                        cswPublic.control = cswPrivate.parent.textArea({
                            rows: cswPrivate.rows,
                            cols: cswPrivate.columns,
                            onChange: function () {
                                var comment = cswPublic.control.val();
                                Csw.tryExec(cswPublic.data.onChange, comment);
                                propertyOption.onPropChange({ newmessage: comment });
                            }
                        }); 
                    }
                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();
                
                return cswPublic;
            }));
    
}());
