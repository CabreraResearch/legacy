(function _initDDComponents() {
    
    Csw.main.onReady.then(function () {

        //A draggable div
        window.Ext.define('Csw.ext.draggable', {
            extend: 'Ext.panel.Panel',
            alias: 'widget.draggable',
            id: '',
            title: '',
            layout: 'fit',
            anchor: '100%',
            frame: true,
            closable: true,
            collapsible: true,
            draggable: { //setting this to "true" would mean we can drag an obj of this type ANYWHERE. "moveOnDrag: false" means the obj will snap the the target drop zone
                moveOnDrag: false
            },
            cls: 'csw-draggable',
            header: false,
            border: 0,
            items: [],
            style: {
                'margin-bottom': '10px',
                'padding': '1px',
                'cursor': 'default' //suppress the drag cursor
            },

            //custom configs:
            allowDrag: function (allow) { //Whether or not this item can be moved around
                if (allow) {
                    this.dd.unlock();
                } else {
                    this.dd.lock();
                }
            }
        });

        //A column that csw.draggables reside in
        window.Ext.define('Csw.ext.dragcolumn', {
            extend: 'Ext.container.Container',
            alias: 'widget.dragcolumn',
            requires: [
                'Ext.layout.container.Anchor',
                'Csw.ext.draggable'
            ],
            cls: 'csw-dd-column',
            layout: 'anchor',
            defaultType: 'draggable'
        });

        //A drop zone
        window.Ext.define('Csw.ext.dropzone', {
            extend: 'Ext.dd.DropTarget',
            constructor: function (portal, cfg) {
                this.portal = portal;
                window.Ext.dd.ScrollManager.register(portal.body);
                Csw.ext.dropzone.superclass.constructor.call(this, portal.body, cfg);
                portal.body.ddScrollConfig = this.ddScrollConfig;
            },
            ddScrollConfig: {
                vthresh: 50,
                hthresh: -1,
                animate: true,
                increment: 200
            },
            createEvent: function (dd, e, data, col, c, pos) {
                return {
                    portal: this.portal,
                    panel: data.panel,
                    columnIndex: col,
                    column: c,
                    position: pos,
                    data: data,
                    source: dd,
                    rawEvent: e,
                    status: this.dropAllowed
                };
            },
            notifyOver: function (dd, e, data) {
                var xy = e.getXY(),
                    portal = this.portal,
                    proxy = dd.proxy;

                // case column widths
                if (!this.grid) {
                    this.grid = this.getGrid();
                }

                // handle case scroll where scrollbars appear during drag
                var cw = portal.body.dom.clientWidth;
                if (!this.lastCW) {
                    // set initial client width
                    this.lastCW = cw;
                } else if (this.lastCW != cw) {
                    // client width has changed, so refresh layout & grid calcs
                    this.lastCW = cw;
                    portal.doLayout();
                    this.grid = this.getGrid();
                }

                // determine column
                var colIndex = 0,
                    colRight = 0,
                    cols = this.grid.columnX,
                    len = cols.length,
                    cmatch = false;

                for (len; colIndex < len; colIndex++) {
                    colRight = cols[colIndex].x + cols[colIndex].w;
                    if (xy[0] < colRight) {
                        cmatch = true;
                        break;
                    }
                }
                // no match, fix last index
                if (!cmatch) {
                    colIndex--;
                }

                // find insert position
                var overPortlet, pos = 0,
                    h = 0,
                    match = false,
                    overColumn = portal.items.getAt(colIndex),
                    portlets = overColumn.items.items,
                    overSelf = false;

                len = portlets.length;

                for (len; pos < len; pos++) {
                    overPortlet = portlets[pos];
                    h = overPortlet.el.getHeight();
                    if (h === 0) {
                        overSelf = true;
                    } else if ((overPortlet.el.getY() + (h / 2)) > xy[1]) {
                        match = true;
                        break;
                    }
                }

                pos = (match && overPortlet ? pos : overColumn.items.getCount()) + (overSelf ? -1 : 0);
                var overEvent = this.createEvent(dd, e, data, colIndex, overColumn, pos);

                if (portal.fireEvent('validatedrop', overEvent) !== false && portal.fireEvent('beforedragover', overEvent) !== false) {

                    // make sure proxy width is fluid in different width columns
                    proxy.getProxy().setWidth('auto');
                    if (overPortlet) {
                        dd.panelProxy.moveProxy(overPortlet.el.dom.parentNode, match ? overPortlet.el.dom : null);
                    } else {
                        dd.panelProxy.moveProxy(overColumn.el.dom, null);
                    }

                    this.lastPos = {
                        c: overColumn,
                        col: colIndex,
                        p: overSelf || (match && overPortlet) ? pos : false
                    };
                    this.scrollPos = portal.body.getScroll();

                    portal.fireEvent('dragover', overEvent);
                    return overEvent.status;
                } else {
                    return overEvent.status;
                }

            },
            notifyOut: function () {
                delete this.grid;
            },
            notifyDrop: function (dd, e, data) {
                delete this.grid;
                if (!this.lastPos) {
                    return;
                }
                var c = this.lastPos.c,
                    col = this.lastPos.col,
                    pos = this.lastPos.p,
                    panel = dd.panel,
                    dropEvent = this.createEvent(dd, e, data, col, c, pos !== false ? pos : c.items.getCount());

                if (this.portal.fireEvent('validatedrop', dropEvent) !== false &&
                    this.portal.fireEvent('beforedrop', dropEvent) !== false) {

                    Ext.suspendLayouts();

                    // make sure panel is visible prior to inserting so that the layout doesn't ignore it
                    panel.el.dom.style.display = '';
                    dd.panelProxy.hide();
                    dd.proxy.hide();

                    if (pos !== false) {
                        c.insert(pos, panel); //add the draggabke into it's new spot
                    } else {
                        c.add(panel); //snap the draggable back to it's prev spot
                    }

                    Ext.resumeLayouts(true);

                    this.portal.fireEvent('drop', dropEvent);

                    // scroll position is lost on drop, fix it
                    var st = this.scrollPos.top;
                    if (st) {
                        var d = this.portal.body.dom;
                        setTimeout(function () {
                            d.scrollTop = st;
                        },
                        10);
                    }
                }

                delete this.lastPos;
                return true;
            },
            // internal cache of body and column coords
            getGrid: function () {
                var box = this.portal.body.getBox();
                box.columnX = [];
                this.portal.items.each(function (c) {
                    box.columnX.push({
                        x: c.el.getX(),
                        w: c.el.getWidth()
                    });
                });
                return box;
            },
            // unregister the dropzone from ScrollManager
            unreg: function () {
                window.Ext.dd.ScrollManager.unregister(this.portal.body);
                //window.Ext.app.PortalDropZone.superclass.unreg.call(this);
                Csw.ext.dropzone.superclass.unreg.call(this);
            }
        });

        //A panel that contains columns that contains draggables
        window.Ext.define('Csw.ext.dragpanel', {
            extend: 'Ext.panel.Panel',
            alias: 'widget.dragpanel',
            requires: [
                'Ext.layout.container.Column',
                'Csw.ext.dropzone',
                'Csw.ext.dragcolumn'
            ],
            cls: 'csw-dd-panel',
            bodyCls: 'csw-dd-panel-body',
            defaultType: 'dragcolumn',
            autoScroll: true,
            manageHeight: false,
            initComponent: function () {
                var me = this;

                // Implement a Container beforeLayout call from the layout to this Container
                this.layout = {
                    type: 'column'
                };
                this.callParent();

                this.addEvents({
                    validatedrop: true,
                    beforedragover: true,
                    dragover: true,
                    beforedrop: true,
                    drop: true
                });
            },
            // Set columnWidth, and set first and last column classes to allow exact CSS targeting.
            beforeLayout: function () {
                var items = this.layout.getLayoutItems(),
                    len = items.length,
                    firstAndLast = ['csw-dd-column-first', 'csw-dd-column-last'],
                    i, item, last;

                for (i = 0; i < len; i++) {
                    item = items[i];
                    item.columnWidth = 1 / len;
                    last = (i == len - 1);

                    if (!i) { // if (first)
                        if (last) {
                            item.addCls(firstAndLast);
                        } else {
                            item.addCls('csw-dd-column-first');
                            item.removeCls('csw-dd-column-last');
                        }
                    } else if (last) {
                        item.addCls('csw-dd-column-last');
                        item.removeCls('csw-dd-column-first');
                    } else {
                        item.removeCls(firstAndLast);
                    }
                }

                return this.callParent(arguments);
            },
            // private
            initEvents: function () {
                this.callParent();
                this.dd = window.Ext.create('Csw.ext.dropzone', this, this.dropConfig);
            },
            // private
            beforeDestroy: function () {
                if (this.dd) {
                    this.dd.unreg();
                }
                this.callParent();
            }
        });
        
    }()); //mainOnReady

}()); //_initDDComponents