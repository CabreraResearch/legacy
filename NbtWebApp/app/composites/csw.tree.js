(function () {
    'use strict';

    Csw.composites.tree = Csw.composites.tree ||
        Csw.composites.register('tree', function (cswParent, cswPrivate) {

            //#region Variables

            var cswPublic = {
                is: (function() {
                    var isMulti = false;
                    return {
                        get multi () {
                            return isMulti;
                        },
                        set multi (val) {
                            if(val === true) {
                                isMulti = true;
                            } else {
                                isMulti = false;
                            }
                            return isMulti;
                        },
                        toggleMulti: function() {
                            isMulti = !isMulti;
                            return isMulti;
                        }
                    };
                }())
            };

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
                    menuDisabled: true
                }];

                cswPrivate.fields = cswPrivate.fields || [{
                    name: 'text',
                    type: 'string'
                }];

                //State
                cswPublic.is.multi = cswPrivate.isMulti;
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
                        root: cswPrivate.root[0]
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
                        Csw.tryExec(cswPrivate.onMouseEnter, window.event, treeNode);
                    },
                    itemmouseleave: function(thisView, treeNode, htmlElement, index, eventObj, eOpts) {
                        Csw.tryExec(cswPrivate.onMouseExit, window.event, treeNode);
                    },
                    afterlayout: function() {
                        //afterlayout fires anytime you expand/collapse nodes in the tree. It fires once for all new content.
                        cswPublic.toggleCheckboxes();
                        $('.x-grid-cell-treecolumn').css({ background: 'transparent' });
                    },
                    afterrender: function() {
                        //Despite the fact that this is the last "render" event to fire, the tree is still _NOT_ in the DOM. 
                        //It _will_ in the next nano-second, so we have to defer.
                        cswPublic.toggleCheckboxes();
                    },
                    viewready: function() {
                        //This is the "last" event to fire, but it's _still_ not safe to assume the DOM is ready.
                        Csw.defer(function () {
                            cswPrivate.rootNode = cswPublic.tree.getRootNode();
                            var firstChild = cswPrivate.rootNode.childNodes[0];
                            cswPublic.selectNode(firstChild);
                            cswPublic.toggleMultiEdit(cswPublic.is.multi);
                        }, 10);
                        
                    },
                    afteritemcollapse: function() {
                        cswPublic.toggleCheckboxes();
                    },
                    select: function(rowModel, record, index, eOpts) {
                        //If you click a node, this event is firing. We must:
                        //1. Keep this generic. Defer to the caller for implementation-specific validation.
                        //2. Properly track the currently and previously selected node (multiple clicks to the same node trigger this event)
                        var ret = true;
                        if (cswPublic.is.multi !== true) {
                            if (record != cswPublic.selectedTreeNode) {
                                record.expand();
                                cswPublic.previousTreeNode = cswPublic.selectedTreeNode;
                                cswPublic.selectedTreeNode = record;
                                //If we're in single edit mode, the count is always 1
                                cswPrivate.selectedNodeCount = 1;
                            }
                        } 
                        if (cswPrivate.selectedNodeCount === 1) {
                            //In either single or multi-edit, render whenever the node count is 1
                            Csw.tryExec(cswPrivate.onSelect, record.raw);
                        }
                        return ret;
                    },
                    checkchange: function (record, checked, eOpts) {
                        //Unfortunately, this event doesn't fire regularly if watched. Debug carefully!

                        //Ext doesn't update the raw data, which is the only thing we have to manage state.
                        //Manually keep it up to date.
                        record.raw.checked = checked;
                        if (cswPublic.is.multi) {
                            var tmpPrevNode = cswPublic.selectedTreeNode;
                            var tmpCrntNode = record;
                            
                            if (null === tmpPrevNode ||
                                tmpPrevNode.raw.checked === false || 
                                null === cswPublic.selectedTreeNode || 
                                Csw.tryExec(cswPrivate.allowMultiSelection, tmpPrevNode, tmpCrntNode)
                             ) {
                                //if the previous node was null (starting from the root) or if the previous node is unchecked
                                //or if the currently selected node is null (unlikely)
                                //or if the caller's algorithm to allow simultaneous selection passes, 
                                //then increment up or down and set the current and previous nodes accordingly
                                var inc = (checked) ? 1 : -1;
                                if (cswPrivate.selectedNodeCount <= 1 || null === cswPrivate.firstSelectedNode) {
                                    cswPrivate.firstSelectedNode = tmpCrntNode;
                                } else {
                                    cswPublic.selectNode(cswPrivate.firstSelectedNode);
                                }
                                cswPublic.previousTreeNode = tmpPrevNode;
                                cswPublic.selectedTreeNode = tmpCrntNode;
                                cswPrivate.selectedNodeCount += inc;
                            } else {
                                //else, manually "uncheck" this node and select the unchanged current node
                                record.raw.checked = false;
                                record.set('checked', false);
                                if (cswPrivate.selectedNodeCount > 0) {
                                    cswPublic.selectNode(cswPrivate.firstSelectedNode);
                                } 
                            }
                        }
                    }
                };
            };
            
            cswPrivate.firstSelectedNode = null;
            cswPrivate.selectedNodeCount = 0;
            cswPublic.selectedTreeNode = null;
            cswPublic.previousTreeNode = null;

            cswPrivate.makeTree = function () {
            	/// <summary>
            	/// Constructs the tree components and attaches it to the DOM
            	/// </summary>
                /// <returns type="Ext.tree.Panel">A tree.</returns>
                var allExpanded = false;
                var hideHeaders = (false === Csw.clientSession.isDebug());
                var treeOpts = {
                    store: cswPrivate.makeStore(), //just like grids, we need a data store
                    renderTo: cswPublic.div.getId(),
                    height: cswPrivate.height,
                    width: cswPrivate.width,
                    //maxWidth: cswPrivate.width,
                    title: cswPrivate.root[0].text,  //root should already be validated at this point
                    useArrows: cswPrivate.useArrows, //not for List views
                    rootVisible: false, //the root node (ViewName) is displayed as the title
                    hideHeaders: hideHeaders, //this hides the tree grid column names
                    border: false,
                    bodyStyle: 'background: transparent !important;',
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
                cswPrivate.firstSelectedNode = null;
                $('.x-tree-checkbox').hide();
                return true;
            };
            
            cswPublic.toggleMultiEdit = function() {
            	/// <summary>
            	/// Toggles Multi-Edit state on this instance.
                /// </summary>
                var selModel = cswPublic.tree.getSelectionModel();
                if (cswPublic.is.multi) {
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
                if (true === cswPublic.is.multi) {
                    cswPrivate.showCheckboxes();
                } else {
                    cswPrivate.hideCheckboxes();
                }
                return cswPublic;
            };

            cswPublic.collapseAll = function () {
                /// <summary>
                /// Collapses all nodes in the tree.
                /// </summary>
                /// <returns type="Csw.composites.tree">This tree</returns>
                //cswPublic.tree.getEl().mask('Collapsing tree...');
                cswPublic.tree.collapseAll(function () {
                    //cswPublic.tree.getEl().unmask();
                });
                return cswPublic;
            };

            cswPublic.expandAll = function () {
                /// <summary>
                /// Expand all nodes in the tree.
                /// </summary>
                /// <returns type="Csw.composites.tree">This tree</returns>
                //cswPublic.tree.getEl().mask('Expanding tree...');
                cswPublic.tree.expandAll(function () {
                    //cswPublic.tree.getEl().unmask();
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
                var checked = cswPublic.tree.getChecked() || [];
                if (checked.length === 0) {
                    //We've switched states and the selectionModel change has not taken effect.
                    cswPublic.eachNode(function(node) {
                        //Iterate the tree. If the node is checked and it validates, add it to the return.
                        if (node && node.raw && node.raw.checked) {
                            if (Csw.tryExec(cswPrivate.allowMultiSelection, node, cswPublic.selectedTreeNode)) {
                                checked.push(node);
                            } else {
                                node.raw.checked = false;
                                node.set('checked', false);
                            }
                        }
                    });
                }
                return checked;
            };

            cswPrivate.eachNodeRecursive = function (node, callBack) {
                if(node && callBack) {
                    callBack(node);
                    if(node.childNodes && node.childNodes.length > 0) {
                        node.childNodes.forEach(function(childNode) {
                            cswPrivate.eachNodeRecursive(childNode, callBack);
                         });
                    }
                }
            };

            cswPublic.eachNode = function(callBack) {
                if(cswPrivate.rootNode && callBack) {
                    cswPrivate.eachNodeRecursive(cswPrivate.rootNode, callBack);
                }
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
