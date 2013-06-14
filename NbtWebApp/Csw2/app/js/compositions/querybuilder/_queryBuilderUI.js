/* jshint undef: true, unused: true */
/* global Ext  */

(function (n$) {

    window.initSqlBuilder = function () {
        // Init the singleton.  Any tag-based quick tips will start working.
        Ext.tip.QuickTipManager.init();

        // create main application namespace $nameSpace$.sql
        Ext.namespace(n$.name + '.sql');

        var onInit = function (that) {
            n$.actions.sql.init();

            // disable gutter (linenumbers) and toolbar for SyntaxHighlighter
            window.SyntaxHighlighter.defaults['gutter'] = false;
            window.SyntaxHighlighter.defaults['toolbar'] = false;

            // add toolbar to the dockedItems
            that.dockedItems = [{
                xtype: 'toolbar',
                dock: 'top',
                items: [{
                    xtype: 'tbfill'
                }, {
                    text: "Save",
                    icon: "img/icon-save.gif"
                }, {
                    text: "Run",
                    icon: "img/run.png"
                }]
            }];
        };

        var sheet = n$.sheets.sheet({
            id: 'qbwindow',
            onInit: onInit
        });

        sheet.addProp('height', 620);
        sheet.addProp('width', 1000);
        sheet.addProp('title', 'Visual SQL Query Builder');
        sheet.addProp('layout', {
            type: 'border'
        });

        var items = [
            n$.actions.querybuilder.qbOutputPanel,
            {
                xtype: 'panel',
                border: false,
                height: 400,
                margin: 5,
                layout: {
                    type: 'border'
                },
                region: 'north',
                split: true,
                items: [
                    n$.actions.querybuilder.qbTablePanel,
                    n$.actions.querybuilder.qbFineTuningGrid,
                    n$.actions.querybuilder.qbTablesTree]
            }
        ];

        sheet.addProp('items', items);

        sheet.init();

        Ext.application({
            name: n$.name + '.qbwindow',
            appFolder: 'sql',
            autoCreateViewport: false,
            errorHandler: function (err) {
                Cs2.console.error(err);
            },
            launch: function () {
                Ext.Error.handle = this.errorHandler;
                // copy application to $nameSpace$.sql so that $nameSpace$.sql.app can be used as an application singleton
                var qbWindow = Ext.create('Ext.' + n$.name + '.qbwindow');
                qbWindow.show();
                Ext.apply(n$.sql, this);
            }
        });
    };
}(window.$nameSpace$));
