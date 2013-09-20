/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.actions.register('subscriptions', function (cswParent, options) {

        var cswPrivate = {
            name: 'action_subscriptions'
        };
        if (options) Csw.extend(cswPrivate, options)

        var cswPublic = {};

        // constructor
        (function () {

            Csw.ajaxWcf.get({
                urlMethod: 'Reports/getSubscriptions',
                success: function (ajaxdata) {
                    var row = 1,
                        atLeastOne = false;

                    cswPrivate.data = {
                        Subscriptions: [{
                            Name: '',
                            NodeId: '',
                            Subscribed: '',
                            Modified: '',
                            IsDemo: ''
                        }]
                    };
                    Csw.extend(cswPrivate.data, ajaxdata);

                    cswParent.br();

                    cswPrivate.descriptionSpan = cswParent.span({ text: "Subscribe to Mail Reports:" }).css({ fontWeight: 'bold' });
                    cswPrivate.table = cswParent.table({
                        suffix: 'tbl',
                        cellpadding: 2,
                        cellvalign: 'middle'
                    }).css({ padding: '10px' });

                    Csw.each(cswPrivate.data.Subscriptions, function (subObj) {
                        atLeastOne = true;
                        cswPrivate.table.cell(row, 1).css({ width: '15px' })
                                                     .checkBox({
                                                         onChange: function (newval) {
                                                             subObj.Subscribed = newval;
                                                             subObj.Modified = true;
                                                         },
                                                         checked: subObj.Subscribed
                                                     }); // checkBox
                        cswPrivate.table.cell(row, 2).text(subObj.Name);
                        if (subObj.IsDemo) {
                            cswPrivate.table.cell(row, 3).text('(DEMO)');
                        }
                        row += 1;
                    }); // each

                    //save button, onClick ajax call - takes all checked items and subscribes them
                    cswPrivate.saveBtn = cswParent.buttonExt({
                        enabledText: 'Save Subscriptions',
                        disabledText: 'Saving...',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                        onClick: function () {
                            Csw.ajaxWcf.post({
                                urlMethod: 'Reports/saveSubscriptions',
                                data: cswPrivate.data,
                                success: function (data) {
                                    cswPrivate.saveBtn.enable();
                                } // success
                            }); // ajax
                        } // onClick
                    }); // saveBtn

                    if (false === atLeastOne) {
                        cswPrivate.descriptionSpan.text('No Mail Reports are available.');
                        cswPrivate.table.hide();
                        cswPrivate.saveBtn.hide();
                    }

                } // success
            }); // ajax
        }());

        return cswPublic;
    });
}());
