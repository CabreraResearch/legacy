(function () {
    'use strict';
    
    Csw.composites.tree = Csw.composites.tree ||
        Csw.composites.register('tree', function (cswParent, cswPrivate) {

            //#region Variables

            var cswPublic = {};

            //#endregion Variables

            //#region Pre-ctor
            (function _pre() {
                //set default values on cswPrivate if none are supplied
                cswPrivate.name = cswPrivate.name || 'No name';
                cswPrivate.bodyStyle = cswPrivate.bodyStyle || ''; //'background-img:url(path/to/img)
                cswPrivate.root = cswPrivate.root || {
                    expanded: true,
                    children: [
                        { text: 'No Children', leaf: true },
                    ],
                    text: 'No Tree Provided'
                };
                cswPrivate.height = cswPrivate.height || 600;
                cswPrivate.width = cswPrivate.width || 250;
                cswPrivate.title = cswPrivate.title || 'No Title';
                cswPrivate.useArrows = cswPrivate.useArrows; //For Lists, useArrows should be false

                cswPrivate.onClick = cswPrivate.onClick || function() {};
                cswPrivate.onMouseEnter = cswPrivate.onMouseEnter || function () { };
                cswPrivate.onMouseExit = cswPrivate.onMouseExit || function () { };

                cswParent.empty();
                cswPublic = cswParent.div();

            }());

            //#endregion Pre-ctor


            //#region Define Class Members

            cswPrivate.makeStore = function() {
                cswPrivate.store = cswPrivate.store ||
                    window.Ext.create('Ext.data.TreeStore', {
                        root: cswPrivate.treeRoot,
                        sorters: [{
                            property: 'text',
                            direction: 'ASC'
                        }]
                    });
            };

            cswPrivate.makeTree = function() {
                cswPrivate.store = cswPrivate.store || cswPrivate.makeStore();

                var treeOpts = {
                    store: cswPrivate.store,
                    renderTo: cswPublic.getId(),
                    height: cswPrivate.height,
                    width: cswPrivate.width,
                    title: cswPrivate.title,
                    useArrows: cswPrivate.useArrows,
                    listeners: {
                        click: function(a,b,c,d,e,f,g) {
                            Csw.tryExec(cswPrivate.onClick);
                        },
                        mouseover: function(a,b,c,d,e,f,g) {
                            Csw.tryExec(cswPrivate.onMouseEnter);
                        },
                        mouseout: function(a,b,c,d,e,f,g) {
                            Csw.tryExec(cswPrivate.onMouseExit);
                        },
                    }
                };
                if (cswPrivate.useArrows) { //then show expand/collapse
                    treeOpts.dockedItems = [{
                        xtype: 'toolbar',
                        items: [{
                                text: 'Expand All',
                                handler: function() {
                                    tree.getEl().mask('Expanding tree...');
                                    var toolbar = this.up('toolbar');
                                    toolbar.disable();

                                    tree.expandAll(function() {
                                        tree.getEl().unmask();
                                        toolbar.enable();
                                    });
                                }
                            }, {
                                text: 'Collapse All',
                                handler: function() {
                                    var toolbar = this.up('toolbar');
                                    toolbar.disable();

                                    tree.collapseAll(function() {
                                        toolbar.enable();
                                    });
                                }
                            }]
                    }];
                }
                
                cswPrivate.tree = cswPrivate.tree ||
                    window.Ext.create('Ext.tree.Panel', treeOpts);
            };

            //#endregion Define Class Members


            //#region Post-ctor

            (function _post() {
                cswPrivate.makeTree();

            }());

            //#endregion Post-ctor

            return cswPublic;
        });

}());
