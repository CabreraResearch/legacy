/* global window:true, Ext:true */

(function(n$) {

    var initTableTree = function(tabTreeDef) {

        var initTreeDragZone = function(thisTree, t) {
            // init tree view as a ViewDragZone
            thisTree.view.dragZone = new Ext.tree.ViewDragZone({
                view: thisTree.view,
                ddGroup: 'sqlDDGroup',
                dragText: '{0} Selected Table{1}',
                repairHighlightColor: 'c3daf9',
                repairHighlight: Ext.enableFx
            });
        };

        /**
         * Define the grid
        */
        var tree = n$.trees.tree({
            id: 'qbTablesTree',
            //TODO: expose
            store: n$.trees.treeStore({
                rootText: 'Tables',
                //dataLoader: n$.dataLoaders.dataLoaderAjax('../../Services/Query/tables'),
                //load: true
                children: [{ "allowDrop": false, "leaf": true, "text": "Assembly" }, { "allowDrop": false, "leaf": true, "text": "Assembly Document" }, { "allowDrop": false, "leaf": true, "text": "Assembly Problem" }, { "allowDrop": false, "leaf": true, "text": "Assembly Schedule" }, { "allowDrop": false, "leaf": true, "text": "Assembly Task" }, { "allowDrop": false, "leaf": true, "text": "Batch Operation" }, { "allowDrop": false, "leaf": true, "text": "Biological" }, { "allowDrop": false, "leaf": true, "text": "Box" }, { "allowDrop": false, "leaf": true, "text": "Building" }, { "allowDrop": false, "leaf": true, "text": "Cabinet" }, { "allowDrop": false, "leaf": true, "text": "Chemical" }, { "allowDrop": false, "leaf": true, "text": "Container" }, { "allowDrop": false, "leaf": true, "text": "Container Dispense Transaction" }, { "allowDrop": false, "leaf": true, "text": "Container Document" }, { "allowDrop": false, "leaf": true, "text": "Container Group" }, { "allowDrop": false, "leaf": true, "text": "Container Location" }, { "allowDrop": false, "leaf": true, "text": "Control Zone" }, { "allowDrop": false, "leaf": true, "text": "Department" }, { "allowDrop": false, "leaf": true, "text": "Enterprise Part" }, { "allowDrop": false, "leaf": true, "text": "Equipment" }, { "allowDrop": false, "leaf": true, "text": "Equipment Document" }, { "allowDrop": false, "leaf": true, "text": "Equipment Problem" }, { "allowDrop": false, "leaf": true, "text": "Equipment Schedule" }, { "allowDrop": false, "leaf": true, "text": "Equipment Task" }, { "allowDrop": false, "leaf": true, "text": "Equipment Type" }, { "allowDrop": false, "leaf": true, "text": "Feedback" }, { "allowDrop": false, "leaf": true, "text": "Fire Class Exempt Amount" }, { "allowDrop": false, "leaf": true, "text": "Fire Class Exempt Amount Set" }, { "allowDrop": false, "leaf": true, "text": "Floor" }, { "allowDrop": false, "leaf": true, "text": "GHS" }, { "allowDrop": false, "leaf": true, "text": "GHS Phrase" }, { "allowDrop": false, "leaf": true, "text": "IMCS Report" }, { "allowDrop": false, "leaf": true, "text": "Inspection Schedule" }, { "allowDrop": false, "leaf": true, "text": "Inventory Group" }, { "allowDrop": false, "leaf": true, "text": "Inventory Group Permission" }, { "allowDrop": false, "leaf": true, "text": "Inventory Level" }, { "allowDrop": false, "leaf": true, "text": "Jurisdiction" }, { "allowDrop": false, "leaf": true, "text": "Lab Safety (demo)" }, { "allowDrop": false, "leaf": true, "text": "Lab Safety Checklist (demo)" }, { "allowDrop": false, "leaf": true, "text": "Lab Safety Group (demo)" }, { "allowDrop": false, "leaf": true, "text": "LQNo" }, { "allowDrop": false, "leaf": true, "text": "Mail Report" }, { "allowDrop": false, "leaf": true, "text": "Manufacturing Equivalent Part" }, { "allowDrop": false, "leaf": true, "text": "Material Component" }, { "allowDrop": false, "leaf": true, "text": "Material Document" }, { "allowDrop": false, "leaf": true, "text": "Material Synonym" }, { "allowDrop": false, "leaf": true, "text": "Print Job" }, { "allowDrop": false, "leaf": true, "text": "Print Label" }, { "allowDrop": false, "leaf": true, "text": "Printer" }, { "allowDrop": false, "leaf": true, "text": "Receipt Lot" }, { "allowDrop": false, "leaf": true, "text": "Regulatory List" }, { "allowDrop": false, "leaf": true, "text": "Report" }, { "allowDrop": false, "leaf": true, "text": "Request" }, { "allowDrop": false, "leaf": true, "text": "Request Container Dispense" }, { "allowDrop": false, "leaf": true, "text": "Request Container Update" }, { "allowDrop": false, "leaf": true, "text": "Request Material Create" }, { "allowDrop": false, "leaf": true, "text": "Request Material Dispense" }, { "allowDrop": false, "leaf": true, "text": "Role" }, { "allowDrop": false, "leaf": true, "text": "Room" }, { "allowDrop": false, "leaf": true, "text": "SDS Document" }, { "allowDrop": false, "leaf": true, "text": "Shelf" }, { "allowDrop": false, "leaf": true, "text": "SI Report" }, { "allowDrop": false, "leaf": true, "text": "Site" }, { "allowDrop": false, "leaf": true, "text": "Size" }, { "allowDrop": false, "leaf": true, "text": "Supply" }, { "allowDrop": false, "leaf": true, "text": "Unit (Each)" }, { "allowDrop": false, "leaf": true, "text": "Unit (Radiation)" }, { "allowDrop": false, "leaf": true, "text": "Unit (Time)" }, { "allowDrop": false, "leaf": true, "text": "Unit (Volume)" }, { "allowDrop": false, "leaf": true, "text": "Unit (Weight)" }, { "allowDrop": false, "leaf": true, "text": "User" }, { "allowDrop": false, "leaf": true, "text": "Vendor" }, { "allowDrop": false, "leaf": true, "text": "Work Unit" }, { "allowDrop": false, "leaf": true, "text": "BatchOpClass" }, { "allowDrop": false, "leaf": true, "text": "ChemicalClass" }, { "allowDrop": false, "leaf": true, "text": "ContainerClass" }, { "allowDrop": false, "leaf": true, "text": "ContainerDispenseTransactionClass" }, { "allowDrop": false, "leaf": true, "text": "ContainerGroupClass" }, { "allowDrop": false, "leaf": true, "text": "ContainerLocationClass" }, { "allowDrop": false, "leaf": true, "text": "DocumentClass" }, { "allowDrop": false, "leaf": true, "text": "EnterprisePartClass" }, { "allowDrop": false, "leaf": true, "text": "EquipmentAssemblyClass" }, { "allowDrop": false, "leaf": true, "text": "EquipmentClass" }, { "allowDrop": false, "leaf": true, "text": "EquipmentTypeClass" }, { "allowDrop": false, "leaf": true, "text": "FeedbackClass" }, { "allowDrop": false, "leaf": true, "text": "FireClassExemptAmountClass" }, { "allowDrop": false, "leaf": true, "text": "FireClassExemptAmountSetClass" }, { "allowDrop": false, "leaf": true, "text": "GHSClass" }, { "allowDrop": false, "leaf": true, "text": "GHSPhraseClass" }, { "allowDrop": false, "leaf": true, "text": "GeneratorClass" }, { "allowDrop": false, "leaf": true, "text": "GenericClass" }, { "allowDrop": false, "leaf": true, "text": "InspectionDesignClass" }, { "allowDrop": false, "leaf": true, "text": "InspectionRouteClass" }, { "allowDrop": false, "leaf": true, "text": "InspectionTargetClass" }, { "allowDrop": false, "leaf": true, "text": "InspectionTargetGroupClass" }, { "allowDrop": false, "leaf": true, "text": "InventoryGroupClass" }, { "allowDrop": false, "leaf": true, "text": "InventoryGroupPermissionClass" }, { "allowDrop": false, "leaf": true, "text": "InventoryLevelClass" }, { "allowDrop": false, "leaf": true, "text": "JurisdictionClass" }, { "allowDrop": false, "leaf": true, "text": "LocationClass" }, { "allowDrop": false, "leaf": true, "text": "MailReportClass" }, { "allowDrop": false, "leaf": true, "text": "ManufacturerEquivalentPartClass" }, { "allowDrop": false, "leaf": true, "text": "MaterialComponentClass" }, { "allowDrop": false, "leaf": true, "text": "MaterialSynonymClass" }, { "allowDrop": false, "leaf": true, "text": "NonChemicalClass" }, { "allowDrop": false, "leaf": true, "text": "PrintJobClass" }, { "allowDrop": false, "leaf": true, "text": "PrintLabelClass" }, { "allowDrop": false, "leaf": true, "text": "PrinterClass" }, { "allowDrop": false, "leaf": true, "text": "ProblemClass" }, { "allowDrop": false, "leaf": true, "text": "ReceiptLotClass" }, { "allowDrop": false, "leaf": true, "text": "RegulatoryListClass" }, { "allowDrop": false, "leaf": true, "text": "ReportClass" }, { "allowDrop": false, "leaf": true, "text": "RequestClass" }, { "allowDrop": false, "leaf": true, "text": "RequestContainerDispenseClass" }, { "allowDrop": false, "leaf": true, "text": "RequestContainerUpdateClass" }, { "allowDrop": false, "leaf": true, "text": "RequestMaterialCreateClass" }, { "allowDrop": false, "leaf": true, "text": "RequestMaterialDispenseClass" }, { "allowDrop": false, "leaf": true, "text": "RoleClass" }, { "allowDrop": false, "leaf": true, "text": "SDSDocumentClass" }, { "allowDrop": false, "leaf": true, "text": "SizeClass" }, { "allowDrop": false, "leaf": true, "text": "TaskClass" }, { "allowDrop": false, "leaf": true, "text": "UnitOfMeasureClass" }, { "allowDrop": false, "leaf": true, "text": "UserClass" }, { "allowDrop": false, "leaf": true, "text": "VendorClass" }, { "allowDrop": false, "leaf": true, "text": "WorkUnitClass" }]
                //children: [
                //    n$.trees.treeNode({ text: 'library' }),
                //    n$.trees.treeNode({ text: 'shelf' }),
                //    n$.trees.treeNode({ text: 'floor' }),
                //    n$.trees.treeNode({ text: 'room' }),
                //    n$.trees.treeNode({ text: 'book' })]
            })
        });

        /**
         * Add the subscribers
        */
        tree.subscribers.add(n$.trees.constants.subscribers.afterrender, function(extView, eOpts) {
            var that = extView;
            initTreeDragZone(that, tree);
        });

        tree.subscribers.add(n$.trees.constants.subscribers.itemdblclick, function(extView, record, item, index, e, eOpts) {
            var qbTablePanel;
            // add a qbSqlWindowTable to the qbTablePanel component
            qbTablePanel = Ext.getCmp('qbTablePanel');
            qbTablePanel.add({
                xtype: 'qbSqlWindowTable',
                constrain: true,
                title: record.get('text')
            }).show();
        });

        tree.init();

        return {
            xtype: 'qbTablesTree',
            border: false,
            region: 'west',
            width: 200,
            height: 400,
            split: true
        };
    };
    n$.actions.querybuilder.register('qbTablesTree', initTableTree);

}(window.$nameSpace$));


