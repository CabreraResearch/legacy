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

                //Data for the tree
                cswPrivate.root = cswPrivate.root || {
                    expanded: true,
                    children: [
                        { text: 'No Children', leaf: true }
                    ],
                    text: 'No Tree Provided'
                };

                cswPrivate.columns = cswPrivate.columns || [{
                    xtype: 'treecolumn', //this is so we know which column will show the tree
                    dataIndex: 'text',
                    menuDisabled: true,
                    width: 500 - 40
                }];

                cswPrivate.fields = cswPrivate.fields || [{
                    name: 'text',
                    type: 'string'
                }];

                //State
                cswPrivate.isMulti = cswPrivate.isMulti || false;
                cswPrivate.allowMultiSelection = cswPrivate.allowMultiSelection || function() {};


                //Styling
                cswPrivate.height = cswPrivate.height || 410;
                cswPrivate.width = cswPrivate.width || 270;
                cswPrivate.title = cswPrivate.title || 'No Title';
                cswPrivate.useArrows = cswPrivate.useArrows; //For Lists, useArrows should be false
                cswPrivate.useToggles = cswPrivate.useToggles;

                //Events
                cswPrivate.onClick = cswPrivate.onClick || function () { };
                cswPrivate.onMouseEnter = cswPrivate.onMouseEnter || function () { };
                cswPrivate.onMouseExit = cswPrivate.onMouseExit || function () { };

                cswParent.empty();
                cswPublic.div = cswParent.div();

            }());

            //#endregion Pre-ctor


            //#region Define Class Members

            cswPrivate.makeStore = function () {
                /// <summary>
                /// Makes the data store for the tree. Non-volatile.
                /// </summary>
                /// <returns type="Ext.data.TreeStore">Ext data store</returns>
                window.Ext.define('Tree', {
                    extend: 'Ext.data.Model',
                    fields: cswPrivate.fields
                });

                cswPrivate.store = cswPrivate.store ||
                    window.Ext.create('Ext.data.TreeStore', {
                        model: 'Tree',
                        root: cswPrivate.root[0],
                        sorters: [{
                            property: 'text',
                            direction: 'ASC'
                        }]
                    });
                return cswPrivate.store;
            };

            cswPrivate.makeListeners = function() {
            	/// <summary>
            	/// Simple abstraction for event bindings.
            	/// </summary>
            	/// <returns type="Object">Set of events to bind</returns>
                return {
                    //*click*: function() {
                    // Ext exposes a slew of click handlers. None of them work unless the node is *selected*, so don't bother.
                    //}
                    itemmouseenter: function (thisView, treeNode, htmlElement, index, eventObj, eOpts) {
                        Csw.tryExec(cswPrivate.onMouseEnter, event, treeNode);
                    },
                    itemmouseleave: function(thisView, treeNode, htmlElement, index, eventObj, eOpts) {
                        Csw.tryExec(cswPrivate.onMouseExit, event, treeNode);
                    },
                    afterlayout: function() {
                        //afterlayout fires anytime you expand/collapse nodes in the tree. It fires once for all new content.
                        cswPublic.toggleCheckboxes();
                    },
                    afterrender: function() {
                        //Despite the fact  that this is the last event to fire, the tree is still _NOT_ in the DOM. 
                        //It _will_ in the next nano-second, so we have to defer.
                        //This might not yet be bullet-proof.
                        Csw.defer(function () {
                            cswPrivate.rootNode = cswPublic.tree.getRootNode();
                            var firstChild = cswPrivate.rootNode.childNodes[0];
                            cswPublic.selectNode(firstChild);
                            //firstChild.expand();
                        }, 100);
                    },
                    select: function(rowModel, record, index, eOpts) {
                        //If you click a node, this event is firing. We must:
                        //1. Keep this generic. Defer to the caller for implementation-specific validation.
                        //2. Properly enforce Multi-Edit (likely deferrals to the caller)
                        //3. Properly track the currently and previously selected node (multiple clicks to the same node trigger this event)
                        var ret = true;
                        if (cswPrivate.isMulti) {
                            if (record.raw.checked === true) {
                                record.set('checked', false);
                            } else {
                                var tmpPrevNode = cswPublic.selectedTreeNode;
                                var tmpCrntNode = record;
                                if (null === cswPublic.selectedTreeNode || Csw.tryExec(cswPrivate.allowMultiSelection, tmpPrevNode, tmpCrntNode)) {
                                    record.set('checked', true);
                                    cswPublic.previousTreeNode = tmpPrevNode;
                                    cswPublic.selectedTreeNode = tmpCrntNode;
                                } else {
                                    record.set('checked', false);
                                    ret = false;
                                    cswPublic.selectNode(cswPublic.selectedTreeNode);
                                }
                            }
                        } else {
                            if (record != cswPublic.selectedTreeNode) {
                                record.expand();
                                cswPublic.previousTreeNode = cswPublic.selectedTreeNode;
                                cswPublic.selectedTreeNode = record;

                                Csw.tryExec(cswPrivate.onSelect, record.raw);
                            }
                        }
                        return ret;
                    },
                    checkchange: function(node, checked, eOpts) {
                        //Counter-intuitively, this event only fires on selected nodes, so we can't use for Multi-Edit without also using select to trigger this event
                        var tmpPrevNode = cswPublic.selectedTreeNode;
                        var tmpCrntNode = node;
                        if (null === cswPublic.selectedTreeNode || Csw.tryExec(cswPrivate.allowMultiSelection, tmpPrevNode, tmpCrntNode)) {
                            node.set('checked', true);
                        } else {
                            node.set('checked', false);
                            cswPublic.selectNode(cswPublic.selectedTreeNode);
                        }
                    }
                };
            };

            cswPublic.selectedTreeNode = null;
            cswPublic.previousTreeNode = null;

            cswPrivate.makeTree = function () {
            	/// <summary>
            	/// Constructs the tree components and attaches it to the DOM
            	/// </summary>
                /// <returns type="Ext.tree.Panel">A tree.</returns>
                var allExpanded = false;

                var treeOpts = {
                    store: cswPrivate.makeStore(), //just like grids, we need a data store
                    renderTo: cswPublic.div.getId(),
                    height: cswPrivate.height,
                    width: cswPrivate.width,
                    title: cswPrivate.root[0].text,  //root should already be validated at this point
                    useArrows: cswPrivate.useArrows, //not for List views
                    rootVisible: false, //the root node (ViewName) is displayed as the title
                    hideHeaders: true, //this hides the tree grid column names
                    listeners: cswPrivate.makeListeners(),
                    columns: cswPrivate.columns, //this is secretly a tree grid
                    dockedItems: {
                        xtype: 'toolbar',
                        items: []
                    },
                    plugins: [new Ext.ux.tree.plugin.NodeDisabled()]
                };

                if (cswPrivate.useArrows) { //then show expand/collapse
                    if (cswPrivate.useToggles !== false) {
                        treeOpts.dockedItems.items.push({
                            text: 'Expand All',
                            handler: function () {
                                var toolbar = this.up('toolbar');
                                toolbar.disable();

                                if (allExpanded === false) {
                                    this.setText('Collapse All');
                                    cswPublic.tree.getEl().mask('Expanding tree...');
                                    cswPublic.tree.expandAll(function () {
                                        cswPublic.tree.getEl().unmask();
                                        toolbar.enable();
                                    });
                                } else {
                                    this.setText('Expand All');
                                    cswPublic.tree.getEl().mask('Collapsing tree...');
                                    cswPublic.tree.collapseAll(function () {
                                        cswPublic.tree.getEl().unmask();
                                        toolbar.enable();
                                    });
                                }
                                allExpanded = !allExpanded;
                            }
                        });

                    }
                }
                
                cswPublic.tree = window.Ext.create('Ext.tree.Panel', treeOpts);
                return cswPublic.tree;
            };

            //#region Tree Mutators

            cswPrivate.showCheckboxes = function () {
            	/// <summary>
            	/// For Multii-Edit, uses jQuery selector to show all checkboxes.
            	/// </summary>
                $('.x-tree-checkbox').show();
                return true;
            };

            cswPrivate.hideCheckboxes = function () {
                /// <summary>
                /// For Multii-Edit, uses jQuery selector to hide all checkboxes.
                /// </summary>
                $('.x-tree-checkbox').hide();
                return true;
            };

            cswPublic.toggleMultiEdit = function(isMultiOverride) {
            	/// <summary>
            	/// Toggles Multi-Edit state on this instance.
            	/// </summary>
                cswPrivate.isMulti = false !== isMultiOverride && !cswPrivate.isMulti;
                var selModel = cswPublic.tree.getSelectionModel();
                if (cswPrivate.isMulti) {
                    selModel.setSelectionMode('MULTI');
                } else {
                    selModel.setSelectionMode('SINGLE');
                }
                cswPublic.toggleCheckboxes();
                return true;
            };

            cswPublic.toggleCheckboxes = function () {
                /// <summary>
                /// Shows or hides all checkboxes according to Multi-Edit state.
                /// </summary>
                /// <returns type="Csw.composites.tree">This tree</returns>
                if (true === cswPrivate.isMulti) {
                    $('.x-tree-checkbox').show();
                } else {
                    $('.x-tree-checkbox').hide();
                }
                return cswPublic;
            };

            cswPublic.collapseAll = function () {
                /// <summary>
                /// Collapses all nodes in the tree.
                /// </summary>
                /// <returns type="Csw.composites.tree">This tree</returns>
                cswPublic.tree.getEl().mask('Collapsing tree...');
                cswPublic.tree.collapseAll(function () {
                    cswPublic.tree.getEl().unmask();
                });
                return cswPublic;
            };

            cswPublic.expandAll = function () {
                /// <summary>
                /// Expand all nodes in the tree.
                /// </summary>
                /// <returns type="Csw.composites.tree">This tree</returns>
                cswPublic.tree.getEl().mask('Expanding tree...');
                cswPublic.tree.expandAll(function () {
                    cswPublic.tree.getEl().unmask();
                });
                return cswPublic;
            };

            cswPublic.selectNode = function (treeNode) {
            	/// <summary>
            	/// Selects a node from the tree and renders it as the currently selected node
            	/// </summary>
                /// <param name="treeNode"></param>
                var path = cswPublic.getPath(treeNode);
                if (path) {
                    cswPublic.tree.selectPath(path, null, '|');
                }
                return false;
            };

            cswPublic.addToolbarItem = function(itemDef, position) {
            	/// <summary>
            	/// Add a docked item to the toolbar
            	/// </summary>
            	/// <returns type="Csw.composites.tree">This tree</returns>
                if (itemDef && itemDef.text && itemDef.handler) {
                    if (!itemDef.dock) {
                        itemDef.dock = 'top';
                    }
                    cswPublic.tree.addDocked(itemDef, position);
                }
                return cswPublic;
            };

            //#endregion Tree Mutators

            //#region Getters

            cswPublic.getPath = function (treeNode) {
            	/// <summary>
            	/// Get the path of a tree node from root
            	/// </summary>
            	/// <returns type="String"></returns>
                var ret = '';
                if (treeNode && treeNode.raw && treeNode.raw.path) {
                    ret = treeNode.raw.path;
                }
                return ret;
            };

            cswPublic.getChecked = function () {
                /// <summary>
                /// For Multii-Edit, get all nodes which are selected (checked) in the tree.
                /// </summary>
                var checked = cswPublic.tree.getChecked();
                var ret = [];
                if (checked && checked.length > 0) {
                    checked.forEach(function(treeNode) {
                        ret.push({ nodeid: treeNode.raw.nodeid, nodekey: treeNode.raw.id });
                    });
                }
                return ret;
            };

            //#endregion Getters
            
            //#endregion Define Class Members


            //#region Post-ctor

            (function _post() {
                if (cswPrivate.root && cswPrivate.root.length >= 1) {
                    cswPrivate.makeTree();
                } else {
                    //throw
                }

            }());

            //#endregion Post-ctor

            return cswPublic;
        });

}());
