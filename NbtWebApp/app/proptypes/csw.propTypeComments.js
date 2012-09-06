/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.comments = Csw.properties.comments ||
        Csw.properties.register('comments',
            Csw.method(function(propertyOption) {
                'use strict';
                var ret = {
                    data: propertyOption
                };
                var render = function(o) {

                    var propVals = o.propData.values;
                    var parent = o.propDiv,
                        rows = Csw.string(propVals.rows),
                        columns = Csw.string(propVals.columns);

                    var commentsDiv = parent.div({
                        value: '',
                        cssclass: 'scrollingdiv',
                        width: '350px'
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
                        FirstCellRightAlign: false,
                        OddCellRightAlign: false,
                        border: 0
                    });
                    var arow = 0;
                    var bgclass = '';
                    Csw.each(propVals.comments, function(acomment) {
                        acomment = acomment || { datetime: '', commenter: '', message: '' };
                        arow += 1;
                        var cell1 = myTable.cell(arow * 2, 1);
                        var cell2 = myTable.cell(arow * 2, 2);
                        if ((arow % 2) === 0) {
                            bgclass = 'OddRow';
                        } else {
                            bgclass = 'EvenRow';
                        }
                        cell1.addClass(bgclass);
                        cell2.addClass(bgclass);
                        cell2.append(acomment.datetime);
                        cell1.append(acomment.commenter);
                        cell2.propDom('align', 'right');
                        cell1.css({ fontStyle: 'italic' });
                        cell2.css({ fontStyle: 'italic' });
                        var cell3 = myTable.cell(arow * 2 + 1, 1);
                        cell3.propNonDom('colspan', '2');
                        cell3.addClass(bgclass);
                        cell3.append(acomment.message);
                    });
                    if (false === o.ReadOnly) {
                        ret.control = parent.textArea({
                            rows: rows,
                            cols: columns,
                            onChange: function () {
                                var comment = ret.control.val();
                                Csw.tryExec(o.onChange, comment);
                                propertyOption.onPropChange({ newmessage: comment });
                            }
                        }); 
                    }
                };

                propertyOption.render(render);
                return ret;
            }));
    
}());
